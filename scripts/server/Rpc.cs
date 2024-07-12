using Godot;
using System;

using System.Threading.Tasks;

using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

// using Kiota for client generation from OpenAPI.
// see https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet
using SpaceTradersApi;
using SpaceTradersApi.Client;
using SpaceTradersApi.Client.Models;
using Godot.NativeInterop;

namespace Server;

public partial class Rpc : Node
{
	const int PORT = 55555;
	const int MAX_CLIENT = 1;

	private readonly SpaceTradersClient _client;

	public Rpc()
	{
		var authProvider = new AnonymousAuthenticationProvider();
		var adapter = new HttpClientRequestAdapter(authProvider);
		_client = new SpaceTradersClient(adapter);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var mpPeer = new ENetMultiplayerPeer();
		mpPeer.CreateServer(PORT, MAX_CLIENT);
		Multiplayer.PeerConnected += OnClientConnected;
		Multiplayer.MultiplayerPeer = mpPeer;
		GD.Print("server started on port " + PORT);
	}

	private void OnClientConnected(long id)
	{
		GD.Print("client connected: " + id);

		var spawner = GetNode<MultiplayerSpawner>("MultiplayerSpawner");
		spawner.SpawnFunction = Callable.From((string s) =>
		{
			GD.Print("SpawnFunction Server: " + s);
			var rect = new ColorRect();
			rect.Color = new Color("red");
			return rect;
		});

		_ = spawner.Spawn("foo");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void TestFunction(byte[] data)
	{
		GD.Print("TestFunction");
		var unpacked = GD.BytesToVarWithObjects(data);
		GD.Print(unpacked.VariantType);
		var obj = unpacked.As<Godot.Collections.Dictionary>();
		GD.Print(obj.ToString());
	}


	/// <summary>
	/// Client -> Server.
	/// Requests a complete sync from the server, used when initially connected.
	/// </summary>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void RequestSync()
	{
		GD.Print("initial sync requested");
		_ = PerformSync();
	}

	private delegate Task SyncTaskFunc();
	sealed private record SyncTask(string Name, SyncTaskFunc Fn);

	/// <summary>
	/// Queries all required data from the API and sends it to the client via RPC.
	/// </summary>
	private async Task PerformSync()
	{
		GD.Print("sync start");

		SyncTask[] tasks = {
			new(
				"Querying Server Status",
				async () => {
					var serverStatus = await _client.GetAsync();
					Rpc(nameof(SetServerStatus), serverStatus.Version, serverStatus.ServerResets.Next);
				}
			),
			new(
				"Foo",
				async () => {
					await Task.Delay(1000);
				}
			),
			new(
				"Bar",
				async () => {
					await Task.Delay(1000);
				}
			)
		};

		for (int i = 0; i < tasks.Length; i++)
		{
			Rpc(nameof(SetSyncProgress), (float)i / tasks.Length, tasks[i].Name);
			await tasks[i].Fn();
		}

		GD.Print("sync finished");
		Rpc(nameof(SyncComplete));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SetSyncProgress(float progress, string desc)
	{
		throw new NotImplementedException();
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SyncComplete()
	{
		throw new NotImplementedException();
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SetServerStatus(string version, string nextReset)
	{
		throw new NotImplementedException();
	}
}
