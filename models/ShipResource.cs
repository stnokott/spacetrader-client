using Godot;
using System;
using System.IO;

namespace Models;

[Serializable]
public partial class ShipResource : Resource, ISerializableModel
{
	[Export]
	public string Name { get; set; }

	[Export]
	public string Status { get; set; }

	public void Write(BinaryWriter writer)
	{
		writer.Write(Name);
		writer.Write(Status);
	}

	public void Read<T>(BinaryReader reader) where T : ISerializableModel, new()
	{
		Name = reader.ReadString();
		Status = reader.ReadString();
	}
}
