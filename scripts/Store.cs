using System;
using Godot;
using Godot.Collections;

using Models;

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	// TODO: check if directive can be used https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_features.html#preprocessor-defines

	// TODO: check if we actually need a copy of each of these on the SERVER, otherwise we can stop assigning values in the setters
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
		ServerInfo = serverInfo;
		Rpc(nameof(RpcSetServerStatus), Serialize.ToBytes(serverInfo));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcSetServerStatus(byte[] data)
	{
		var unpacked = Serialize.FromBytes<ServerStatusResource>(data);
		ServerInfo = unpacked;
		EmitSignal(SignalName.ServerInfoUpdate, unpacked);
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler(AgentInfoResource agent);

	public void SetAgentInfo(AgentInfoResource agent)
	{
		AgentInfo = agent;
		Rpc(nameof(RpcSetAgentInfo), Serialize.ToBytes(agent));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcSetAgentInfo(byte[] data)
	{
		var unpacked = Serialize.FromBytes<AgentInfoResource>(data);
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
		Ships.Add(ship);
		Rpc(nameof(RpcAddShip), Serialize.ToBytes(ship));
	}


	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcAddShip(byte[] data)
	{
		Ships.Add(Serialize.FromBytes<ShipResource>(data));
		EmitSignal(SignalName.ShipUpdate);
	}
}
