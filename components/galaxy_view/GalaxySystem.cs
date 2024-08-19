using Godot;
using System;

public partial class GalaxySystem : Node2D
{
	private Label _systemNameLabel;
	private HBoxContainer _shipCountContainer;
	private Label _shipCountLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_systemNameLabel = GetNode<Label>("%SystemNameLabel");
		_shipCountContainer = GetNode<HBoxContainer>("%ShipCountContainer");
		_shipCountLabel = GetNode<Label>("%ShipCountLabel");
	}

	public void SetData(string systemName, int shipCount)
	{
		_systemNameLabel.Text = systemName;
		if (shipCount > 0)
		{
			_shipCountLabel.Text = shipCount.ToString();
			_shipCountContainer.Visible = true;
		}
		else
		{
			_shipCountContainer.Visible = false;
		}
	}
}
