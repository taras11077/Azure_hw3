using hw3;
using hw3.Models;
using hw3.Services;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
	.AddJsonFile("config.json")
	.Build();

string connectionString = config.GetConnectionString("Default") ?? throw new NullReferenceException("Connection string not found");

var tableRepository = new TableRepository(connectionString, "UserTable");
var blobService = new BlobService(connectionString);

var userService = new UserService(tableRepository, blobService);


// додавання
string picturePath = @"C:\Users\Master\Desktop\Images2\image3.jpg";

var newUser = new User
{
	Name = "Vasyl",
	Email = "vasyl@gmail.com",
	Picture = picturePath,
};

await userService.AddAsync(newUser);


// оновлення
string rk = "b37f9784-9c95-47d8-a102-212cb1979346";
string newPicture = @"C:\Users\Master\Desktop\Images2\image4.jpg";
await userService.UpdateAsync(rk, newPicture);

// видалення
//await userService.DeleteAsync("User", "40cdbc53-2aee-45a3-914b-3806900ebfc5");


// показати всіх
await userService.DisplayAllAsync();