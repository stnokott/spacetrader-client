using Godot;

using System.IO;

namespace Models;

public partial class AgentInfoResource : Resource, ISerializableModel
{
	[Export]
	public string Name { get; set; }

	[Export]
	public long Credits { get; set; }

	public void Write(BinaryWriter writer)
	{
		writer.Write(Name);
		writer.Write(Credits);
	}

	public void Read<T>(BinaryReader reader) where T : ISerializableModel, new()
	{
		Name = reader.ReadString();
		Credits = reader.ReadInt64();
	}
}
