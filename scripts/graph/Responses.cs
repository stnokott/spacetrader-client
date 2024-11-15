using System.Collections.Generic;

namespace GraphQLModels;

public class ServerResponse
{
	public Server Server { get; set; }
}
public class AgentResponse
{
	public Agent Agent { get; set; }
}

public class ShipsResponse
{
	public List<Ship> Ships { get; set; }
}

public class SystemCountResponse
{
	public long SystemCount { get; set; }
}

public class SystemSubscriptionResponse
{
	public System System  { get; set; }
}