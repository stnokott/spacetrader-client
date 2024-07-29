using Godot;

public partial class SystemNode : GraphNode
{

	private Label SystemName;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SystemName = GetNode<Label>("SystemName");

		ItemRectChanged += CenterPivotOffset;
	}

	private void CenterPivotOffset()
	{
		PivotOffset = Size / 2;
	}

	private static readonly PackedScene _systemNodeScene = GD.Load<PackedScene>("res://components/system_graph/system_node.tscn");

	public static void Add(Node parent, string name, Vector2 pos)
	{
		var node = _systemNodeScene.Instantiate<SystemNode>();
		node.PositionOffset = pos;

		parent.AddChild(node);
		// _Ready is called, we can now perform operations on child nodes
		node.SystemName.Text = name;
	}
}
