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
using System.Collections.Generic;
using System.Threading;
using System.Data;

namespace Server;

public partial class Server : Node
{
	const int PORT = 55555;
	const int MAX_CLIENT = 1;

	private readonly SpaceTradersClient _client;

	internal class TokenProvider : IAccessTokenProvider
	{
		public AllowedHostsValidator AllowedHostsValidator { get; }

		public Task<string> GetAuthorizationTokenAsync(
		Uri uri,
		Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = default)
		{
			return Task.FromResult("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZGVudGlmaWVyIjoiU1ROT0tPVFQxIiwidmVyc2lvbiI6InYyLjIuMCIsInJlc2V0X2RhdGUiOiIyMDI0LTA2LTMwIiwiaWF0IjoxNzIwODg3ODU4LCJzdWIiOiJhZ2VudC10b2tlbiJ9.HX20mNrnXmH_jr3aQwFjXVK2DJ9s0ggfjMS-LEH7lLpom5CGr5q2qjvwiB7iFsz_2aibmx1o89lI4hs9vSOotUPX33IW5exaHH5MLExinTXC-VL0CcGcm103ThV3ZFQO7lLcXsuqm7VNX6XwE-s8_Z5OGDnWeuVb8AGz7dCW2SpsUk44M-04yhzmEra3XOmxYVWwvATY99xCoYZt9sHiVcw-vY2UOCY5dAzYhh_y--Uzy8YKnkpmaffhc2EWyQyoZw0R3iJorI2R9C-Kqa9HUJ6H1KpPRWRSAp-YfpSqUyIOFkc03F8D3SUlq5sfDA80w6VXCJry1QFAb9bcxWRGOQ");
		}
	}

	public Server()
	{
		var authProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider());
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
				"Querying Agent Info",
				async () => {
					try {
						var agent = await _client.My.Agent.GetAsync();
						Rpc(nameof(SetAgentInfo),agent.Data.Symbol, agent.Data.Credits ?? 0);
					} catch (Exception e) {
						GD.PrintErr(e);
					}
				}
			),
			new(
				"Querying Fleet",
				async () => {
					// TODO: pagination
					var fleet = await _client.My.Ships.GetAsync();
					foreach (var ship in fleet.Data) {
						var res = new ShipResource {
							Name = ship.Symbol,
							Registration = ship.Registration.Name
						};
						var packed = res.ToBytes();
						Rpc(nameof(AddShip), packed);
					}
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

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SetAgentInfo(string name, long credits)
	{
		throw new NotImplementedException();
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void ClearShips()
	{
		throw new NotImplementedException();
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void AddShip(byte[] data)
	{
		throw new NotImplementedException();
	}
}
