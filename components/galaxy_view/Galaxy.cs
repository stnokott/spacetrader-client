using Godot;

public partial class Galaxy : Node2D
{
	private static readonly PackedScene _systemScene = GD.Load<PackedScene>("res://components/galaxy_view/galaxy_system.tscn");

	private Camera2D _camera;
	private Node2D _systemNodeLayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("%Viewport");
		_systemNodeLayer = GetNode<Node2D>("%SystemNodeLayer");

		Store.Instance.SystemsUpdated += () =>
		{
			CallDeferred(MethodName.RefreshSystems);
		};
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
		ScaleNodeToCameraZoom(node, _camera.Zoom);
		_systemNodeLayer.AddChild(node);
		node.SetSystem(name, shipCount, hasJumpgates);
	}

	private void ClearSystemNodes()
	{
		foreach (var node in _systemNodeLayer.GetChildren())
		{
			_systemNodeLayer.RemoveChild(node);
			node.QueueFree();
		}
	}

	public void OnCameraZoomChanged()
	{
		var cameraZoom = _camera.Zoom;
		foreach (var node in _systemNodeLayer.GetChildren())
		{
			ScaleNodeToCameraZoom((Node2D)node, cameraZoom);
		}
	}

	private static void ScaleNodeToCameraZoom(Node2D node, Vector2 zoom)
	{
		node.Scale = Vector2.One / zoom;
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
