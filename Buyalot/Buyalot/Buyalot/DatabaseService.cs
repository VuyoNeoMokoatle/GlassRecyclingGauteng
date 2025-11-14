using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

namespace Buyalot
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;
        private string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "buyalot1.db");
            _db = new SQLiteAsyncConnection(_dbPath);
        }

        public async Task InitAsync()
        {
            try
            {
                await InitializeTablesAsync();
            }
            catch (SQLiteException ex)
            {
                if (ex.Message.Contains("PRIMARY KEY") || ex.Message.Contains("duplicate column name"))
                {
                    if (File.Exists(_dbPath))
                        File.Delete(_dbPath);

                    _db = new SQLiteAsyncConnection(_dbPath);
                    await InitializeTablesAsync();

                    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            await Application.Current.MainPage.DisplayAlert(
                                "Database Reset",
                                "Schema conflict detected. The local database was automatically recreated.",
                                "OK"
                            );
                        }
                    });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task InitializeTablesAsync()
        {
            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Product>();
            await _db.CreateTableAsync<CartItem>();
        }

        public Task<int> AddUserAsync(User user) => _db.InsertAsync(user);

        public Task<User?> GetUserByEmailAsync(string email) =>
            _db.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();

        public Task<User?> AuthenticateUserAsync(string email, string password) =>
            _db.Table<User>()
               .Where(u => u.Email == email && u.Password == password)
               .FirstOrDefaultAsync();

        public Task<int> AddProductAsync(Product product)
        {
            return _db.InsertAsync(product);
        }

        public Task<List<Product>> GetProductsAsync(User user)
        {
            return _db.Table<Product>()
                      .Where(p => p.UserId == user.UserId)
                      .ToListAsync();
        }

        public Task<List<Product>> GetAvailableProductsAsync(User user)
        {
            return _db.Table<Product>()
                      .Where(p => p.UserId == user.UserId)
                      .ToListAsync();
        }

        public Task<int> DeleteProductAsync(Product product) => _db.DeleteAsync(product);

    
        public Task<int> AddToCartAsync(User user, Product product)
        {
            var cartItem = new CartItem
            {
                UserId = user.UserId,
                ProductId = product.ProductId,
                Name = product.Name,
                Type = product.Type,
                Price = product.Price
            };
            return _db.InsertAsync(cartItem);
        }

        public Task<List<CartItem>> GetCartItemsAsync(User user) =>
            _db.Table<CartItem>().Where(c => c.UserId == user.UserId).ToListAsync();

        public Task<int> RemoveFromCartAsync(CartItem item) => _db.DeleteAsync(item);

        public Task<int> ClearCartAsync(User user) =>
            _db.Table<CartItem>().Where(c => c.UserId == user.UserId).DeleteAsync();
    }
}
