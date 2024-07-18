using System;
using System.Collections.Generic;
using System.Threading;
using Godot;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using NSubstitute;
using SpaceTradersApi.Client;
using SpaceTradersApi.Client.Models;
using SpaceTradersApi.Client.My.Agent;
using SpaceTradersApi.Client.My.Ships;
using SpaceTradersApi.Client.Systems;

namespace Server;

public class ServerMock
{
	public static readonly GetResponse ServerStatus = new()
	{
		Version = "v1.2.3",
		ServerResets = new GetResponse_serverResets
		{
			Next = DateTime.Now.ToString()
		}
	};

	public static readonly AgentGetResponse AgentInfo = new()
	{
		Data = new Agent
		{
			Symbol = "MYAGENT",
			Credits = 123_456_789
		}
	};

	public static readonly ShipsGetResponse Fleet = new()
	{
		Data = new List<Ship>{
			new() {
				Symbol = "ENTERPRISE",
				Nav = new ShipNav{
					Status = ShipNavStatus.DOCKED
				}
			},
			new() {
				Symbol = "SILENT_RUNNING",
				Nav = new ShipNav{
					Status = ShipNavStatus.IN_ORBIT
				}
			}
		}
	};

	public static readonly SystemsGetResponse Systems = new()
	{
		Data = new List<SpaceTradersApi.Client.Models.System>{
			new() {
				Symbol = "ABC-123",
				X = 0, Y = 0,
				Type = SystemType.NEBULA,
				Factions = new List<SystemFaction>()
			}
		},
		Meta = new Meta
		{
			Total = 1,
			Page = 1
		}
	};

	public static IRequestAdapter NewAdapter()
	{
		var adapter = Substitute.For<IRequestAdapter>();

		adapter.SendAsync(
			Arg.Is<RequestInformation>(req => req.URI.AbsolutePath == "/v2"),
			Arg.Any<ParsableFactory<GetResponse>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>()
		).Returns(ServerStatus);

		adapter.SendAsync(
			Arg.Is<RequestInformation>(req => req.URI.AbsolutePath == "/v2/my/agent"),
			Arg.Any<ParsableFactory<AgentGetResponse>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>()
		).Returns(AgentInfo);

		adapter.SendAsync(
			Arg.Is<RequestInformation>(req => req.URI.AbsolutePath == "/v2/my/ships"),
			Arg.Any<ParsableFactory<ShipsGetResponse>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>()
		).Returns(Fleet);

		adapter.SendAsync(
			Arg.Is<RequestInformation>(req => req.URI.AbsolutePath == "/v2/systems"),
			Arg.Any<ParsableFactory<SystemsGetResponse>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>()
		).Returns(Systems);

		return adapter;
	}

	private static void PrintDict(IDictionary<string, object> d)
	{
		GD.Print("printing:");
		foreach (KeyValuePair<string, object> kv in d)
		{
			GD.Print("k=" + kv.Key + " v=" + kv.Value);
		}
	}
}
