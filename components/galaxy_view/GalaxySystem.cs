using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Reflection;
using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class GalaxySystem : Sprite2D
{
	private Label _systemNameLabel;
	private TextureRect _shipIcon;
	private Sprite2D _jumpgateIcon;
	private Area2D _mouseArea;
	private AnimationPlayer _animationPlayer;

	private bool selected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_systemNameLabel = GetNode<Label>("%SystemNameLabel");
		_shipIcon = GetNode<TextureRect>("%ShipIcon");
		_jumpgateIcon = GetNode<Sprite2D>("%JumpgateIcon");

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
		_systemNameLabel.Text = name;
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
		if (selected)
		{
			return;
		}
		_animationPlayer.Play("fade_in");
	}

	private void OnMouseExited()
	{
		// keep state if selected
		if (selected)
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

	public void Select()
	{
		selected = true;
		OnMouseEntered();
	}

	public void Deselect()
	{
		selected = false;
		OnMouseExited();
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
