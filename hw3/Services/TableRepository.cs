using Azure;
using Azure.Data.Tables;
using hw3.Models;

namespace hw3.Services;

public class TableRepository
{
	private readonly TableServiceClient _tableService;
	private readonly TableClient _table;

	public TableRepository(string connectionString, string tableName)
	{
		_tableService = new TableServiceClient(connectionString);
		_table = _tableService.GetTableClient(tableName);
		_table.CreateIfNotExists();
	}

	// додавання сутностей
	public async Task AddEntity(ITableEntity entity)
	{
		await _table.AddEntityAsync(entity);
		Console.WriteLine("Entity was added");
	}

	// Upsert - додавання з перевіркою їснування сутності по ключам
	public async Task UpsertEntity(ITableEntity entity)
	{
		await _table.UpsertEntityAsync(entity);
		Console.WriteLine("Entity was upserted");
	}


	// отримання сутностей
	public AsyncPageable<T> GetAll<T>() where T : class, ITableEntity, new() 
	{
		var pk = new T().PartitionKey;
		var pageable = _table.QueryAsync<T>(x => x.PartitionKey.Equals(pk));

		return pageable;
	}

	// відображення сутностей
	public async Task Display<T>() where T : class, ITableEntity, new()
	{
		await foreach (var entity in GetAll<T>())
		{
			string content = entity switch
			{
				User c => $"{c.Name}\t\t{c.Email}\t\t {c.Picture}",
				_ => "no data"
			};
			Console.WriteLine(content);
		}

	}

	// оновлення
	public async Task UpdateEntity(ITableEntity entity)
	{
		await _table.UpdateEntityAsync(entity, ETag.All);
		Console.WriteLine("Entity was updated");
	}

	// видалення
	public async Task DeleteEntity(string pk, string rk)
	{
		var response = _table.DeleteEntityAsync(pk, rk);
		Console.WriteLine("Entity was deleted");
	}

	// отримання за RowKey
	public async Task<T> GetByRowKey<T>(string rk) where T : class, ITableEntity, new()
	{
		var pk = new T().PartitionKey;
		var pageable = _table.QueryAsync<T>(x => x.PartitionKey.Equals(pk));

		return pageable.ToBlockingEnumerable().Single(x => x.RowKey == rk);

	}

}

