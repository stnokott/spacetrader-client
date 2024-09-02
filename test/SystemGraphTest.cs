using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
public class SystemGraphTest
{
	[TestCase(0)]
	[TestCase(1)]
	[TestCase(99)]
	public async Task AddSystemNode(int numShipsInSystem)
	{
		// run scene
		ISceneRunner sceneRunner = ISceneRunner.Load("res://components/galaxy_view/galaxy.tscn");
		await sceneRunner.AwaitIdleFrame();

		// get scene root
		var root = sceneRunner.Scene() as Galaxy;
		AssertObject(root).IsNotNull();

		IEnumerable<GalaxySystem> getSystemsInGraph()
		{
			var nodes = root.GetChildren().Where((child) => child is GalaxySystem);
			return nodes.Select((node) => node as GalaxySystem);
		}
		bool isShipVisible(GalaxySystem node)
		{
			return node.GetNode<CanvasItem>("%ShipIcon").Visible;
		}

		root.ClearSystemNodes();
		// assert no nodes yet present
		AssertArray(getSystemsInGraph()).HasSize(0);

		// add system node without ships
		root.AddSystemNode(
			name: "SYS-NO-SHIPS",
			pos: new Vector2(100, -200),
			shipCount: 0,
			hasJumpgates: false
		);
		var systemNodes = getSystemsInGraph().ToList();
		// assert we now have n nodes
		AssertArray(systemNodes).HasSize(1);
		// assert we have no ship count displayed (since there are no ships in the store)
		AssertBool(isShipVisible(systemNodes[0])).IsFalse();

		// add system node which has ships
		root.AddSystemNode(
					name: "SYS-WITH-SHIPS",
			pos: new Vector2(100, -200),
			shipCount: numShipsInSystem,
			hasJumpgates: false
		);
		systemNodes = getSystemsInGraph().ToList();
		AssertArray(systemNodes).HasSize(2);
		// assert we have ships displayed
		AssertBool(isShipVisible(systemNodes[1])).Equals(numShipsInSystem > 0);
	}
}
