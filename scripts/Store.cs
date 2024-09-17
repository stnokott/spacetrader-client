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

	// use in-memory graph for saving systems and ships and waypoints...
	public ConcurrentDictionary<string, SystemModel> Systems = new();
	public ConcurrentDictionary<string, ShipModel> Ships = new();

	public override void _Ready()
	{
		Instance = this;

		ConnectMQTT();
	}

	private async void ConnectMQTT()
	{
		// TODO: connection error handling
		_mqttFactory = new MqttFactory();
		_mqtt = _mqttFactory.CreateMqttClient();

		var options = new MqttClientOptionsBuilder()
			.WithClientId("SpacetraderClient")
			.WithWebSocketServer(o => o.WithUri("localhost:55555")) // TODO: configure from UI
			.WithCleanSession()
			.Build();
		await _mqtt.ConnectAsync(options, CancellationToken.None);
		GD.Print("MQTT connected");

		await Subscribe<GrpcSpacetrader.Agent>("agent", t => t == "agent", OnAgentMsg);
		await Subscribe<GrpcSpacetrader.ServerStatus>("server", t => t == "server", OnServerMsg);
		await Subscribe<GrpcSpacetrader.Ship>("ships/+", t => t.StartsWith("ships/") && t.Count("/") == 1, OnShipMsg);
		await Subscribe<GrpcSpacetrader.System>("systems/+", t => t.StartsWith("systems/") && t.Count("/") == 1, OnSystemMsg);
	}

	private async Task Subscribe<T>(string topicFilter, Predicate<string> topicMatcher, Action<T> callback) where T : IMessage
	{
		_mqtt.ApplicationMessageReceivedAsync += ea =>
		{
			if (!topicMatcher.Invoke(ea.ApplicationMessage.Topic))
			{
				return Task.CompletedTask;
			}

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
			.WithExactlyOnceQoS()
			.WithTopic(topicFilter)
			.Build();
		var mqttSubscribeOptions = _mqttFactory
			.CreateSubscribeOptionsBuilder()
			.WithTopicFilter(filter)
			.Build();
		var response = await _mqtt.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
		GD.Print("subscribed to " + topicFilter + ": " + response.ReasonString);
	}

	public override void _ExitTree()
	{
		_mqtt.DisconnectAsync();
		base._ExitTree();
	}

	[Signal]
	public delegate void ServerInfoUpdateEventHandler();


	private void OnServerMsg(GrpcSpacetrader.ServerStatus server)
	{
		ServerStatus = new()
		{
			Version = server.Version,
			NextReset = server.NextReset.ToDateTime()
		};
		EmitSignal(SignalName.ServerInfoUpdate);
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler();

	private void OnAgentMsg(GrpcSpacetrader.Agent agent)
	{
		Agent = new()
		{
			Name = agent.Name,
			Credits = agent.Credits
		};
		EmitSignal(SignalName.AgentInfoUpdate);
	}

	[Signal]
	public delegate void ShipUpdateEventHandler(string name);

	private void OnShipMsg(GrpcSpacetrader.Ship ship)
	{
		Ships[ship.Id] = new ShipModel
		{
			Name = ship.Id,
			Status = ship.Status,
			SystemName = ship.CurrentLocation.System.Id,
			WaypointName = ship.CurrentLocation.Waypoint.Id
		}; ;
		EmitSignal(SignalName.ShipUpdate, ship.Id);
	}

	[Signal]
	public delegate void SystemUpdateEventHandler(string name);

	private void OnSystemMsg(GrpcSpacetrader.System system)
	{
		Systems[system.Id] = new SystemModel
		{
			Pos = new Vector2(system.X, system.Y),
			HasJumpgates = true, // TODO: include jumpgate info in system proto
		};
		EmitSignal(SignalName.SystemUpdate, system.Id);
	}
}
