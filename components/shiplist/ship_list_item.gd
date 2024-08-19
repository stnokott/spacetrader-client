extends Control
class_name ShipListItem

const InternalShip = preload("res://scripts/models/InternalShip.cs")

@onready
var statusIcon: TextureRect = %ShipStatusIcon
@onready
var nameLabel: Label = %ShipNameLabel

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
	statusIcon.texture = iconRes
	
	nameLabel.text = ship.Name
