using System;
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
		async Task runTask(Func<IProgress<float>, Task> taskFunc, string name)
		{
			SetSyncProgress(name, 0f);
			var progress = new Progress<float>((p) => SetSyncProgress(name, p));
			await taskFunc(progress);
		}

		await runTask(Store.Instance.QueryServer, "Getting Server Status");
		await runTask(Store.Instance.QueryAgent, "Loading Agent");
		await runTask(Store.Instance.QuerySystems, "Loading Galaxy");
		await runTask(Store.Instance.QueryShips, "Loading Fleet");

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
