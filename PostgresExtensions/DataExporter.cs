using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace PostgresExtensions;

public interface IDataExporterFactory<T> where T : DataExporter
{
	Task<T> CreateAsync();
}

public record ColumnSchema(string Name, string Type, string DbType);

public abstract class DataExporter
{
	protected abstract IEnumerable<(string Name, IQueryable Query)> GetQueries();

	public async Task ExportAsync(Stream output, JsonSerializerOptions? jsonSerializerOptions = null)
	{		
		using var writer = new Utf8JsonWriter(output, new JsonWriterOptions { Indented = true });

		writer.WriteStartObject();

		foreach (var (name, query) in GetQueries())
		{
			writer.WritePropertyName(name);
			writer.WriteStartObject();

			using var cmd = query.CreateDbCommand();
			using var results = await cmd.ExecuteReaderAsync();
			
			// Extract schema information
			var schema = ExtractSchema(results);
			
			// Write schema section
			writer.WritePropertyName("schema");
			JsonSerializer.Serialize(writer, schema, jsonSerializerOptions);
			
			// Write data section
			writer.WritePropertyName("data");
			writer.WriteStartArray();
			
			while (await results.ReadAsync())
			{
				var row = new object[results.FieldCount];
				results.GetValues(row);
				JsonSerializer.Serialize(writer, row, jsonSerializerOptions);
			}
			
			writer.WriteEndArray();
			writer.WriteEndObject();
			
			await results.DisposeAsync();
			cmd.Dispose();
		}

		writer.WriteEndObject();
	}

	private static ColumnSchema[] ExtractSchema(IDataReader reader)
	{
		var schema = new ColumnSchema[reader.FieldCount];
		for (int i = 0; i < reader.FieldCount; i++)
		{
			var name = reader.GetName(i);
			var fieldType = reader.GetFieldType(i);
			var dataTypeName = reader.GetDataTypeName(i);
			
			schema[i] = new ColumnSchema(name, fieldType.FullName ?? fieldType.Name, dataTypeName);
		}
		return schema;
	}	
}
