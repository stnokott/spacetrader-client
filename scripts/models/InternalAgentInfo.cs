using Godot;

namespace Models;

public partial class InternalAgentInfo : Resource
{
	[Export]
	public string Name { get; set; }
	[Export]
	public long Credits { get; set; }
}
