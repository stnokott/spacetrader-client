using System.Collections.Generic;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class StoreTest
{
	private GrpcSpacetrader.Ship MakeShipWithLocation(string loc)
	{
		return new GrpcSpacetrader.Ship
		{
			CurrentLocation = new GrpcSpacetrader.Ship.Types.Location
			{
				System = loc
			}
		};
	}

	[AfterTest]
	public void Teardown()
	{
		Store.Instance.Ships = new List<GrpcSpacetrader.Ship>();
	}

	[TestCase]
	public void GetNumShipsInSystem()
	{
		Store.Instance.Ships = new List<GrpcSpacetrader.Ship>{
			MakeShipWithLocation("ABC-123"),
			MakeShipWithLocation("DEF-456"),
			MakeShipWithLocation("GHI-789"),
			MakeShipWithLocation("DEF-456")
		};

		AssertInt(Store.Instance.GetNumShipsInSystem(new GrpcSpacetrader.System
		{
			Id = "ABC-123"
		})).Equals(1);
		AssertInt(Store.Instance.GetNumShipsInSystem(new GrpcSpacetrader.System
		{
			Id = "DEF-456"
		})).Equals(2);
		AssertInt(Store.Instance.GetNumShipsInSystem(new GrpcSpacetrader.System
		{
			Id = "GHI-789"
		})).Equals(1);
		AssertInt(Store.Instance.GetNumShipsInSystem(new GrpcSpacetrader.System
		{
			Id = "JKL-000"
		})).Equals(0);
	}
}
