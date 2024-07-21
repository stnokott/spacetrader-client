using Godot;

namespace Models;


public partial class InternalServerStatus : Resource
{
	[Export]
	public string Version { get; set; }
	[Export]
	public string NextReset { get; set; }
}
