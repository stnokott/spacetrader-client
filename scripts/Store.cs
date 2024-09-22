using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Google.Protobuf;
using Models;
using MQTTnet;
using MQTTnet.Client;



public partial class Store : Node
{
	public static Store Instance { get; private set; }

	private MqttFactory _mqttFactory;
	private IMqttClient _mqtt;

	public ServerStatusModel ServerStatus = new() { Version = "v0.0.0", NextReset = DateTime.MaxValue };
	public AgentInfoModel Agent = new() { Name = "!AGENTNAME", Credits = 0 };

	public Graph Graph;

	[Signal]
	public delegate void ServerInfoUpdateEventHandler();
	[Signal]
	public delegate void AgentInfoUpdateEventHandler();
	[Signal]
	public delegate void ShipUpdateEventHandler(string name);
	[Signal]
	public delegate void SystemUpdateEventHandler(string name);

	public override void _Ready()
	{
		Instance = this;

		Graph = new Graph(
			(string systemName) => EmitSignal(SignalName.SystemUpdate, systemName),
			(string shipName) => EmitSignal(SignalName.ShipUpdate, shipName)
		);

		CallDeferred(MethodName.ConnectMQTT);
	}

	private async void ConnectMQTT()
	{
		// TODO: connection error handling
		_mqttFactory = new MqttFactory();
		_mqtt = _mqttFactory.CreateMqttClient();

		var options = new MqttClientOptionsBuilder()
			.WithClientId("SpacetraderClient")
			.WithWebSocketServer(o => o.WithUri("localhost:55555")) // TODO: configure from UI
			.WithCleanStart()
			.WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
			.Build();
		await _mqtt.ConnectAsync(options, CancellationToken.None);
		GD.Print("MQTT connected");

		await Subscribe<GrpcSpacetrader.System>("systems/+", 0, t => t.StartsWith("systems/") && t.Count("/") == 1, OnSystemMsg);
		await Subscribe<GrpcSpacetrader.Ship>("ships/+", 0, t => t.StartsWith("ships/") && t.Count("/") == 1, OnShipMsg);
		await Subscribe<GrpcSpacetrader.Agent>("agent", 0, t => t == "agent", OnAgentMsg);
		await Subscribe<GrpcSpacetrader.ServerStatus>("server", 0, t => t == "server", OnServerMsg);
	}

	private async Task Subscribe<T>(string topicFilter, MQTTnet.Protocol.MqttQualityOfServiceLevel qos, Predicate<string> topicMatcher, Action<T> callback) where T : IMessage
	{
		_mqtt.ApplicationMessageReceivedAsync += ea =>
		{
			// match topic
			if (!topicMatcher.Invoke(ea.ApplicationMessage.Topic))
			{
				return Task.CompletedTask;
			}
			//GD.Print("received topic " + ea.ApplicationMessage.Topic);

			// create target Protobuf message instance
			var deserialized = (T)Activator.CreateInstance(typeof(T));
			// deserialize from payload
			deserialized.MergeFrom(ea.ApplicationMessage.PayloadSegment);
			// invoke callback with deserialized proto
			// we do it deferred since the MQTT is on a separate thread and will thus require deferring
			Callable.From(() =>
			{
				callback.Invoke(deserialized);
			}).CallDeferred();

			return Task.CompletedTask;
		};

		var filter = _mqttFactory
			.CreateTopicFilterBuilder()
			.WithQualityOfServiceLevel(qos)
			.WithRetainAsPublished()
			.WithTopic(topicFilter)
			.Build();
		var mqttSubscribeOptions = _mqttFactory
			.CreateSubscribeOptionsBuilder()
			.WithTopicFilter(filter)
			.Build();
		var response = await _mqtt.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
		foreach (var item in response.Items)
		{
			if (item.ResultCode != (MqttClientSubscribeResultCode)qos)
			{
				throw new Exception("could not subscribe to " + item.TopicFilter.Topic + ": " + item.ResultCode);
			}
			GD.Print("subscribed to " + item.TopicFilter.Topic);
		}
	}

	public override void _ExitTree()
	{
		_mqtt.DisconnectAsync();
		base._ExitTree();
	}


	private void OnServerMsg(GrpcSpacetrader.ServerStatus server)
	{
		ServerStatus = new()
		{
			Version = server.Version,
			NextReset = server.NextReset.ToDateTime()
		};
		EmitSignal(SignalName.ServerInfoUpdate);
	}

	private void OnAgentMsg(GrpcSpacetrader.Agent agent)
	{
		Agent = new()
		{
			Name = agent.Name,
			Credits = agent.Credits
		};
		EmitSignal(SignalName.AgentInfoUpdate);
	}

	private void OnShipMsg(GrpcSpacetrader.Ship ship)
	{
		Graph.UpdateShip(new ShipModel
		{
			Name = ship.Id,
			Status = ship.Status,
			SystemName = ship.CurrentLocation.System.Id,
			WaypointName = ship.CurrentLocation.Waypoint.Id
		});
	}

	private void OnSystemMsg(GrpcSpacetrader.System system)
	{
		Graph.UpdateSystem(new SystemModel
		{
			Name = system.Id,
			Pos = new Vector2(system.X, system.Y),
			HasJumpgates = true, // TODO: include jumpgate info in system proto
		});
	}
}

public class Graph
{
	private readonly Action<string> onSystemUpdated;
	private readonly Action<string> onShipUpdated;

	public Graph(Action<string> onSystemUpdated, Action<string> onShipUpdated)
	{
		this.onSystemUpdated = onSystemUpdated;
		this.onShipUpdated = onShipUpdated;
	}

	public class StoredSystem
	{
		public SystemModel Model { get; internal set; }
		private readonly Graph graph;
		internal HashSet<string> shipNames = new();

		public IReadOnlyList<ShipModel> Ships
		{
			get
			{
				return graph.ships
					.Where((kv) => shipNames.Contains(kv.Key))
					.Select((kv) => kv.Value)
					.ToList();
			}
		}

		public StoredSystem(Graph parent, SystemModel model)
		{
			graph = parent;
			Model = model;
		}
	}

	private readonly ConcurrentDictionary<string, StoredSystem> systems = new();

	private readonly ConcurrentDictionary<string, ShipModel> ships = new();

	private readonly ConcurrentDictionary<string, ShipModel> shipsWithoutSystem = new();

	public IReadOnlyList<StoredSystem> GetAllSystems()
	{
		return systems.Values.ToList();
	}

	public IReadOnlyList<ShipModel> GetAllShips()
	{
		return ships.Values.ToList();
	}

	public StoredSystem GetSystem(string name)
	{
		return systems[name];
	}

	public ShipModel GetShip(string name)
	{
		return ships[name];
	}

	public void UpdateSystem(SystemModel system)
	{
		var key = system.Name;
		// check if system already exists
		if (systems.TryGetValue(key, out StoredSystem storedSystem))
		{
			// update model data
			storedSystem.Model = system;
			onSystemUpdated(key);
			return;
		}
		// create new
		systems[key] = new StoredSystem(this, system);
		onSystemUpdated(key);
		CheckOrphanedShips((kv) => system.Name == kv.Value.SystemName);
	}

	public void UpdateShip(ShipModel ship)
	{
		var key = ship.Name;
		// check if ship already exists
		if (ships.TryGetValue(key, out ShipModel oldShip))
		{
			// remove ship name from old system
			RemoveShipRef(oldShip.Name, oldShip.SystemName);
		}
		// add ship name to new system
		var valid = AddShipRef(ship);
		if (valid)
		{
			// save ship
			ships[key] = ship;
			onShipUpdated(key);
		}
	}

	private void RemoveShipRef(string shipName, string systemName)
	{
		// remove ship references from system if it exists
		if (systems.TryGetValue(systemName, out StoredSystem storedSystem))
		{
			storedSystem.shipNames.Remove(shipName);
			onSystemUpdated(systemName);
		}
	}

	private bool AddShipRef(ShipModel ship)
	{
		if (systems.TryGetValue(ship.SystemName, out StoredSystem storedSystem))
		{
			storedSystem.shipNames.Add(ship.Name);
			onSystemUpdated(storedSystem.Model.Name);
			return true;
		}
		shipsWithoutSystem[ship.Name] = ship;
		return false;
	}

	private void CheckOrphanedShips(Func<KeyValuePair<string, ShipModel>, bool> filter)
	{
		foreach (var ship in shipsWithoutSystem.Where(filter))
		{
			var systemName = ship.Value.SystemName;
			if (systems.ContainsKey(systemName))
			{
				UpdateShip(ship.Value);
				shipsWithoutSystem.Remove(ship.Key, out _);
				GD.Print("unorphaned: " + ship.Key);
			}
		}
	}
}
