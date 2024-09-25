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
		var totalProgress = 0f;

		var runTask = async (Func<IProgress<float>, Task> taskFunc, string name, float progressChunk) =>
		{
			var progress = new Progress<float>();
			progress.ProgressChanged += (_, p) =>
			{
				// calculate progress within chunk
				var pChunk = totalProgress + (progressChunk * p);
				SetSyncProgress(name, pChunk);
			};
			await taskFunc(progress);
			totalProgress += progressChunk;
			Debug.Assert(totalProgress <= 1.0);
		};

		await runTask(Store.Instance.QueryServer, "Checking Server Status", 0.25f);
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
