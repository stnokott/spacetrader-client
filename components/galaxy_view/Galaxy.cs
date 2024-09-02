using Godot;
using System.Threading.Tasks;

using MathExtensionMethods;
using System.Linq;
using System.Collections.Generic;
using System;

public partial class Galaxy : Node2D
{
	private static readonly PackedScene _systemScene = GD.Load<PackedScene>("res://components/galaxy_view/galaxy_system.tscn");

	private Camera2D _camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("%Viewport");

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

	public void AddSystemNode(string name, Vector2 pos, int shipCount, bool hasJumpgates)
	{
		var node = _systemScene.Instantiate<GalaxySystem>();
		node.Position = pos;
		AddChild(node);
		node.SetSystem(name, shipCount, hasJumpgates);
	}

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
