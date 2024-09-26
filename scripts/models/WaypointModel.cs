using System.Collections.Generic;

namespace Models;

public struct WaypointModel
{
	public string Name;
	/// <summary>
	/// List of Waypoint names connected via jumpgate
	/// </summary>
	public IReadOnlyList<string> ConnectedWaypoints;
}
