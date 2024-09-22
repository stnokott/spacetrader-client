using Godot;
using System;

public partial class ShipList : VBoxContainer
{
	[Export]
	public Galaxy galaxy;

	private VBoxContainer _shipListContainer;

	public override void _Ready()
	{
		_shipListContainer = GetNode<VBoxContainer>("%ShipListContainer");

		Store.Instance.ShipUpdate += OnShipUpdated;
	}

	private static readonly PackedScene _shipItemScene = GD.Load<PackedScene>("res://components/shiplist/ship_list_item.tscn");

	private void OnShipUpdated(string shipName)
	{
		var ship = Store.Instance.Graph.GetShip(shipName);
		// use existing node if possible
		var node = GetNodeOrNull<ShipListItem>(ship.Name);
		if (node == null)
		{
			// create new node if null
			node = _shipItemScene.Instantiate<ShipListItem>();
			_shipListContainer.AddChild(node);
			node.Name = ship.Name; // set node name for identification
		}
		// update ship data
		node.SetShip(ship, () => galaxy.ZoomTo(Store.Instance.Graph.GetSystem(ship.SystemName).Model.Pos));
	}
}
