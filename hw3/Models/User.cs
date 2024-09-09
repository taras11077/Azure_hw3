using Azure;
using Azure.Data.Tables;

namespace hw3.Models;

public class User : ITableEntity
{
	public User()
	{
		PartitionKey = nameof(User);
		RowKey = Guid.NewGuid().ToString();
		Timestamp = DateTimeOffset.UtcNow;
		ETag = new ETag();
	}


	public string Name { get; set; }
	public string Email { get; set; }
	public string Picture { get; set; }  // URL зображення

	public string PartitionKey { get; set; } // Discriminator
	public string RowKey { get; set; } // Id
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}

