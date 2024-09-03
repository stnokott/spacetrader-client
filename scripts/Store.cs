using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

using Models;


public partial class Store : Node
{
	public static Store Instance { get; private set; }

	private Rpc.Client _grpc;
	public Dictionary<string, System> Systems = new();

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
				Status = ship.Status.ToString(), // TODO: enum?
				Pos = new Vector2(ship.CurrentLocation.System.X, ship.CurrentLocation.System.Y)
			};
		});
		EmitSignal(SignalName.FleetUpdate, new Godot.Collections.Array<InternalShip>(internalShips));
	}

	[Signal]
	public delegate void SystemsUpdatedEventHandler();

	public async Task UpdateSystems()
	{
		var stream = _grpc.GetAllSystems(new Empty());

		Systems.Clear();
		while (await stream.ResponseStream.MoveNext())
		{
			var system = stream.ResponseStream.Current;
			Systems.Add(system.Name, new System
			{
				Pos = new Vector2(system.Pos.X, system.Pos.Y),
				ShipCount = system.ShipCount,
				HasJumpgates = system.HasJumpgates
			});
		}

		EmitSignal(SignalName.SystemsUpdated);
	}

	public sealed record System
	{
		public required Vector2 Pos;
		public required int ShipCount;
		public bool HasJumpgates;
	}
}
