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

public partial class API : Control
{
	const int PORT = 55555;
	const int MAX_CLIENT = 1;

	private readonly SpaceTradersClient _client;

	public API()
	{
		var authProvider = new AnonymousAuthenticationProvider();
		var adapter = new HttpClientRequestAdapter(authProvider);
		_client = new SpaceTradersClient(adapter);
	}

	private Label _loadingDesc;
	private ProgressBar _loadingProgress;
	private Node _loadingOverlay;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_loadingDesc = GetNode<Label>("LoadingOverlay/VBoxContainer/Description");
		_loadingProgress = GetNode<ProgressBar>("LoadingOverlay/VBoxContainer/Progress");
		_loadingOverlay = GetNode<Node>("LoadingOverlay");
	}

	private delegate Task SyncTaskFunc();
	sealed private record SyncTask(string Name, SyncTaskFunc Fn);

	public void DoInit()
	{
		_ = Init();
	}

	/// <summary>
	/// Queries all required data from the API and sends it to the client via RPC.
	/// </summary>
	public async Task Init()
	{
		GD.Print("sync start");

		SyncTask[] tasks = {
			new(
				"Querying Server Status",
				async () => {
					var serverStatus = await _client.GetAsync();
					GD.Print("got server version" + serverStatus.Version);
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
			_loadingDesc.SetDeferred("text", tasks[i].Name + "...");
			_loadingProgress.SetDeferred("value", (float)i / tasks.Length);
			await tasks[i].Fn();
		}

		GD.Print("sync finished");
		Rpc(nameof(InitFinished));
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void InitFinished()
	{
		_loadingOverlay.QueueFree();
	}
}
