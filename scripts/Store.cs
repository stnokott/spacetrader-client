using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQLModels;
using Models;
// ReSharper disable RedundantArgumentDefaultValue

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	public readonly Data Data = new();

	private GraphQLHttpClient _graphQlClient;
	private IDisposable? _systemSubscription;

	public override void _Ready()
	{
		Instance = this;

		_graphQlClient = new GraphQLHttpClient(
			new GraphQLHttpClientOptions
			{
				EndPoint = new Uri("http://localhost:55555/graphql"),
				EnableAutomaticPersistedQueries = (_) => true,
				UseWebSocketForQueriesAndMutations = true
			},
			new NewtonsoftJsonSerializer()
		);
	}

	public override void _ExitTree()
	{
		_systemSubscription?.Dispose();
		_graphQlClient.Dispose();
		base._ExitTree();
	}

	[Signal]
	public delegate void ServerInfoUpdateEventHandler();

	private static readonly GraphQLQuery ServerQuery = new(
		new GraphQLModels.QueryQueryBuilder()
			.WithServer(new GraphQLModels.ServerQueryBuilder()
				.WithVersion()
				.WithNextReset()
			)
			.Build(GraphQLModels.Formatting.None)
	);

	public async Task QueryServer(IProgress<float> _)
	{
		var resp = await _graphQlClient.SendQueryAsync<GraphQLModels.ServerResponse>(ServerQuery);
		var server = resp.Data.Server; // TODO: check errors

		Data.ServerStatus = new ServerStatusModel
		{
			Version = server.Version,
			NextReset = server.NextReset
		};
		EmitSignal(SignalName.ServerInfoUpdate);
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler();

	private static readonly GraphQLQuery AgentQuery = new(
		new GraphQLModels.QueryQueryBuilder()
			.WithAgent(new GraphQLModels.AgentQueryBuilder()
				.WithName()
				.WithCredits()
			)
			.Build(GraphQLModels.Formatting.None)
	);

	public async Task QueryAgent(IProgress<float> progress)
	{
		var resp = await _graphQlClient.SendQueryAsync<GraphQLModels.AgentResponse>(AgentQuery);
		var agent = resp.Data.Agent; // TODO: check errors

		Data.Agent = new AgentInfoModel
		{
			Name = agent.Name,
			Credits = agent.Credits
		};
		EmitSignal(SignalName.AgentInfoUpdate);
	}

	[Signal]
	public delegate void SystemUpdateEventHandler(string name);

	private static readonly GraphQLQuery SystemCountQuery = new(
		new QueryQueryBuilder().WithSystemCount().Build(GraphQLModels.Formatting.None)
	);
	
	private static readonly GraphQLRequest SubscribeSystemsRequest = new(
		new GraphQLModels.SubscriptionQueryBuilder()
			.WithSystem(
				new SystemQueryBuilder()
					.WithName()
					.WithX()
					.WithY()
					.WithWaypoints(new WaypointQueryBuilder()
						.WithConnectedTo(
							new WaypointQueryBuilder()
								.WithName()
						)
					)
			)
			.Build(GraphQLModels.Formatting.None)
	);

	public async Task QuerySystems(IProgress<float> progress)
	{
		var systemCountResp = await _graphQlClient.SendQueryAsync<SystemCountResponse>(SystemCountQuery);
		// TODO: handle errors
		var systemCount = systemCountResp.Data.SystemCount;
		
		IObservable<GraphQLResponse<GraphQLModels.SystemSubscriptionResponse>> subscriptionStream = _graphQlClient.CreateSubscriptionStream<GraphQLModels.SystemSubscriptionResponse>(
			SubscribeSystemsRequest,
			exception =>
			{
				GD.PrintErr("Subscription Error: " + exception.Message);
			}
		);

		var i = 0;
		var completionSource = new TaskCompletionSource();
		
		_systemSubscription = subscriptionStream.Subscribe(
			response =>
			{
				var system = response.Data.System;
				var key = system.Name;

				Data.systems[key] = new SystemModel
				{
					Name = system.Name,
					Pos = new Vector2I(system.X!.Value, system.Y!.Value),
					HasJumpgates = system.Waypoints.Any(wp => wp.ConnectedTo.Count > 0)
				};
				CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.SystemUpdate, key);
				progress.Report(i / (float)systemCount);
				i++;
				if (i == systemCount) completionSource.SetResult(); // complete underlying task
			},
			exception =>
			{
				GD.PrintErr("Subscription Error: " + exception.Message);
			}
		);

		await completionSource.Task;
	}

	[Signal]
	public delegate void ShipUpdateEventHandler(string name);
	[Signal]
	public delegate void ShipMovedFromSystemEventHandler(string ship, string system);
	[Signal]
	public delegate void ShipMovedToSystemEventHandler(string ship, string system);

	private static readonly GraphQLQuery ShipsQuery = new(
		new GraphQLModels.QueryQueryBuilder()
		.WithShips(new GraphQLModels.ShipQueryBuilder()
			.WithName()
			.WithStatus()
			.WithSystem(new GraphQLModels.SystemQueryBuilder().WithName())
			.WithWaypoint(new GraphQLModels.WaypointQueryBuilder().WithName())
		)
		.Build(GraphQLModels.Formatting.None)
	);

	public async Task QueryShips(IProgress<float> progress)
	{
		var resp = await _graphQlClient.SendQueryAsync<GraphQLModels.ShipsResponse>(ShipsQuery);
		// TODO: check errors

		for (var i = 0; i < resp.Data.Ships.Count; i++)
		{
			var ship = resp.Data.Ships[i];
			var key = ship.Name;
			var shipExists = Data.ships.TryGetValue(key, out ShipModel oldShip);

			var newShip = new ShipModel
			{
				Name = ship.Name,
				Status = (GraphQLModels.ShipStatus)ship.Status!,
				SystemName = ship.System.Name,
				WaypointName = ship.Waypoint.Name
			};
			Data.ships[key] = newShip;
			EmitSignal(SignalName.ShipUpdate, key);

			// check ship movement between systems
			if (shipExists && oldShip.SystemName != newShip.SystemName)
			{
				//EmitSignal(SignalName.ShipMovedFromSystem, oldShip.Name, oldShip.Name);
				//EmitSignal(SignalName.ShipMovedToSystem, newShip.Name, newShip.SystemName);
			}
			if (!shipExists)
			{
				//EmitSignal(SignalName.ShipMovedToSystem, newShip.Name, newShip.SystemName);
			}
			progress.Report((float)(i + 1) / resp.Data.Ships.Count);
		}
	}

	public List<ShipModel> ShipsInSystem(string system)
	{
		return Data.ships.Where((kv) => kv.Value.SystemName == system).Select((kv) => kv.Value).ToList();
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()


public class Data
{
	public ServerStatusModel ServerStatus = new() { Version = "v0.0.0", NextReset = DateTime.MaxValue };
	public AgentInfoModel Agent = new() { Name = "!AGENTNAME", Credits = 0 };

	internal readonly ConcurrentDictionary<string, SystemModel> systems = new();
	public ReadOnlyDictionary<string, SystemModel> Systems => new(systems);

	internal readonly ConcurrentDictionary<string, ShipModel> ships = new();
	public ReadOnlyDictionary<string, ShipModel> Ships => new(ships);
}
