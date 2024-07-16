using Godot;
using System;

namespace Models;

[Serializable]
public partial class ServerStatusResource : Resource
{
	[Export]
	public string Version { get; set; }

	[Export]
	public string NextReset { get; set; }


	public byte[] ToBytes()
	{
		var arr = new Godot.Collections.Array<Variant>
			{
				Version, NextReset
			};
		return GD.VarToBytes(arr);
	}

	public static ServerStatusResource FromBytes(byte[] data)
	{
		var unpacked = GD.BytesToVar(data);
		var arr = unpacked.As<Godot.Collections.Array<Variant>>();
		var res = new ServerStatusResource
		{
			Version = arr[0].AsString(),
			NextReset = arr[1].AsString()
		};
		return res;
	}
}
