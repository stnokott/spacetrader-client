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

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	public Data Data = new();

	private GraphQLHttpClient graphQLClient;

	public override void _Ready()
	{
		Instance = this;

		graphQLClient = new GraphQLHttpClient(
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
		graphQLClient.Dispose();
		base._ExitTree();
	}


	[Signal]
	public delegate void ServerInfoUpdateEventHandler();

	private static readonly GraphQLQuery serverQuery = new(
		new GraphQLModels.QueryQueryBuilder()
			.WithServer(new GraphQLModels.ServerQueryBuilder()
				.WithVersion()
				.WithNextReset()
			)
			.Build(GraphQLModels.Formatting.None)
	);

	public async Task QueryServer(IProgress<float> _)
	{
		var resp = await graphQLClient.SendQueryAsync<GraphQLModels.ServerResponse>(serverQuery);
		var server = resp.Data.Server; // TODO: check errors

		Data.ServerStatus = new()
		{
			Version = server.Version,
			NextReset = server.NextReset
		};
		EmitSignal(SignalName.ServerInfoUpdate);
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler();

	private static readonly GraphQLQuery agentQuery = new(
		new GraphQLModels.QueryQueryBuilder()
			.WithAgent(new GraphQLModels.AgentQueryBuilder()
				.WithName()
				.WithCredits()
			)
			.Build(GraphQLModels.Formatting.None)
	);

	public async Task QueryAgent(IProgress<float> progress)
	{
		var resp = await graphQLClient.SendQueryAsync<GraphQLModels.AgentResponse>(agentQuery);
		var agent = resp.Data.Agent; // TODO: check errors

		Data.Agent = new()
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
			.Build(GraphQLModels.Formatting.None);
		return new GraphQLQuery(q);
	}

	private const int SYSTEMS_PER_PAGE = 100;

	public async Task QuerySystems(IProgress<float> progress)
	{
		var hasMorePages = true;
		var nextCursor = "";

		var n = 0;
		while (hasMorePages)
		{
			var query = MakePaginatedSystemsQuery(nextCursor, SYSTEMS_PER_PAGE);

			var resp = await graphQLClient.SendQueryAsync<GraphQLModels.SystemsResponse>(query);

			var data = resp.Data.Systems;
			// TODO: check errors
			foreach (var edge in data.Edges)
			{
				var system = edge.Node;
				var key = system.Name;

				Data.systems[key] = new SystemModel
				{
					Name = system.Name,
					Pos = new Vector2I(system!.X!.Value, system!.Y!.Value),
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

	[Signal]
	public delegate void ShipUpdateEventHandler(string name);
	[Signal]
	public delegate void ShipMovedFromSystemEventHandler(string ship, string system);
	[Signal]
	public delegate void ShipMovedToSystemEventHandler(string ship, string system);

	private static readonly GraphQLQuery shipsQuery = new(
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
		var resp = await graphQLClient.SendQueryAsync<GraphQLModels.ShipsResponse>(shipsQuery);
		// TODO: check errors

		for (var i = 0; i < resp.Data.Ships.Count; i++)
		{
			var ship = resp.Data.Ships[i];
			var key = ship.Name;
			var shipExists = Data.ships.TryGetValue(key, out ShipModel oldShip);

			var newShip = new ShipModel
			{
				Name = ship.Name,
				Status = (GraphQLModels.ShipStatus)ship!.Status!,
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
	public ReadOnlyDictionary<string, SystemModel> Systems
	{
		get
		{
			return new ReadOnlyDictionary<string, SystemModel>(systems);
		}
	}

	internal readonly ConcurrentDictionary<string, ShipModel> ships = new();
	public ReadOnlyDictionary<string, ShipModel> Ships
	{
		get
		{
			return new ReadOnlyDictionary<string, ShipModel>(ships);
		}
	}
}
