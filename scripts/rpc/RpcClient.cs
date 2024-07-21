using System;
using Godot;
using GrpcSpacetrader;

namespace Rpc;
public class Client : Spacetrader.SpacetraderClient, IDisposable
{
	private readonly Grpc.Net.Client.GrpcChannel _channel;

	public Client(string host, int port) : base(MakeChannel(host, port, out Grpc.Net.Client.GrpcChannel channel))
	{
		_channel = channel;
	}

	private static Grpc.Net.Client.GrpcChannel MakeChannel(string host, int port, out Grpc.Net.Client.GrpcChannel channel)
	{
		channel = Grpc.Net.Client.GrpcChannel.ForAddress(string.Format("http://{0}:{1}", host, port));
		return channel;
	}

	public void Dispose()
	{
		_channel.Dispose();
		GC.SuppressFinalize(this);
	}
}
