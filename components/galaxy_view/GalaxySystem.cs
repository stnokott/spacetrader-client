using Godot;

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
	}

	public void SetSystem(string name, int shipCountInSys, bool hasJumpgates)
	{
		// set system name
		_systemNameLabel.Text = name;
		// set ship count
		_shipIcon.Visible = shipCountInSys > 0;
		if (shipCountInSys > 0)
		{
			_shipIcon.TooltipText = "Contains " + shipCountInSys + " of your ships";
		}
		// set jump gate
		_jumpgateIcon.Visible = hasJumpgates;
	}
}
