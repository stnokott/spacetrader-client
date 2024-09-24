using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class GalaxySystem : Sprite2D
{
	private Label _systemNameLabel;
	private TextureRect _shipIcon;
	private Sprite2D _jumpgateIcon;
	private Area2D _mouseArea;
	private AnimationPlayer _animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_systemNameLabel = GetNode<Label>("%SystemNameLabel");
		_shipIcon = GetNode<TextureRect>("%ShipIcon");
		_jumpgateIcon = GetNode<Sprite2D>("%JumpgateIcon");

		_mouseArea = GetNode<Area2D>("%MouseArea");
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

		_mouseArea.MouseEntered += () =>
		{
			_animationPlayer.Play("fade_in");
		};
		_mouseArea.MouseExited += () =>
		{
			_animationPlayer.PlayBackwards("fade_in");
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
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
