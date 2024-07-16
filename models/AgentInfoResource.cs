using Godot;
using System;

namespace Models;

[Serializable]
public partial class AgentInfoResource : Resource
{
	[Export]
	public string Name { get; set; }

	[Export]
	public long Credits { get; set; }


	public byte[] ToBytes()
	{
		var arr = new Godot.Collections.Array<Variant>
			{
				Name, Credits
			};
		return GD.VarToBytes(arr);
	}

	public static AgentInfoResource FromBytes(byte[] data)
	{
		var unpacked = GD.BytesToVar(data);
		var arr = unpacked.As<Godot.Collections.Array<Variant>>();
		var res = new AgentInfoResource
		{
			Name = arr[0].AsString(),
			Credits = arr[1].AsInt64()
		};
		return res;
	}
}
