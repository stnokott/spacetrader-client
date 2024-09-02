using Godot;
using System.Threading.Tasks;

using MathExtensionMethods;
using System.Linq;

public partial class Galaxy : Node2D
{
	private static readonly PackedScene _systemScene = GD.Load<PackedScene>("res://components/galaxy_view/galaxy_system.tscn");

	private Camera2D _camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("%Viewport");

		CallDeferred(MethodName.OnCameraViewportChanged);
	}

	public async void OnCameraViewportChanged()
	{
		await RefreshSystems();
	}

	// number of pixels to prefetch in each direction of the viewport
	private const int PIXEL_PREFETCH = 300;

	// TODO: automatic refresh

	private async Task RefreshSystems()
	{
		var cameraViewport = GetCameraViewportRect().AsInt();
		// expand viewport to include prefetched pixels
		cameraViewport = cameraViewport.Grow(PIXEL_PREFETCH);

		var result = await Store.Instance.GetSystemsInRect(cameraViewport.Position, cameraViewport.End);
		ClearSystemNodes();
		foreach (var item in result)
		{
			AddSystemNode(item);
		}
	}

	private Rect2 GetCameraViewportRect()
	{
		var cameraSize = _camera.GetViewportRect().Size * _camera.Zoom;
		var cameraRect = new Rect2(_camera.Position - cameraSize / 2, cameraSize);
		return cameraRect;
	}

	public void AddSystemNode(GrpcSpacetrader.GetSystemsInRectResponse item)
	{
		var system = item.System;
		var node = _systemScene.Instantiate<GalaxySystem>();
		node.Position = new Vector2(system.X, system.Y);
		AddChild(node);
		node.SetSystem(system, item.ShipCount, item.ConnectedSystems.Count > 0);
	}

	// TODO: reuse instead of re-creating nodes
	public void ClearSystemNodes()
	{
		foreach (var node in GetChildren())
		{
			if (node is GalaxySystem)
			{
				RemoveChild(node);
				node.QueueFree();
			}
		}
	}

	public async void ZoomToShip(string shipName)
	{
		Vector2 shipCoords = await Store.Instance.GetShipCoordinates(shipName);
		if (shipCoords.IsEqualApprox(_camera.Position))
		{
			return;
		}
		_camera.Position = shipCoords;
		await RefreshSystems();
	}
}