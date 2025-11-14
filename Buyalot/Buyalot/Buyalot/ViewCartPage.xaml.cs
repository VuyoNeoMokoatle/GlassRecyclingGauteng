using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Buyalot;

public partial class ViewCartPage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseService _db;
    private User currentUser;

    public ViewCartPage()
    {
        InitializeComponent();
        _db = new DatabaseService();
        BindingContext = this;
    }

    public ViewCartPage(User user) : this()
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
        await LoadCartItemsAsync();
        await LoadCartCountAsync();
    }

    private async Task LoadCartItemsAsync()
    {
    
        var items = await _db.GetCartItemsAsync(currentUser);
        cartList.ItemsSource = items;
        UpdateTotal(items);
    }

    private void UpdateTotal(IEnumerable<CartItem> items)
    {
        decimal total = items.Sum(i => i.Price);
        totalLabel.Text = $"Total: R{total:F2}";
    }

    private async Task LoadCartCountAsync()
    {
        try
        {
            var items = await _db.GetCartItemsAsync(currentUser);
            CartCount = items.Count();
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

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (CartItem)button.CommandParameter;

        bool confirm = await DisplayAlert("Remove Item",
            $"Are you sure you want to remove '{item.Name}' from your cart?",
            "Yes", "No");

        if (confirm)
        {
            await _db.RemoveFromCartAsync(item);
            await LoadCartItemsAsync();
            await LoadCartCountAsync();
            await DisplayAlert("Removed", $"{item.Name} removed from cart.", "OK");
        }
    }

    private async void OnClearCartClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Clear Cart",
            "Are you sure you want to remove all items from your cart?",
            "Yes", "No");

        if (confirm)
        {
            await _db.ClearCartAsync(currentUser);
            await LoadCartItemsAsync();
            await LoadCartCountAsync();
            await DisplayAlert("Cleared", "Your cart is now empty.", "OK");
        }
    }

    private async void OnCheckoutClicked(object sender, EventArgs e)
    {
        var items = await _db.GetCartItemsAsync(currentUser);
        if (!items.Any())
        {
            await DisplayAlert("Empty Cart", "No items to checkout.", "OK");
            return;
        }

        decimal total = items.Sum(i => i.Price);
        bool confirm = await DisplayAlert("Confirm Checkout",
            $"Proceed to checkout? Total amount: R{total:F2}",
            "Yes", "No");

        if (confirm)
        {
            await _db.ClearCartAsync(currentUser);
            await LoadCartItemsAsync();
            await LoadCartCountAsync();
            await DisplayAlert("Thank You", "Your order has been placed successfully!", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
