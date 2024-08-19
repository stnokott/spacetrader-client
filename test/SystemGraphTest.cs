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
		bool isShipCountVisible(GalaxySystem node)
		{
			return node.GetNode<CanvasItem>("%ShipCountContainer").Visible;
		}
		string getShipCountString(GalaxySystem node)
		{
			return node.GetNode<Label>("%ShipCountLabel").Text;
		}

		root.ClearSystemNodes();
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
		// assert we have no ship count displayed (since there are no ships in the store)
		AssertBool(isShipCountVisible(systemNodes[0])).IsFalse();

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
		AssertBool(isShipCountVisible(systemNodes[1])).Equals(numShipsInSystem > 0);
		// assert we have the correct ship count displayed
		AssertString(getShipCountString(systemNodes[1])).Equals(numShipsInSystem.ToString());
	}
}
