using Godot;
using System;

[Serializable]
public partial class ShipResource : Resource
{
	[Export]
	public string Name { get; set; }

	[Export]
	public string Status { get; set; }


	public byte[] ToBytes()
	{
		var arr = new Godot.Collections.Array<Variant>
			{
				Name, Status
			};
		return GD.VarToBytes(arr);
	}

	public static ShipResource FromBytes(byte[] data)
	{
		var unpacked = GD.BytesToVar(data);
		var arr = unpacked.As<Godot.Collections.Array<Variant>>();
		var res = new ShipResource
		{
			Name = arr[0].AsString(),
			Status = arr[1].AsString()
		};
		return res;
	}
}
