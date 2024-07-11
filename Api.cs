using Godot;
using System;

using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

using SpaceTradersApi;
using SpaceTradersApi.Client;
using SpaceTradersApi.Client.Models;
using System.Threading.Tasks;

// using Kiota for client generation from OpenAPI.
// see https://learn.microsoft.com/en-us/openapi/kiota/quickstarts/dotnet

namespace Api
{
	public class Client
	{

		private readonly SpaceTradersClient _client;

		public Client()
		{
			var authProvider = new AnonymousAuthenticationProvider();
			var adapter = new HttpClientRequestAdapter(authProvider);
			_client = new SpaceTradersClient(adapter);
		}

		public async Task<ServerStatus> GetServerStatus()
		{
			var serverStatus = await _client.GetAsync();
			return new(serverStatus.Version, serverStatus.ServerResets.Next);
		}
	}

	public record ServerStatus(string Version, string NextResetTs);
}
