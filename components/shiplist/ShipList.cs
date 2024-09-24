using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

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
		var ship = Store.Instance.Data.Ships[shipName];
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
		node.SetShip(ship, () => galaxy.ZoomTo(Store.Instance.Data.Systems[ship.SystemName].Pos));
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
