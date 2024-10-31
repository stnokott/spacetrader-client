using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Models;
// ReSharper disable RedundantArgumentDefaultValue

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	public readonly Data Data = new();

	private GraphQLHttpClient _graphQlClient;

	public override void _Ready()
	{
		Instance = this;

		_graphQlClient = new GraphQLHttpClient(
			new GraphQLHttpClientOptions
			{
				EndPoint = new Uri("http://localhost:55555/graphql"),
				EnableAutomaticPersistedQueries = (_) => true
			},
			new NewtonsoftJsonSerializer()
		);
	}

	public override void _ExitTree()
	{
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

	private static GraphQLQuery MakePaginatedSystemsQuery(string afterCursor, int perPage)
	{
		var param = new GraphQLModels.GraphQlQueryParameter<GraphQLModels.PageArgs>(
			"page",
			nameof(GraphQLModels.PageArgs),
			new GraphQLModels.PageArgs
			{
				After = afterCursor,
				First = perPage
			}
		);

		var q = new GraphQLModels.QueryQueryBuilder()
			.WithSystems(new GraphQLModels.SystemConnectionQueryBuilder()
				.WithPageInfo(new GraphQLModels.PageInfoQueryBuilder()
					.WithHasNextPage()
					.WithTotalCount()
				)
				.WithEdges(new GraphQLModels.SystemEdgeQueryBuilder()
					.WithCursor()
					.WithNode(new GraphQLModels.SystemQueryBuilder()
						.WithName()
						.WithX()
						.WithY()
						.WithHasJumpgates()
					)
				),
				param)
			.WithParameter(param)
		;
		return new GraphQLQuery(q.Build(GraphQLModels.Formatting.None));
	}

	private const int SystemsPerPage = 100;

	public async Task QuerySystems(IProgress<float> progress)
	{
		var hasMorePages = true;
		var nextCursor = "";

		var n = 0;
		while (hasMorePages)
		{
			var query = MakePaginatedSystemsQuery(nextCursor, SystemsPerPage);

			var resp = await _graphQlClient.SendQueryAsync<GraphQLModels.SystemsResponse>(query);

			var data = resp.Data.Systems;
			// TODO: check errors
			foreach (var edge in data.Edges)
			{
				var system = edge.Node;
				var key = system.Name;

				Data.systems[key] = new SystemModel
				{
					Name = system.Name,
					Pos = new Vector2I(system.X!.Value, system.Y!.Value),
					HasJumpgates = system.HasJumpgates!.Value
				};
				EmitSignal(SignalName.SystemUpdate, key);
				n++;
			}

			hasMorePages = data.PageInfo.HasNextPage!.Value;
			nextCursor = data.Edges.Last().Cursor;
			progress.Report((float)n / data.PageInfo.TotalCount!.Value);
		}
	}

	private static GraphQLQuery MakeSystemQuery(string id)
	{
		var builder = new GraphQLModels.QueryQueryBuilder()
			.WithSystem(new GraphQLModels.SystemQueryBuilder()
				.WithWaypoints(new GraphQLModels.WaypointQueryBuilder()
					.WithConnectedTo(new GraphQLModels.WaypointQueryBuilder()
						.WithSystem(new GraphQLModels.SystemQueryBuilder()
							.WithName()
						)
					)
				), id
			)
		;
		return new GraphQLQuery(builder.Build(GraphQLModels.Formatting.None));
	}

	public async Task<DetailedSystemModel> QuerySystem(string id)
	{
		var q = MakeSystemQuery(id);
		var resp = await _graphQlClient.SendQueryAsync<GraphQLModels.SingleSystemResponse>(q);
		// TODO: error handling

		var connectedSystems = resp.Data.System.Waypoints.SelectMany((wp) =>
		{
			return wp.ConnectedTo.Select((wpConnected) => wpConnected.System.Name);
		});
		return new DetailedSystemModel
		{
			connectedSystems = connectedSystems.ToHashSet()
		};
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
				EmitSignal(SignalName.ShipMovedFromSystem, oldShip.Name, oldShip.Name);
				EmitSignal(SignalName.ShipMovedToSystem, newShip.Name, newShip.SystemName);
			}
			if (!shipExists)
			{
				EmitSignal(SignalName.ShipMovedToSystem, newShip.Name, newShip.SystemName);
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
