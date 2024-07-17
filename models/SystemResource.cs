using Godot;
using System;
using System.IO;

namespace Models;

[Serializable]
public partial class SystemResource : Resource
{
	[Export]
	public string Name { get; set; }
	[Export]
	public Vector2I Pos { get; set; }
	[Export]
	public string Type { get; set; }
	[Export]
	public string Factions { get; set; }

	// TODO: remove once present in store
	public byte[] ToBytes()
	{
		var arr = new Godot.Collections.Array<Variant>
		{
			Name, Pos, Type, Factions
		};
		return GD.VarToBytes(arr);
	}

	public static SystemResource FromBytes(byte[] data)
	{
		var unpacked = GD.BytesToVar(data);
		var arr = unpacked.As<Godot.Collections.Array<Variant>>();
		var res = new SystemResource
		{
			Name = arr[0].AsString(),
			Pos = arr[1].AsVector2I(),
			Type = arr[2].AsString(),
			Factions = arr[3].AsString()
		};
		return res;
	}
}
