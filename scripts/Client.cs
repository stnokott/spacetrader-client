using System.Security.AccessControl;
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
		SetSyncProgress(0f, "Querying Server Status");
		await Store.Instance.UpdateServerStatus();
		SetSyncProgress(0.3f, "Updating Agent");
		await Store.Instance.UpdateAgentInfo();
		SetSyncProgress(0.6f, "Querying Fleet");
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

/*

@onready var system_graph: Node = $UI/VBoxContainer/CenterPanel/HSplitContainer/HSplitContainer/SystemGraph

#@rpc("any_peer", "call_local", "reliable")
func BuildSystemIndex() -> void:
	# TODO: move this and below to system graph
	system_graph.show_loading_overlay("Building System Index...")
	var result = await Store.GetSystemsInRect(Vector2i(-500, -500), Vector2i(500, 500))
	BuildSystemIndexComplete()
	
#@rpc("authority", "call_local", "reliable")
func BuildSystemIndexComplete() -> void:
	system_graph.hide_loading_overlay()
	# system_graph.refresh_systems(true)

func _on_system_graph_update_required(rect: Rect2i) -> void:
	if !connected:
		await multiplayer.connected_to_server
	system_graph.clear_systems()
	RequestSystemsInRect.rpc_id(1, rect)

#@rpc("any_peer", "call_remote", "reliable")
func RequestSystemsInRect(_rect: Rect2) -> void: pass

#@rpc("authority", "call_remote", "reliable")
#func AddSystem(data: PackedByteArray) -> void:
#	system_graph.add_system(SystemResource.FromBytes(data))
#endregion

*/
