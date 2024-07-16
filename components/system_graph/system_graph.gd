extends PanelContainer

const SystemResource = preload("res://models/SystemResource.cs")

@onready var graph: GraphEdit = $GraphEdit

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	OS.low_processor_usage_mode = true

@onready var loadingOverlay: PanelContainer = $LoadingOverlay
@onready var loadingLabel: Label = $LoadingOverlay/VBoxContainer/Label

func show_loading_overlay(text: String):
	loadingLabel.text = text
	loadingOverlay.show()
	
func hide_loading_overlay():
	loadingOverlay.hide()
	
func clear_systems() -> void:
	for n in graph.get_children():
		if n is GraphNode:
			graph.remove_child(n)
			n.queue_free()

# defines the scaling between pixels and galaxy coordinates
# higher values allow a higher coordinate range to be displayed (effectively zooming out)
const UNITS_PER_PIXEL: float = 2.0
# number of pixels to prefetch in each direction of the viewport
const PIXEL_PREFETCH: int = 100

signal graph_update_required(rect: Rect2i)

var prev_viewport: Rect2 = Rect2()

func refresh_systems(force: bool = false) -> void:
	var offset_px: Vector2 = graph.scroll_offset # offset is the top-left coordinate of the viewport
	offset_px -= Vector2(PIXEL_PREFETCH, PIXEL_PREFETCH)
	var size_px: Vector2 = graph.size
	size_px += Vector2(PIXEL_PREFETCH, PIXEL_PREFETCH)*2
	var zoom: float = graph.zoom
	
	var viewport_offset: Vector2 = offset_px * UNITS_PER_PIXEL / zoom
	var viewport_size: Vector2 = size_px * UNITS_PER_PIXEL / zoom
	
	var viewport: Rect2 = Rect2(viewport_offset, viewport_size)
	
	if force or viewport != prev_viewport:
		emit_signal("graph_update_required", viewport)
		prev_viewport = viewport

func add_system(system: SystemResource) -> void:
	var node: GraphNode = GraphNode.new()
	node.title = system.Name
	node.position_offset = system.Pos / UNITS_PER_PIXEL
	node.draggable = false
	graph.add_child(node)
