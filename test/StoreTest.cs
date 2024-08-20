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
		Store.Instance.Ships = new Dictionary<string, GrpcSpacetrader.Ship>();
	}

	[TestCase]
	public void GetNumShipsInSystem()
	{
		Store.Instance.Ships = new Dictionary<string, GrpcSpacetrader.Ship>{
			{"ABC-123", MakeShipWithLocation("ABC-123")},
			{"DEF-456", MakeShipWithLocation("DEF-456")},
			{"GHI-789", MakeShipWithLocation("GHI-789")}
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
