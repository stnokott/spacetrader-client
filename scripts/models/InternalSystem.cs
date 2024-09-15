using Godot;

public partial class InternalSystem : Resource
{
	[Export]
	public Vector2 Pos { get; set; }

	[Export]
	public int ShipCount { get; set; }

	[Export]
	public bool HasJumpgates { get; set; }
}
