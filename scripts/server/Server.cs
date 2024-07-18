using Godot;
using System;

using System.Threading.Tasks;

using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

// using Kiota for client generation from OpenAPI.
// see https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet
using SpaceTradersApi.Client;
using System.Collections.Generic;
using System.Threading;

using Models;

namespace Server;

public partial class Server : Node
{
	const int PORT = 55555;
	const int MAX_CLIENT = 1;

	private readonly SpaceTradersClient _client;
	private readonly Store store = Store.Instance;

	internal class TokenProvider : IAccessTokenProvider
	{
		public AllowedHostsValidator AllowedHostsValidator { get; }

		public Task<string> GetAuthorizationTokenAsync(
		Uri uri,
		Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = default)
		{
			return Task.FromResult(DotEnv.Get("API_TOKEN"));
		}
	}

	public Server()
	{
		var authProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider());
#if TOOLS
		GD.Print("running in editor, using mock server");
		var adapter = ServerMock.NewAdapter();
#else
		var adapter = new HttpClientRequestAdapter(authProvider);v
#endif
		_client = new SpaceTradersClient(adapter);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var mpPeer = new ENetMultiplayerPeer();
		mpPeer.CreateServer(PORT, MAX_CLIENT);
		Multiplayer.PeerConnected += OnClientConnected;
		Multiplayer.PeerDisconnected += OnClientDisconnected;
		Multiplayer.MultiplayerPeer = mpPeer;
		GD.Print("server started on port " + PORT);
	}

	private void OnClientConnected(long id)
	{
		GD.Print("server: new peer: " + id);
	}

	private void OnClientDisconnected(long id)
	{
		GD.Print("server: peer disconnected: " + id);
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
					store.SetServerStatus(new ServerStatusResource{
						Version = serverStatus.Version,
						NextReset = serverStatus.ServerResets.Next
					});
				}
			),
			new(
				"Query Agent Info",
				async () => {
					try {
						var agent = await _client.My.Agent.GetAsync();
						store.SetAgentInfo(new AgentInfoResource{
							Name = agent.Data.Symbol,
							Credits = agent.Data.Credits.Value
					});
					} catch (Exception e) {
						GD.PrintErr(e);
					}
				}
			),
			new(
				"Query Fleet",
				async () => {
					var fleet = await _client.My.Ships.GetAsync((q) => {
						// TODO: pagination
						q.QueryParameters.Limit = 10;
						q.QueryParameters.Page = 1;
					});
					store.ClearShips();
					foreach (var ship in fleet.Data) {
						var res = new ShipResource {
							Name = ship.Symbol,
							Status = ship.Nav.Status.ToString()
						};
						store.AddShip(res);
					}
				}
			)
		};

		for (int i = 0; i < tasks.Length; i++)
		{
			Rpc(nameof(SetSyncProgress), (float)i / tasks.Length, tasks[i].Name);
			try
			{
				await tasks[i].Fn();
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				GetTree().Quit(1);
			}
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

	private bool buildingIndex = false;

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public async void BuildSystemIndex()
	{
		buildingIndex = true;
		await SystemIndex.Update(_client, force: false);
		Rpc(nameof(BuildSystemIndexComplete));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void BuildSystemIndexComplete()
	{
		buildingIndex = false;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public async void RequestSystemsInRect(Rect2 rect)
	{
		if (buildingIndex)
		{
			GD.Print("building system index, rejecting system query");
			return;
		}
		var systems = SystemIndex.GetSystemsInRect(rect);
		try
		{
			while (await systems.MoveNextAsync())
			{
				Rpc(nameof(AddSystem), systems.Current.ToBytes());
			}
		}
		finally
		{
			await systems.DisposeAsync();
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void AddSystem(byte[] data)
	{
		throw new NotImplementedException();
	}
}
