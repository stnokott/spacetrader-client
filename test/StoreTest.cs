using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using System.Linq;

[TestSuite]
public class ObjectStoreTest
{
	[TestCase]
	public void Basic()
	{
		var graph = new Graph(
			(string _) => { },
			(string _) => { }
		);
		AssertArray(graph.GetAllSystems()).IsEmpty();
		AssertArray(graph.GetAllShips()).IsEmpty();

		// add system
		var sys = new SystemModel
		{
			Name = "Foo",
			Pos = Vector2.One * 42,
			HasJumpgates = false,
		};
		graph.UpdateSystem(sys);
		// need to have 1 system
		AssertArray(graph.GetAllSystems()).HasSize(1);
		// the systems need to match
		AssertObject(graph.GetSystem(sys.Name)).Equals(sys);

		// add ship (in that system)
		var ship = new ShipModel
		{
			Name = "Bar",
			Status = GrpcSpacetrader.Ship.Types.FlightStatus.Docked,
			SystemName = "Foo",
			WaypointName = "Fuzz"
		};
		graph.UpdateShip(ship);
		// need to have 1 ship
		AssertArray(graph.GetAllShips()).HasSize(1);
		// the ships need to match
		AssertObject(graph.GetShip(ship.Name)).Equals(ship);
		// the ship reference in the system needs to match
		AssertThat(graph.GetSystem(ship.SystemName).Ships.Contains(ship)).IsTrue();
	}

	[TestCase]
	public void ShipMoves()
	{
		var graph = new Graph(
			(string _) => { },
			(string _) => { }
		);

		var sys1 = new SystemModel
		{
			Name = "Foo",
			Pos = Vector2.One * 42,
			HasJumpgates = false,
		};
		var sys2 = new SystemModel
		{
			Name = "Bar",
			Pos = Vector2.One * -20,
			HasJumpgates = false,
		};

		graph.UpdateSystem(sys1);
		graph.UpdateSystem(sys2);

		// create ship in first system
		var ship = new ShipModel
		{
			Name = "Enterprise",
			Status = GrpcSpacetrader.Ship.Types.FlightStatus.Docked,
			SystemName = sys1.Name,
			WaypointName = "Fuzz"
		};
		graph.UpdateShip(ship);

		// sys1 must contain ship
		AssertThat(graph.GetSystem(sys1.Name).Ships.Contains(ship)).IsTrue();
		// sys2 must not contain ship
		AssertThat(graph.GetSystem(sys2.Name).Ships.Contains(ship)).IsFalse();

		// move ship to second system
		ship.SystemName = sys2.Name;
		graph.UpdateShip(ship);

		// sys1 must not contain ship
		AssertThat(graph.GetSystem(sys1.Name).Ships.Contains(ship)).IsFalse();
		// sys2 must contain ship
		AssertThat(graph.GetSystem(sys2.Name).Ships.Contains(ship)).IsTrue();
	}

	[TestCase]
	public void Unordered()
	{
		var graph = new Graph(
			(string _) => { },
			(string _) => { }
		);

		// create ship with system that doesn't exist yet
		var ship = new ShipModel
		{
			Name = "Enterprise",
			Status = GrpcSpacetrader.Ship.Types.FlightStatus.InOrbit,
			SystemName = "Foo",
			WaypointName = "Fuzz"
		};

		graph.UpdateShip(ship);
		// should not be added to ship list since orphaned
		AssertArray(graph.GetAllShips()).IsEmpty();

		// add independent system
		var sys1 = new SystemModel
		{
			Name = "Bar",
			HasJumpgates = false,
			Pos = Vector2.One * 42
		};
		graph.UpdateSystem(sys1);
		// should still be orphaned
		AssertArray(graph.GetAllShips()).IsEmpty();

		// add system of orphaned ship
		var sys2 = new SystemModel
		{
			Name = "Foo",
			HasJumpgates = false,
			Pos = Vector2.One * -42
		};
		graph.UpdateSystem(sys2);
		// should not be orphaned anymore
		AssertArray(graph.GetAllShips()).HasSize(1);
	}
}
