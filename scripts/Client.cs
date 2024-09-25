using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class Client : Node
{
	private Control _loadingOverlay;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_loadingOverlay = GetNode<Control>("UI/LoadingOverlay");
		_loadingOverlay.Show();
		InitialSync();
	}

	private async void InitialSync()
	{
		async Task runTask(Func<IProgress<float>, Task> taskFunc, string name, float progressChunk)
		{
			var progress = new Progress<float>();
			progress.ProgressChanged += (_, p) => SetSyncProgress(name, p);
			await taskFunc(progress);
		}

		await runTask(Store.Instance.QueryServer, "Getting Server Status", 0.25f);
		await runTask(Store.Instance.QueryAgent, "Loading Agent", 0.25f);
		await runTask(Store.Instance.QuerySystems, "Loading Systems", 0.25f);
		await runTask(Store.Instance.QueryShips, "Loading Fleet", 0.25f);

		_loadingOverlay.Hide();
		// only required initially, so can be removed for good
		_loadingOverlay.QueueFree();
	}

	private void SetSyncProgress(string desc, float p)
	{
		_loadingOverlay.Call("set_progress", desc, p);
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
