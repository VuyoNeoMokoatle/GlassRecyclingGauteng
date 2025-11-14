using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Linq;

namespace Buyalot;

public partial class ViewProductsPage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseService _db;
    private User currentUser;

    public ViewProductsPage()
    {
        InitializeComponent();
        _db = new DatabaseService();
        BindingContext = this;
    }

    public ViewProductsPage(User user) : this()
    {
        currentUser = user;
        _ = LoadCartCountAsync();
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion

    #region Cart Properties
    private int cartCount;
    public int CartCount
    {
        get => cartCount;
        set
        {
            if (cartCount != value)
            {
                cartCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCartVisible));
            }
        }
    }

    public bool IsCartVisible => CartCount > 0;

    public IRelayCommand ViewCartCommand => new AsyncRelayCommand(ViewCartAsync);
    #endregion

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProductsAsync();
        await LoadCartCountAsync();
    }

    private async Task LoadProductsAsync()
    {
        // Load only products for the current user
        productsList.ItemsSource = await _db.GetProductsAsync(currentUser);
    }

    private async Task LoadCartCountAsync()
    {
        try
        {
            var cartItems = await _db.GetCartItemsAsync(currentUser);
            CartCount = cartItems.Count();
        }
        catch
        {
            CartCount = 0;
        }
    }

    private async Task ViewCartAsync()
    {
        if (currentUser != null)
            await Navigation.PushAsync(new ViewCartPage(currentUser));
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var product = (Product)button.CommandParameter;

        await _db.AddToCartAsync(currentUser, product);
        await LoadCartCountAsync(); // live update badge
        await DisplayAlert("Added", $"{product.Name} added to your cart!", "OK");
    }

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var product = (Product)button.CommandParameter;

        bool confirm = await DisplayAlert("Remove Product",
            $"Are you sure you want to delete '{product.Name}'?",
            "Yes", "No");

        if (confirm)
        {
            await _db.DeleteProductAsync(product);
            await LoadProductsAsync();
            await LoadCartCountAsync(); // update badge
            await DisplayAlert("Removed", $"{product.Name} deleted successfully.", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Back button
    }
}
