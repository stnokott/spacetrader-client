using Godot;

public partial class Galaxy : Node2D
{
	private static readonly PackedScene _systemScene = GD.Load<PackedScene>("res://components/galaxy_view/galaxy_system.tscn");

	private Camera2D _camera;
	private Area2D _cameraCollisionArea;
	private StringName _visibleNodesGroup = new("visible_nodes");
	private Node2D _systemNodeLayer;

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("%Viewport");
		_cameraCollisionArea = GetNode<Area2D>("%CameraCollisionArea");
		_systemNodeLayer = GetNode<Node2D>("%SystemNodeLayer");

		_cameraCollisionArea.AreaEntered += (area) =>
		{
			var systemNode = area.GetParent<GalaxySystem>();
			systemNode.Visible = true;
			systemNode.Scale = NodeScaleForZoom(_camera.Zoom);
			systemNode.AddToGroup(_visibleNodesGroup);
		};
		_cameraCollisionArea.AreaExited += (area) =>
		{
			var systemNode = area.GetParent<GalaxySystem>();
			systemNode.Visible = false;
			systemNode.RemoveFromGroup(_visibleNodesGroup);
		};

		Store.Instance.SystemsUpdated += () =>
		{
			CallDeferred(MethodName.RefreshSystems);
		};
	}

	private void ClearSystemNodes()
	{
		foreach (var node in _systemNodeLayer.GetChildren())
		{
			_systemNodeLayer.RemoveChild(node);
			node.QueueFree();
		}
	}

	private void RefreshSystems()
	{
		ClearSystemNodes();
		foreach (var sys in Store.Instance.Systems)
		{
			AddSystemNode(sys.Key, sys.Value.Pos, sys.Value.ShipCount, sys.Value.HasJumpgates);
		}
	}

	private void AddSystemNode(string name, Vector2 pos, int shipCount, bool hasJumpgates)
	{
		var node = _systemScene.Instantiate<GalaxySystem>();
		node.Position = pos;
		node.Scale = NodeScaleForZoom(_camera.Zoom);
		_systemNodeLayer.AddChild(node);
		node.SetSystem(name, shipCount, hasJumpgates);
	}

	public void OnCameraZoomChanged()
	{
		GetTree().SetGroup(_visibleNodesGroup, "scale", NodeScaleForZoom(_camera.Zoom));
	}

	private static Vector2 NodeScaleForZoom(Vector2 zoom)
	{
		return Vector2.One / zoom;
	}

	// TODO: store ship coordinates when querying fleet to avoid gRPC call here
	public async void ZoomToShip(string shipName)
	{
		Vector2 shipCoords = await Store.Instance.GetShipCoordinates(shipName);
		if (shipCoords.IsEqualApprox(_camera.Position))
		{
			return;
		}
		_camera.Position = shipCoords;
	}
}
