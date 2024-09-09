using Azure.Storage.Blobs;

namespace hw3.Services;

public class BlobService
{
	private readonly string _connectionsString;

	public BlobService(string connectionsString)
	{
		_connectionsString = connectionsString;
	}

	// створення контейнера
	public async Task<BlobContainerClient> GetContainer(string name)
	{
		BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionsString);
		BlobContainerClient container = blobServiceClient.GetBlobContainerClient(name);
		await container.CreateIfNotExistsAsync();

		return container;
	}

	// створення блоба
	public async Task<string> AddBlob(BlobContainerClient container, string path)
	{
		var originalName = Path.GetFileNameWithoutExtension(path);  // ім'я файлу без розширення
		var extension = Path.GetExtension(path);  // розширення файлу

		// нове унікальне ім'я
		var uniqueName = $"{originalName}_{Guid.NewGuid()}{extension}";

		BlobClient blobClient = container.GetBlobClient(uniqueName);

		if (!File.Exists(path))
		{
			throw new FileNotFoundException("file not found");
		}

		await blobClient.UploadAsync(path);
		Console.WriteLine($"Blob '{uniqueName}' was uploaded to Azure");

		return uniqueName;
	}




	// показати всі блоби контейнера
	public async Task DisplayBlobs(BlobContainerClient container)
	{
		Console.WriteLine("Name\t\t\tLast Modified\t\tAccess tier\t\tSize");

		await foreach (var blob in container.GetBlobsAsync())
		{
			double size = blob.Properties.ContentLength!.Value / 1024.0;
			Console.WriteLine($"{blob.Name}\t\t{blob.Properties.LastModified!.Value.DateTime.ToShortTimeString()}\t\t\t{blob.Properties.AccessTier}\t\t\t{size.ToString("F2")} KiB");
		}
	}


	// видалення блоба
	public async Task DeleteBlob(BlobContainerClient container, string name)
	{
		BlobClient blobClient = container.GetBlobClient(name);
		await blobClient.DeleteIfExistsAsync();
	}

}

