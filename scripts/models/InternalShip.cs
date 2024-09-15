using Godot;

public partial class InternalShip : Resource
{
	[Export]
	public string Name { get; set; }

	[Export]
	public string Status { get; set; }

	[Export]
	public Vector2 Pos { get; set; }
}
