using Godot;
using System.Threading.Tasks;

public partial class SystemGraph : PanelContainer
{
	private GraphEdit _graph;
	private PanelContainer _loadingOverlay;
	private Label _loadingLabel;
	private Label _cursorPosLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OS.LowProcessorUsageMode = true;

		_graph = GetNode<GraphEdit>("GraphEdit");
		_cursorPosLabel = GetNode<Label>("%CursorPosLabel");
	}

	public override void _Process(double delta)
	{
		// display mouse position in graph as coordinates
		var localMousePos = _graph.GetLocalMousePosition();
		// only update when mouse is inside of graph rect
		if (localMousePos >= Vector2.Zero && localMousePos <= _graph.Size)
		{
			var graphMousePos = (localMousePos + _graph.ScrollOffset) / _graph.Zoom;
			_cursorPosLabel.Text = string.Format("{0:F0}, {1:F0}", graphMousePos.X, graphMousePos.Y);
		}
	}

	public async void OnGraphUpdateTimer()
	{
		await RefreshSystems(force: false);
	}

	// defines the scaling between pixels and galaxy coordinates
	// higher values allow a higher coordinate range to be displayed (effectively zooming out)
	private const float UNITS_PER_PIXEL = 2f;

	// number of pixels to prefetch in each direction of the viewport
	private const int PIXEL_PREFETCH = 100;

	private Rect2 _prevViewport = new();

	private async Task RefreshSystems(bool force = false)
	{
		var offsetPx = _graph.ScrollOffset; // offset is the top-left coordinate of the viewport
		offsetPx -= Vector2I.One * PIXEL_PREFETCH;
		var sizePx = _graph.Size;
		sizePx += Vector2I.One * PIXEL_PREFETCH * 2;
		var zoom = _graph.Zoom;
		var viewportOffset = offsetPx * UNITS_PER_PIXEL / zoom;
		var viewportSize = sizePx * UNITS_PER_PIXEL / zoom;
		var viewport = new Rect2I(
			new Vector2I((int)viewportOffset.X, (int)viewportOffset.Y),
			new Vector2I((int)viewportSize.X, (int)viewportSize.Y)
		);

		if (force || viewport != _prevViewport)
		{
			var systems = await Store.Instance.GetSystemsInRect(viewport.Position, viewport.End);
			ClearSystemNodes();
			foreach (var system in systems)
			{
				AddSystemNode(system);
			}
			_prevViewport = viewport;
		}
	}

	private void AddSystemNode(GrpcSpacetrader.System system)
	{
		SystemNode.Add(
			_graph,
			system.Id,
			new Vector2(system.X, system.Y) / UNITS_PER_PIXEL,
			Store.Instance.GetNumShipsInSystem(system)
		);
	}

	private void ClearSystemNodes()
	{
		foreach (var node in _graph.GetChildren())
		{
			if (node is GraphNode)
			{
				_graph.RemoveChild(node);
				node.QueueFree();
			}
		}
	}
}
