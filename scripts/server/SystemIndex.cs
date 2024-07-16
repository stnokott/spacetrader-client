using Godot;
using System;

using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Linq;
using SpaceTradersApi.Client;
using System.Collections.Generic;
using System.Data;

namespace Server;

public static class SystemIndex
{

	public static async Task Update(SpaceTradersClient client, bool force = false)
	{
		await WithDBConn(async (conn) =>
		{
			await CreateSystemDatabase(conn);

			if (force || !await HasSystemIndex(conn))
			{
				await ReplaceSystemIndex(client, conn);
			}
		});
	}

	public static async IAsyncEnumerator<SystemResource> GetSystemsInRect(Rect2 rect)
	{
		using var connection = NewConn();
		await connection.OpenAsync();

		var select = connection.CreateCommand();
		select.CommandText = @"
			SELECT symbol, x, y, type, factions FROM systems
			WHERE TRUE
				AND x >= $x_min AND x <= $x_max
				AND y >= $y_min AND y <= $y_max
		";
		select.Parameters.AddWithValue("$x_min", rect.Position.X);
		select.Parameters.AddWithValue("$y_min", rect.Position.Y);
		select.Parameters.AddWithValue("$x_max", rect.End.X);
		select.Parameters.AddWithValue("$y_max", rect.End.Y);

		using var reader = await select.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			var res = new SystemResource
			{
				Name = reader.GetString(0),
				Pos = new Vector2I(reader.GetInt32(1), reader.GetInt32(2)),
				Type = reader.GetString(3),
				Factions = reader.GetString(4)
			};
			yield return res;
		}
	}

	private static SqliteConnection NewConn()
	{
		return new SqliteConnection("Data Source=systems.db");
	}

	private static async Task WithDBConn(Func<SqliteConnection, Task> f)
	{
		using var connection = NewConn();
		await connection.OpenAsync();

		await f(connection);
	}

	private static async Task CreateSystemDatabase(SqliteConnection conn)
	{
		// TODO: DB migration
		var createTableCmd = conn.CreateCommand();
		createTableCmd.CommandText =
		@"
			CREATE TABLE IF NOT EXISTS systems (
				symbol TEXT PRIMARY KEY UNIQUE,
				x INTEGER NOT NULL,
				y INTEGER NOT NULL,
				type TEXT NOT NULL,
				factions TEXT NOT NULL
			);

			CREATE INDEX IF NOT EXISTS system_x_pos_index ON systems(x);
			CREATE INDEX IF NOT EXISTS system_y_pos_index ON systems(y);
			CREATE INDEX IF NOT EXISTS system_type_index ON systems(type);
		";
		await createTableCmd.ExecuteNonQueryAsync();
	}

	private static async Task<bool> HasSystemIndex(SqliteConnection conn)
	{
		var command = conn.CreateCommand();
		command.CommandText = @"SELECT 1 FROM systems LIMIT 1";

		using var reader = await command.ExecuteReaderAsync();
		return reader.HasRows;
	}

	private static async Task ReplaceSystemIndex(SpaceTradersClient client, SqliteConnection conn)
	{
		GD.Print("replacing systems index");
		using var transaction = await conn.BeginTransactionAsync();

		var truncCmd = conn.CreateCommand();
		truncCmd.CommandText = @"DELETE FROM TABLE systems";
		await truncCmd.ExecuteNonQueryAsync();

		var insertCmd = conn.CreateCommand();
		insertCmd.CommandText =
		@"
			INSERT INTO systems VALUES (
				$symbol, $x, $y, $type, $factions
			);
		";

		var total = 10000;
		var n = 0;
		for (int page = 1; n < total; page++)
		{
			var systems = await client.Systems.GetAsync((q) =>
			{
				q.QueryParameters.Limit = 20;
				q.QueryParameters.Page = page;
			});

			foreach (var system in systems.Data)
			{
				/*
				var res = new SystemResource {
					Name = system.Symbol,
					X = system.X.Value,
					Y = system.Y.Value
				};
				Rpc(nameof(AddSystem), res.ToBytes());
				*/
				insertCmd.Parameters.Clear();
				insertCmd.Parameters.AddWithValue("$symbol", system.Symbol);
				insertCmd.Parameters.AddWithValue("$x", system.X);
				insertCmd.Parameters.AddWithValue("$y", system.Y);
				insertCmd.Parameters.AddWithValue("$type", system.Type.ToString());
				insertCmd.Parameters.AddWithValue("$factions", string.Join(",", system.Factions.Select((f) => f.Symbol)));
				await insertCmd.ExecuteNonQueryAsync();
			}

			total = systems.Meta.Total.Value;
			n += systems.Data.Count;
			GD.Print("querying: " + n + "/" + total);
		}

		GD.Print("committing...");
		await transaction.CommitAsync();
		GD.Print("commit done");
	}
}
