using Godot;
using System;

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
		if (inputEvent is InputEventMouseButton)
		{
			if (inputEvent.IsActionPressed("ui_click"))
			{
				AddThemeStyleboxOverride("panel", GetThemeStylebox("pressed"));
			}
			else if (Input.IsActionJustReleased("ui_click"))
			{
				RemoveThemeStyleboxOverride("panel");
			}
		}
	}

	private static readonly Texture2D _flightStatusDocked = GD.Load<Texture2D>("res://textures/ship_status/docked.tres");
	private static readonly Texture2D _flightStatusInOrbit = GD.Load<Texture2D>("res://textures/ship_status/in_orbit.tres");
	private static readonly Texture2D _flightStatusInTransit = GD.Load<Texture2D>("res://textures/ship_status/in_transit.tres");
	private static readonly Texture2D _flightStatusUnknown = GD.Load<Texture2D>("res://textures/ship_status/unknown.tres");

	public void SetShip(ShipModel ship, Action onLocateButton)
	{
		Texture2D iconRes = ship.Status switch
		{
			GrpcSpacetrader.Ship.Types.FlightStatus.Docked => _flightStatusDocked,
			GrpcSpacetrader.Ship.Types.FlightStatus.InOrbit => _flightStatusInOrbit,
			GrpcSpacetrader.Ship.Types.FlightStatus.InTransit => _flightStatusInTransit,
			_ => _flightStatusUnknown,
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
