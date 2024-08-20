extends PanelContainer
class_name ShipListItem

const InternalShip = preload("res://scripts/models/InternalShip.cs")

@onready
var status_icon: TextureRect = %ShipStatusIcon
@onready
var name_label: Label = %ShipNameLabel
@onready
var locate_button: Button = %LocateOnMapButton

signal locate_button_pressed()

func _ready() -> void:
	self.mouse_entered.connect(_on_mouse_entered)
	self.mouse_exited.connect(_on_mouse_exited)
	self.gui_input.connect(_on_input)
	# pass ship name to signal
	locate_button.pressed.connect(locate_button_pressed.emit)
	
func _on_mouse_entered() -> void:
	add_theme_stylebox_override("panel", get_theme_stylebox("hover"))

func _on_mouse_exited() -> void:
	remove_theme_stylebox_override("panel")
	
func _on_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if Input.is_action_just_pressed("ui_click"):
			add_theme_stylebox_override("panel", get_theme_stylebox("pressed"))
		elif Input.is_action_just_released("ui_click"):
			remove_theme_stylebox_override("panel")

func set_ship(ship: InternalShip) -> void:
	var iconRes: Texture2D
	match ship.Status:
		"DOCKED":
			iconRes = load("res://textures/ship_status/docked.tres")
		"IN_ORBIT":
			iconRes = load("res://textures/ship_status/in_orbit.tres")
		"IN_TRANSIT":
			iconRes = load("res://textures/ship_status/in_transit.tres")
		_:
			iconRes = load("res://textures/ship_status/unknown.tres")
	status_icon.texture = iconRes
	
	name_label.text = ship.Name
