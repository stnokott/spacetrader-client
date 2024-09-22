using System;
using System.Linq;
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

		// we use  collision area bound to the camera viewport to track which systems
		// are currently visible.
		// this enables us to only apply scaling fixes related to camera zoom
		// to the systems which are currently visible instead of having
		// to iterate all ~9000 system nodes.
		// additionally, we set Visible=false for nodes not in the camera's
		// viewport since Godot doesn't perform 2D culling at the time of writing.
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

		Store.Instance.SystemUpdate += OnSystemUpdated;
	}

	private void OnSystemUpdated(string systemName)
	{
		var sys = Store.Instance.Graph.GetSystem(systemName);
		var node = _systemNodeLayer.GetNodeOrNull<GalaxySystem>(systemName);

		if (node == null)
		{
			node = _systemScene.Instantiate<GalaxySystem>();
			node.Name = systemName;
			node.Position = sys.Model.Pos;
			node.Scale = NodeScaleForZoom(_camera.Zoom);
			_systemNodeLayer.AddChild(node);
		}

		node.SetSystem(systemName, sys.Ships.Count, sys.Model.HasJumpgates);
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
