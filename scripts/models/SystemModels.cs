using System.Collections.Generic;
using Godot;

namespace Models;

public struct SystemModel
{
	public string Name;
	public Vector2 Pos;
	public bool HasJumpgates;
}

public struct DetailedSystemModel
{
	public IReadOnlySet<string> connectedSystems;
}
