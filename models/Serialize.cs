using System;
using System.IO;
using Godot;

namespace Models;

public interface ISerializableModel
{
	void Write(BinaryWriter writer);
	void Read<T>(BinaryReader reader) where T : ISerializableModel, new();
}

public static class Serialize
{
	public static byte[] ToBytes(ISerializableModel model)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8);
		model.Write(writer);
		return stream.ToArray();
	}

	public static T FromBytes<T>(byte[] data) where T : ISerializableModel, new()
	{
		using var stream = new MemoryStream(data);
		using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8);

		var instance = new T();
		instance.Read<T>(reader);
		return instance;
	}
}
