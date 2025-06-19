using System.IO.Compression;

namespace PostgresExtensions;

public static class ExporterExtensions
{
	public static async Task ExportZipAsync(
		this DataExporter exporter,
		Func<Stream, Task> executeWithStream,
		string? entryName = null)
	{
		entryName ??= "my-data.json";		

		using var download = new MemoryStream();

		{
			using var zipFile = new ZipArchive(download, ZipArchiveMode.Create, leaveOpen: true);
			var entry = zipFile.CreateEntry(entryName);
			using var entryStream = entry.Open();
			await exporter.ExportAsync(entryStream);
		}

		download.Position = 0;
		await executeWithStream(download);
	}
}
