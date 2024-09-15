using Godot;

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
		await Store.Instance.UpdateServerStatus();
		SetSyncProgress(0.25f, "Loading Agent");
		await Store.Instance.UpdateAgentInfo();
		SetSyncProgress(0.5f, "Loading Systems");
		await Store.Instance.UpdateSystems();
		SetSyncProgress(0.75f, "Loading Fleet");
		await Store.Instance.UpdateShips();
		_loadingOverlay.Hide();
		// only required initially, so can be removed for good
		_loadingOverlay.QueueFree();
	}

	private void SetSyncProgress(float p, string desc)
	{
		_loadingOverlay.Call("set_progress", p, desc);
	}
}
