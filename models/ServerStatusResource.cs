using Godot;
using System;
using System.IO;

namespace Models;

[Serializable]
public partial class ServerStatusResource : Resource, ISerializableModel
{
	[Export]
	public string Version { get; set; }

	[Export]
	public string NextReset { get; set; }

	public void Write(BinaryWriter writer)
	{
		writer.Write(Version);
		writer.Write(NextReset);
	}

	public void Read<T>(BinaryReader reader) where T : ISerializableModel, new()
	{
		Version = reader.ReadString();
		NextReset = reader.ReadString();
	}
}
