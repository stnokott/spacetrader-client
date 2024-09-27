using Godot;

#pragma warning disable CS8618 // Godot classes are reliably initialized in _Ready()

public partial class GalaxySystemConnection : Line2D
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		_animationPlayer.Play("fade_in");
	}

	public override async void _ExitTree()
	{
		_animationPlayer.PlayBackwards("fade_in");
		await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
	}
}

#pragma warning restore CS8618 // Godot classes are reliably initialized in _Ready()
