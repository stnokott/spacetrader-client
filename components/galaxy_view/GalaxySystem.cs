using System.Collections.Generic;
using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class GalaxySystem : Sprite2D
{
	private Label _systemNameLabel;
	public string SystemName
	{
		get => _systemNameLabel.Text;
		set => _systemNameLabel.Text = value;
	}
	private TextureRect _shipIcon;
	private Sprite2D _jumpgateIcon;
	private Node2D _jumpgateConnectionLayer;

	private Area2D _mouseArea;
	private AnimationPlayer _animationPlayer;

	private bool _selected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_systemNameLabel = GetNode<Label>("%SystemNameLabel");
		_shipIcon = GetNode<TextureRect>("%ShipIcon");
		_jumpgateIcon = GetNode<Sprite2D>("%JumpgateIcon");
		_jumpgateConnectionLayer = GetNode<Node2D>("%JumpgateConnectionLayer");

		_mouseArea = GetNode<Area2D>("%MouseArea");
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

		_mouseArea.MouseEntered += OnMouseEntered;
		_mouseArea.MouseExited += OnMouseExited;

		_mouseArea.InputEvent += (_, ev, _) =>
		{
			if (ev is InputEventMouseButton && Input.IsActionJustReleased("ui_click"))
			{
				OnClicked();
			}
		};

		SetShipCount(0);
	}

	public void SetSystem(string name, bool hasJumpgates)
	{
		// set system name
		SystemName = name;
		// set jump gate
		_jumpgateIcon.Visible = hasJumpgates;
	}

	public void SetShipCount(int n)
	{
		// set ship count
		_shipIcon.Visible = n > 0;
		if (n > 0)
		{
			_shipIcon.TooltipText = "Contains " + n + " of your ships";
		}
	}

	private void OnMouseEntered()
	{
		// keep state if selected
		if (_selected)
		{
			return;
		}
		_animationPlayer.Play("fade_in");
	}

	private void OnMouseExited()
	{
		// keep state if selected
		if (_selected)
		{
			return;
		}
		_animationPlayer.PlayBackwards("fade_in");
	}

	[Signal]
	public delegate void ClickedEventHandler();

	private void OnClicked()
	{
		EmitSignal(SignalName.Clicked);
	}

	private static readonly PackedScene SystemConnectionScene = GD.Load<PackedScene>("res://components/galaxy_view/galaxy_system_connection.tscn");

	private static readonly StringName SystemConnectionGroup = new("system_connections");
	public void Select(IEnumerable<Vector2> connections)
	{
		foreach (var connPos in connections)
		{
			var node = SystemConnectionScene.Instantiate<GalaxySystemConnection>();
			node.Position = Vector2.Zero;
			node.SetPointPosition(1, connPos - Position);
			node.AddToGroup(SystemConnectionGroup);
			_jumpgateConnectionLayer.AddChild(node);
		}

		_selected = true;
	}

	public void Deselect()
	{
		GetTree().CallGroup(SystemConnectionGroup, MethodName.QueueFree);

		_selected = false;
		OnMouseExited(); // replay exit animation to fade out
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
