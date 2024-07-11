using Godot;
using System;

using Api;

namespace Rpc;

public partial class Rpc : Node
{
	const int PORT = 55555;

	private readonly Api.Client client = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var mpPeer = new ENetMultiplayerPeer();
		mpPeer.CreateServer(PORT);
		Multiplayer.PeerConnected += OnClientConnected;
		Multiplayer.MultiplayerPeer = mpPeer;
		GD.Print("server started on port " + PORT);
	}

	private async void OnClientConnected(long id)
	{
		GD.Print("client connected: " + id);

		var serverStatus = await client.GetServerStatus();
		RpcId(id, nameof(SetServerInfo), serverStatus.Version, serverStatus.NextResetTs);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SetServerInfo(string version, string nextReset)
	{
		throw new NotSupportedException();
	}
}
