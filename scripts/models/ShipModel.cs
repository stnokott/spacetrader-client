using Godot;

using FlightStatus = GrpcSpacetrader.Ship.Types.FlightStatus;

public sealed record ShipModel
{
	public string Name;

	public FlightStatus Status;

	public string SystemName;
	public string WaypointName;
}
