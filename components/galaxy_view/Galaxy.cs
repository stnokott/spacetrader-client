using System.Linq;
using System.Threading.Tasks;
using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class Galaxy : Node2D
{
	private static readonly PackedScene _systemScene = GD.Load<PackedScene>("res://components/galaxy_view/galaxy_system.tscn");

	private Camera2D _camera;
	private Area2D _cameraCollisionArea;
	private StringName _visibleNodesGroup = new("visible_nodes");
	private Node2D _systemNodeLayer;

	private GalaxySystem? selectedSystem = null;

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("%Viewport");
		_cameraCollisionArea = GetNode<Area2D>("%CameraCollisionArea");
		_systemNodeLayer = GetNode<Node2D>("%SystemNodeLayer");

		// we use  collision area bound to the camera viewport to track which systems
		// are currently visible.
		// this enables us to only apply scaling fixes related to camera zoom
		// to the systems which are currently visible instead of having
		// to iterate all ~9000 system nodes.
		// additionally, we set Visible=false for nodes not in the camera's
		// viewport since Godot doesn't perform 2D culling at the time of writing.
		_cameraCollisionArea.AreaEntered += (systemArea) =>
		{
			var systemNode = systemArea.GetParent<GalaxySystem>();
			systemNode.Visible = true;
			systemNode.Scale = NodeScaleForZoom(_camera.Zoom);
			systemNode.AddToGroup(_visibleNodesGroup);
		};
		_cameraCollisionArea.AreaExited += (systemArea) =>
		{
			var systemNode = systemArea.GetParent<GalaxySystem>();
			if (systemNode != selectedSystem)
			{
				// only hide if not selected
				systemNode.Visible = false;
			}
			systemNode.RemoveFromGroup(_visibleNodesGroup);
		};
		// enable physics object picking so that click events on system nodes don't propagate.
		GetViewport().PhysicsObjectPickingSort = true;
		GetViewport().PhysicsObjectPickingFirstOnly = true;
		_cameraCollisionArea.InputEvent += (_, ev, _) =>
		{
			if (ev is InputEventMouseButton && Input.IsActionJustReleased("ui_click"))
			{
				DeselectSystem();
			}
		};

		Store.Instance.SystemUpdate += OnSystemUpdated;
		Store.Instance.ShipMovedFromSystem += OnShipMoved;
		Store.Instance.ShipMovedToSystem += OnShipMoved;
	}

	private void OnSystemUpdated(string systemName)
	{
		var sys = Store.Instance.Data.Systems[systemName];
		var node = _systemNodeLayer.GetNodeOrNull<GalaxySystem>(systemName);

		if (node == null)
		{
			// create new node instance
			node = _systemScene.Instantiate<GalaxySystem>();
			node.Name = systemName;
			node.Position = sys.Pos;
			node.Scale = NodeScaleForZoom(_camera.Zoom);
			node.Clicked += () => OnSystemClicked(node);
			_systemNodeLayer.AddChild(node);
		}

		node.SetSystem(systemName, sys.HasJumpgates);
	}

	private async void OnSystemClicked(GalaxySystem node)
	{
		if (node == selectedSystem)
		{
			return;
		}
		DeselectSystem();
		selectedSystem = node;

		var system = await Store.Instance.QuerySystem(node.SystemName);
		var connections = system.connectedSystems.Select((systemName) =>
		{
			// get system position by id from node
			return _systemNodeLayer.GetNode<Node2D>(systemName).Position;
		});
		node.Select(connections);
	}

	private void DeselectSystem()
	{
		if (selectedSystem == null)
		{
			return;
		}
		selectedSystem.Deselect();
		if (!selectedSystem.IsInGroup(_visibleNodesGroup))
		{
			selectedSystem.Visible = false;
		}
		selectedSystem = null;
	}

	private void OnShipMoved(string _, string systemName)
	{
		var node = _systemNodeLayer.GetNode<GalaxySystem>(systemName);
		var shipCount = Store.Instance.ShipsInSystem(systemName).Count;
		node.SetShipCount(shipCount);
	}

	public void OnCameraZoomChanged()
	{
		// scale nodes inverse to camera zoom so they always stay the same size
		// regardless of zoom.
		// (i.e. when zooming out, nodes will scale up and vice-versa)
		GetTree().SetGroup(_visibleNodesGroup, "scale", NodeScaleForZoom(_camera.Zoom));
	}

	private static Vector2 NodeScaleForZoom(Vector2 zoom)
	{
		return Vector2.One / zoom;
	}

	public void ZoomTo(Vector2 pos)
	{
		if (!pos.IsEqualApprox(_camera.Position))
		{
			_camera.Position = pos;
		}
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
