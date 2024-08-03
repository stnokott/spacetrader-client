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
	[BeforeTest]
	public void Setup()
	{
		// start with empty ship list
		Store.Instance.Ships.Clear();
	}

	[TestCase(0)]
	[TestCase(1)]
	[TestCase(99)]
	public async Task AddSystemNode(int numShipsInSystem)
	{
		// run scene
		ISceneRunner sceneRunner = ISceneRunner.Load("res://components/system_graph/system_graph.tscn");
		await sceneRunner.AwaitIdleFrame();

		// get scene root
		var root = sceneRunner.Scene() as SystemGraph;
		AssertObject(root).IsNotNull();

		IEnumerable<SystemNode> getSystemsInGraph()
		{
			var nodes = root._graph.GetChildren().Where((child) => child is SystemNode);
			return nodes.Select((node) => node as SystemNode);
		}
		IEnumerable<Node> getSystemNodeShipNodes(SystemNode node)
		{
			return node.GetShipsHBox().GetChildren();
		}

		// assert no nodes yet present
		AssertArray(getSystemsInGraph()).HasSize(0);

		// add ships to store
		for (int i = 0; i < numShipsInSystem; i++)
		{
			Store.Instance.Ships.Add(new GrpcSpacetrader.Ship
			{
				CurrentLocation = new GrpcSpacetrader.Ship.Types.Location
				{
					System = "SYS-WITH-SHIPS"
				}
			});
		}

		// add system node without ships
		root.AddSystemNode(new GrpcSpacetrader.System
		{
			Id = "SYS-NO-SHIPS",
			X = 100,
			Y = -200,
		});
		var systemNodes = getSystemsInGraph().ToList();
		// assert we now have n nodes
		AssertArray(systemNodes).HasSize(1);
		// assert we have no ships displayed (since there are no ships in the store)
		AssertArray(getSystemNodeShipNodes(systemNodes[0])).HasSize(0);

		// add system node which has ships
		root.AddSystemNode(new GrpcSpacetrader.System
		{
			Id = "SYS-WITH-SHIPS",
			X = 100,
			Y = -200,
		});
		systemNodes = getSystemsInGraph().ToList();
		AssertArray(systemNodes).HasSize(2);
		// assert we have ships displayed
		AssertArray(getSystemNodeShipNodes(systemNodes[1])).HasSize(numShipsInSystem);
	}
}
