using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Buyalot;

public partial class AddProductPage : ContentPage, INotifyPropertyChanged
{
       
        private readonly DatabaseService _db;
        private User currentUser;

        public ObservableCollection<Product> Products { get; set; }

        public AddProductPage(User user)
        {
            InitializeComponent();
            _db = new DatabaseService();
            currentUser = user;
            Products = new ObservableCollection<Product>();
            LoadProducts();
            BindingContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public IRelayCommand<Product> AddToCartCommand => new AsyncRelayCommand<Product>(AddToCartAsync);

        private void LoadProducts()
        {
            // Hardcoded product data with local image resources
            Products.Add(new Product { ProductId = 1, Name = "Eco Sport Glasses", ImageUrl = "Buyalot.Images.glasses.jpg", Price = 450 });
            Products.Add(new Product { ProductId = 2, Name = "Recycled Classic Shades", ImageUrl = "Buyalot.Images.glasses2.jpg", Price = 400 });
            Products.Add(new Product { ProductId = 3, Name = "Premium Cycling Glasses", ImageUrl = "Buyalot.Images.glasses3.jpg", Price = 550 });
        }

        private async Task AddToCartAsync(Product product)
        {
            try
            {
                // Implement logic to add product to the cart
                await _db.AddToCartAsync(currentUser, product);
                await DisplayAlert("Success", $"{product.Name} added to cart!", "OK");
            }
            catch
            {
                await DisplayAlert("Error", "Failed to add product to cart.", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }


