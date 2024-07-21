using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcSpacetrader;

using Models;

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	private Rpc.Client _grpc;
	public List<Ship> Ships { get; set; } = new List<Ship>();

	public override void _Ready()
	{
		Instance = this;
		_grpc = new Rpc.Client("localhost", 55555); // TODO: configure from UI
																								// TODO: connection error handling
	}

	public override void _ExitTree()
	{
		_grpc.Dispose();
		base._ExitTree();
	}

	[Signal]
	public delegate void ServerInfoUpdateEventHandler(InternalServerStatus status);

	public async Task UpdateServerStatus()
	{
		var serverStatus = await _grpc.GetServerStatusAsync(new Empty());
		EmitSignal(SignalName.ServerInfoUpdate, new InternalServerStatus
		{
			Version = serverStatus.Version,
			NextReset = serverStatus.NextReset.ToString() // TODO: check if DateTime can be used
		});
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler(InternalAgentInfo agent);

	public async Task UpdateAgentInfo()
	{
		var agentInfo = await _grpc.GetCurrentAgentAsync(new Empty());
		EmitSignal(SignalName.AgentInfoUpdate, new InternalAgentInfo
		{
			Name = agentInfo.Name,
			Credits = agentInfo.Credits
		});
	}

	[Signal]
	public delegate void FleetUpdateEventHandler(Godot.Collections.Array<InternalShip> ships);

	public async Task UpdateShips()
	{
		var ships = await _grpc.GetFleetAsync(new Empty());
		var internalShips = ships.Ships.Select((ship) =>
		{
			return new InternalShip
			{
				Name = ship.Name,
				Status = ship.Status.ToString() // TODO: enum?
			};
		});
		EmitSignal(SignalName.FleetUpdate, new Godot.Collections.Array<InternalShip>(internalShips));
	}

	public async Task<List<GrpcSpacetrader.System>> GetSystemsInRect(Vector2I start, Vector2I end)
	{
		var systems = _grpc.GetSystemsInRect(new Rect
		{
			Start = new Vector { X = start.X, Y = start.Y },
			End = new Vector { X = end.X, Y = end.Y }
		});

		var result = new List<GrpcSpacetrader.System>();
		while (await systems.ResponseStream.MoveNext())
		{
			result.Add(systems.ResponseStream.Current);
		}
		return result;
	}
}
