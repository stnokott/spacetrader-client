using Godot;
using Godot.Collections;

public partial class Store : Node
{
	public static Store Instance { get; private set; }

	private string ServerVersion { get; set; } = "unknown";
	private string ServerNextReset { get; set; } = "unknown";
	private string AgentName { get; set; } = "unknown";
	private long Credits { get; set; } = 0;
	private Array<ShipResource> Ships { get; set; } = new Array<ShipResource>();

	public override void _Ready()
	{
		Instance = this;
	}

	[Signal]
	public delegate void ServerInfoUpdateEventHandler(string serverVersion, string serverNextReset);

	public void SetServerStatus(string serverVersion, string serverNextReset)
	{
		Rpc(nameof(RpcSetServerStatus), serverVersion, serverNextReset);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcSetServerStatus(string serverVersion, string serverNextReset)
	{
		ServerVersion = serverVersion;
		ServerNextReset = serverNextReset;
		EmitSignal(SignalName.ServerInfoUpdate, serverVersion, serverNextReset);
	}


	[Signal]
	public delegate void AgentInfoUpdateEventHandler(string serverVersion, string serverNextReset);

	public void SetAgentInfo(string agentName, long credits)
	{
		Rpc(nameof(RpcSetAgentInfo), agentName, credits);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void RpcSetAgentInfo(string agentName, long credits)
	{
		AgentName = agentName;
		Credits = credits;
		EmitSignal(SignalName.AgentInfoUpdate, agentName, credits);
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
