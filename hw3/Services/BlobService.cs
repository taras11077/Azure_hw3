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
	public async Task AddBlob(BlobContainerClient container, string path)
	{
		var name = Path.GetFileName(path);
		BlobClient blobClient = container.GetBlobClient(name);

		if (!File.Exists(path))
		{
			throw new FileNotFoundException("file not found");
		}

		await blobClient.UploadAsync(path);
		Console.WriteLine($"Blob '{name}' was uploaded to Azure");
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

