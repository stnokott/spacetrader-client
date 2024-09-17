using System;

namespace Models;


public sealed record ServerStatusModel
{
	public string Version;
	public DateTime NextReset;
}
