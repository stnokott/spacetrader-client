using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class SystemGraph : PanelContainer
{
	private GraphEdit _graph;
	private PanelContainer _loadingOverlay;
	private Label _loadingLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OS.LowProcessorUsageMode = true;
		_graph = GetNode<GraphEdit>("GraphEdit");
		_loadingOverlay = GetNode<PanelContainer>("LoadingOverlay");
		_loadingLabel = GetNode<Label>("LoadingOverlay/VBoxContainer/Label");
	}

	private void ShowLoadingOverlay(string text)
	{
		_loadingLabel.Text = text;
		_loadingOverlay.Show();
	}

	private void HideLoadingOverlay()
	{
		_loadingOverlay.Hide();
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
		/*
		var node = SystemNode.New(
			system.Id,
			new Vector2(system.X, system.Y) / UNITS_PER_PIXEL
		);
		_graph.AddChild(node);
		*/
		SystemNode.Add(_graph, system.Id, new Vector2(system.X, system.Y) / UNITS_PER_PIXEL);
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
