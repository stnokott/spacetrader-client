using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

	private static readonly Paginated<SpaceTradersApi.Client.Models.System> SystemsPaginated = new(
		GenerateSystems(100, -2000, 2000)
	);

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
		).Returns(x =>
		{
			var req = x.ArgAt<RequestInformation>(0);
			var qp = req.QueryParameters;
			var (items, meta) = SystemsPaginated.Page((int)qp["page"], (int)qp["limit"]);
			return new SystemsGetResponse
			{
				Data = items,
				Meta = meta
			};
		});

		return adapter;
	}

	private static List<SpaceTradersApi.Client.Models.System> GenerateSystems(int count, int coordMin, int coordMax)
	{
		List<SpaceTradersApi.Client.Models.System> systems = new(count);
		Random rnd = new();
		for (var i = 0; i < count; i++)
		{
			var systemTypes = Enum.GetValues(typeof(SystemType));
			var systemFactions = Enum.GetValues(typeof(FactionSymbol));
			var system = new SpaceTradersApi.Client.Models.System
			{
				Symbol = GenerateSystemName(count, i),
				X = rnd.Next(coordMin, coordMax),
				Y = rnd.Next(coordMin, coordMax),
				Type = (SystemType)systemTypes.GetValue(rnd.Next(systemTypes.Length)),
				Factions = new List<SystemFaction>(systemFactions.OfType<SystemFaction>().ToList())
			};
			systems.Add(system);
		}
		return systems;
	}

	private static string GenerateSystemName(int totalSystems, int i)
	{
		var range = 'Z' - 'A' + 1;
		var nameLength = (int)Math.Ceiling((double)totalSystems / range);
		var s = new StringBuilder(nameLength);
		for (int j = 0; j < nameLength; j++)
		{
			s.Append((char)('A' + (i % range)));
			i /= range;
		}
		return s.ToString();
	}

	private class Paginated<T>
	{
		private readonly List<T> Items;

		public Paginated(List<T> items)
		{
			Items = items;
		}

		public (List<T>, Meta) Page(int page, int limit)
		{
			var l = Items.Chunk(limit).ElementAtOrDefault(page - 1).ToList();
			var meta = new Meta
			{
				Limit = limit,
				Page = page,
				Total = Items.Count
			};
			return (l, meta);
		}
	}
}
