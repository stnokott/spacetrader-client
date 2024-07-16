using Godot;
using Godot.Collections;

using Models;

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	// TODO: check if we actually need a copy of each of these on the SERVER, otherwise we can stop calling the RPC methods on the server too (CallLocal=false)
	private ServerStatusResource ServerInfo = new();
	private AgentInfoResource AgentInfo = new();
	private Array<ShipResource> Ships { get; set; } = new Array<ShipResource>();

	public override void _Ready()
	{
		Instance = this;
	}

	[Signal]
	public delegate void ServerInfoUpdateEventHandler(ServerStatusResource status);

	public void SetServerStatus(ServerStatusResource serverInfo)
	{
		Rpc(nameof(RpcSetServerStatus), serverInfo.ToBytes());
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcSetServerStatus(byte[] data)
	{
		var unpacked = ServerStatusResource.FromBytes(data);
		ServerInfo = unpacked;
		EmitSignal(SignalName.ServerInfoUpdate, unpacked);
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler(AgentInfoResource agent);

	public void SetAgentInfo(AgentInfoResource agent)
	{
		Rpc(nameof(RpcSetAgentInfo), agent.ToBytes());
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcSetAgentInfo(byte[] data)
	{
		var unpacked = AgentInfoResource.FromBytes(data);
		AgentInfo = unpacked;
		EmitSignal(SignalName.AgentInfoUpdate, unpacked);
	}

	[Signal]
	public delegate void ShipUpdateEventHandler();

	public void ClearShips()
	{
		Rpc(nameof(RpcClearShips));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcClearShips()
	{
		Ships.Clear();
		EmitSignal(SignalName.ShipUpdate);
	}

	public void AddShip(ShipResource ship)
	{
		Rpc(nameof(RpcAddShip), ship.ToBytes());
	}


	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcAddShip(byte[] data)
	{
		Ships.Add(ShipResource.FromBytes(data));
		EmitSignal(SignalName.ShipUpdate);
	}
}
