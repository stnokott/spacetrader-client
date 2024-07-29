using Godot;
using System;

public partial class SystemNode : GraphNode
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CenterPivotOffset();
	}

	private void CenterPivotOffset()
	{
		PivotOffset = Size / 2;
	}
}
