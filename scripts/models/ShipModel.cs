using Godot;

using FlightStatus = GrpcSpacetrader.Ship.Types.FlightStatus;

public struct ShipModel
{
	public string Name;

	public FlightStatus Status;

	public string SystemName;
	public string WaypointName;
}
