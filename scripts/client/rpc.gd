extends Node

const PORT = 55555

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$MultiplayerSpawner.set_spawn_function(_spawn_function)
	
	var peer: ENetMultiplayerPeer = ENetMultiplayerPeer.new()
	peer.create_client("localhost", PORT)
	multiplayer.connected_to_server.connect(_on_connected)
	multiplayer.connection_failed.connect(_on_connection_failure)
	multiplayer.multiplayer_peer = peer
	
func _spawn_function(x: Variant) -> Node:
	print("SpawnFunction Client: ", str(x))
	var rect = ColorRect.new()
	rect.color = Color("red")
	return rect
	

func _on_connected() -> void:
	print("connected")
	
	var data = { "key": "value" }
	var packed = var_to_bytes_with_objects(data)
	TestFunction.rpc_id(1, packed)
	
	_initialSync()
	
@rpc("any_peer", "call_local", "reliable")
func TestFunction(data: PackedByteArray):
	pass

	
func _on_connection_failure() -> void:
	printerr("connection to server failed")
	get_tree().quit(1)
	
# TODO: move all below to separate script

func _initialSync() -> void:
	RequestSync.rpc_id(1)

@rpc("any_peer", "call_remote", "reliable")
func RequestSync() -> void: pass

@onready var loadingOverlay: PanelContainer = get_node("UI/LoadingOverlay")
@onready var loadingProgress: ProgressBar = get_node("UI/LoadingOverlay/VBoxContainer/Progress")
@onready var loadingDesc: Label = get_node("UI/LoadingOverlay/VBoxContainer/Description")

@rpc("authority", "call_remote", "reliable")
func SetSyncProgress(progress: float, desc: String) -> void:
	loadingProgress.value = progress
	loadingDesc.text = desc + "..."

@rpc("authority", "call_remote", "reliable")	
func SyncComplete() -> void:
	loadingOverlay.hide()
	
@onready var gameVersionLabel: Label = get_node("UI/VBoxContainer/Header/HBoxContainer/VBoxLeft/GameVersionLabel")
@onready var nextResetLabel: Label = get_node("UI/VBoxContainer/Header/HBoxContainer/VBoxLeft/NextReset")

@rpc("authority", "call_remote", "reliable")
func SetServerStatus(version: String, next_reset: String) -> void:
	gameVersionLabel.text = "SpaceTraders " + version
	nextResetLabel.text = "Next reset: " + next_reset
