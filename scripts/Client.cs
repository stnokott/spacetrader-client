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
		// TODO: modularize
		SetSyncProgress(0f, "Checking Server Status");
		await Store.Instance.QueryServer();
		SetSyncProgress(0.25f, "Loading Agent");
		await Store.Instance.QueryAgent();
		SetSyncProgress(0.5f, "Loading Systems");
		await Store.Instance.QuerySystems();
		SetSyncProgress(0.75f, "Loading Fleet");
		await Store.Instance.QueryShips();
		_loadingOverlay.Hide();
		// only required initially, so can be removed for good
		_loadingOverlay.QueueFree();
	}

	private void SetSyncProgress(float p, string desc)
	{
		_loadingOverlay.Call("set_progress", p, desc);
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
