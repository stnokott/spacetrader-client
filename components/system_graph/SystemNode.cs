using Godot;

[Tool]
public partial class SystemNode : GraphNode
{
	private Label _systemNameLabel;
	private HBoxContainer _shipsInSystemHBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_systemNameLabel = GetNode<Label>("%SystemNameLabel");
		_shipsInSystemHBox = GetNode<HBoxContainer>("%ShipsInSystemHBox");

		ItemRectChanged += CenterPivotOffset;
	}

	private void CenterPivotOffset()
	{
		var titleBarSize = GetTitlebarHBox().Size;
		PivotOffset = (Vector2.Down * titleBarSize + Size) / 2;
	}

	private static readonly Texture2D _shipGenericTexture = GD.Load<Texture2D>("res://textures/ship_generic.png");

	private void SetNumShips(int n)
	{
		foreach (var child in _shipsInSystemHBox.GetChildren())
		{
			_shipsInSystemHBox.RemoveChild(child);
			child.QueueFree();
		}
		for (int i = 0; i < n; i++)
		{
			var textureRect = new TextureRect
			{
				Texture = _shipGenericTexture
			};
			_shipsInSystemHBox.AddChild(textureRect);
		}
	}

	private static readonly PackedScene _systemNodeScene = GD.Load<PackedScene>("res://components/system_graph/system_node.tscn");


	public static void Add(Node parent, string name, Vector2 pos, int numShips)
	{
		var node = _systemNodeScene.Instantiate<SystemNode>();
		parent.AddChild(node);
		// _Ready has been called, we can now access child widgets
		node.PositionOffset = pos;
		node._systemNameLabel.Text = name;

		node.SetNumShips(numShips);
	}
}
