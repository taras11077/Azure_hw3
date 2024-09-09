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
string picturePath = @"C:\Users\Master\Desktop\Images2\image2.jpg";

var newUser = new User
{
	Name = "Mykola",
	Email = "vasyl@gmail.com",
	Picture = picturePath,
};

//await userService.AddAsync(newUser);


// оновлення
string rk = "18211faf-682d-4b27-8a57-c1ab27879e3d";

string newName = "Mykola";
string newEmail = "mykola@gmail.com";
string newPicture = @"C:\Users\Master\Desktop\Images2\image4.jpg";

await userService.UpdateAsync(rk, newName, newEmail, newPicture);

// видалення
await userService.DeleteAsync("User", "3010e71b-c0d3-4a6b-8ddc-ffb60761bfb6");


// показати всіх
await userService.DisplayAllAsync();