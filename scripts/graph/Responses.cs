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

public class SystemsResponse
{
	public SystemConnection Systems { get; set; }
}
