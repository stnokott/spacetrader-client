using Godot;
using System;
using Models;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class ShipListItem : PanelContainer
{

	private TextureRect _statusIcon;
	private Label _nameLabel;
	private Button _locateButton;

	public override void _Ready()
	{
		_statusIcon = GetNode<TextureRect>("%ShipStatusIcon");
		_nameLabel = GetNode<Label>("%ShipNameLabel");
		_locateButton = GetNode<Button>("%LocateOnMapButton");

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		GuiInput += OnInput;
	}

	private void OnMouseEntered()
	{
		AddThemeStyleboxOverride("panel", GetThemeStylebox("hover"));
	}

	private void OnMouseExited()
	{
		RemoveThemeStyleboxOverride("panel");
	}

	private void OnInput(InputEvent inputEvent)
	{
		if (inputEvent is not InputEventMouseButton) return;
		if (inputEvent.IsActionPressed("ui_click"))
		{
			AddThemeStyleboxOverride("panel", GetThemeStylebox("pressed"));
		}
		else if (Input.IsActionJustReleased("ui_click"))
		{
			RemoveThemeStyleboxOverride("panel");
		}
	}

	private static readonly Texture2D FlightStatusDocked = GD.Load<Texture2D>("res://textures/ship_status/docked.tres");
	private static readonly Texture2D FlightStatusInOrbit = GD.Load<Texture2D>("res://textures/ship_status/in_orbit.tres");
	private static readonly Texture2D FlightStatusInTransit = GD.Load<Texture2D>("res://textures/ship_status/in_transit.tres");
	private static readonly Texture2D FlightStatusUnknown = GD.Load<Texture2D>("res://textures/ship_status/unknown.tres");

	public void SetShip(ShipModel ship, Action onLocateButton)
	{
		Texture2D iconRes = ship.Status switch
		{
			GraphQLModels.ShipStatus.Docked => FlightStatusDocked,
			GraphQLModels.ShipStatus.InOrbit => FlightStatusInOrbit,
			GraphQLModels.ShipStatus.InTransit => FlightStatusInTransit,
			_ => FlightStatusUnknown,
		};
		_statusIcon.Texture = iconRes;
		_nameLabel.Text = ship.Name;

		foreach (var conn in _locateButton.GetSignalConnectionList(BaseButton.SignalName.Pressed))
		{
			_locateButton.Disconnect(BaseButton.SignalName.Pressed, (Callable)conn["callable"]);
		}
		_locateButton.Pressed += onLocateButton;
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
