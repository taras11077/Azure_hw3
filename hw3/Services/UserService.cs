using hw3.Models;

namespace hw3.Services;

public class UserService
{
	private readonly TableRepository _tableRepository;
	private readonly BlobService _blobService;
	private readonly string _blobContainerName = "pictures";

	public UserService(TableRepository tableRepository, BlobService blobService)
	{
		_tableRepository = tableRepository;
		_blobService = blobService;
	}



	// Додавання користувача
	public async Task AddAsync(User user)
	{
		var container = await _blobService.GetContainer(_blobContainerName);

		// завантаження зображення до Blob Storage
		await _blobService.AddBlob(container, user.Picture);

		// формування URL зображення
		var pictureName = Path.GetFileName(user.Picture);
		user.Picture = container.GetBlobClient(pictureName).Uri.ToString();

		// додавання користувача до Table Storage
		await _tableRepository.AddEntity(user);
	}


	// Отримання всіх користувачів
	public async Task DisplayAllAsync()
	{
		await _tableRepository.Display<User>();
	}


	// Отримання користувача за його RowKey
	public async Task<User?> GetUserByRowKeyAsync(string rowKey)
	{
		return await _tableRepository.GetByRowKey<User>(rowKey);
	}



	// Оновлення користувача
	public async Task UpdateAsync(string rowKey, string? newName = null, string? newEmail = null, string? newPicturePath = null)
	{
		var user = await GetUserByRowKeyAsync(rowKey);

		if (user == null)
		{
			throw new Exception("User not found.");
		}

		bool isUpdated = false;

		if (!string.IsNullOrEmpty(newName) && user.Name != newName)
		{
			user.Name = newName;
			isUpdated = true;
		}


		if (!string.IsNullOrEmpty(newEmail) && user.Email != newEmail)
		{
			user.Email = newEmail;
			isUpdated = true;
		}

		// оновлення зображення, якщо передано новий шлях до зображення
		if (!string.IsNullOrEmpty(newPicturePath))
		{
			var container = await _blobService.GetContainer(_blobContainerName);

			// завантаження нового зображення до Blob Storage та отримання нового унікального імені блоба
			var uniqueBlobName = await _blobService.AddBlob(container, newPicturePath);

			// оновлення URL для нового зображення, якщо воно змінилося
			var newPictureUrl = container.GetBlobClient(uniqueBlobName).Uri.ToString();
			if (user.Picture != newPictureUrl)
			{
				user.Picture = newPictureUrl;
				isUpdated = true;
			}
		}

		if (isUpdated)
			await _tableRepository.UpdateEntity(user);
	}



	// видалення користувача
	public async Task DeleteAsync(string partitionKey, string rowKey)
	{
		var user = await _tableRepository.GetByRowKey<User>(rowKey);

		if (user != null)
		{
			var container = await _blobService.GetContainer(_blobContainerName);

			// видалення зображення з Blob Storage
			await _blobService.DeleteBlob(container, Path.GetFileName(user.Picture));

			// видалення користувача з Table Storage
			await _tableRepository.DeleteEntity(partitionKey, rowKey);
		}
	}
}



