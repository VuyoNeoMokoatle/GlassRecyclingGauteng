using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;

namespace Buyalot
{
    public partial class Main : ContentPage, INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private readonly User currentUser;
        int currentPosition = 0;
        int totalItems = 3;

        public Main(User user)
        {
            InitializeComponent();

            _db = new DatabaseService();
            currentUser = user;

            BindingContext = this;
            StartAutoScroll();
            WelcomeText = $"Welcome, {user.FirstName} {user.LastName}!";
            OnPropertyChanged(nameof(UserFullName));

            WeakReferenceMessenger.Default.Register<CartUpdatedMessage>(this, (r, m) =>
            {
                CartCount = m.NewCount;
            });

            _ = LoadUserStatsAsync();
        }

        private void StartAutoScroll()
        {
            this.Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                currentPosition++;
                if (currentPosition >= totalItems)
                    currentPosition = 0;

                imageCarousel.Position = currentPosition;

                return true;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string welcomeText;
        public string WelcomeText
        {
            get => welcomeText;
            set
            {
                if (welcomeText != value)
                {
                    welcomeText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UserFullName => $"{currentUser.FirstName} {currentUser.LastName}";

        private int totalAvailable;
        public int TotalAvailable
        {
            get => totalAvailable;
            set
            {
                if (totalAvailable != value)
                {
                    totalAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public IRelayCommand AddProductCommand => new AsyncRelayCommand(() =>
            NavigateWithAnimationAsync(() => new AddProductPage(currentUser)));

        public IRelayCommand ViewProductsCommand => new AsyncRelayCommand(() =>
            NavigateWithAnimationAsync(() => new ViewProductsPage(currentUser)));

        public IRelayCommand ViewCartCommand => new AsyncRelayCommand(() =>
            NavigateWithAnimationAsync(() => new ViewCartPage(currentUser)));

        public IRelayCommand LogoutCommand => new AsyncRelayCommand(LogoutAsync);

        private async Task LoadUserStatsAsync()
        {
            try
            {
                var availableProducts = await _db.GetAvailableProductsAsync(currentUser);
                var cartItems = await _db.GetCartItemsAsync(currentUser);

                TotalAvailable = availableProducts.Count();
                CartCount = cartItems.Count();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load stats: {ex.Message}", "OK");
            }
        }

        private async Task NavigateWithAnimationAsync(Func<ContentPage> pageFactory)
        {
            try
            {
                var page = pageFactory();
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", ex.Message, "OK");
            }
        }

        private async Task LogoutAsync()
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");
            if (confirm)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserStatsAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            WeakReferenceMessenger.Default.Unregister<CartUpdatedMessage>(this);
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void OnProfileImageTapped(object sender, EventArgs e)
        {
            var user = currentUser;

            var overlay = new Grid
            {
                BackgroundColor = Colors.Black.MultiplyAlpha(0.5f),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                InputTransparent = false,
                Opacity = 0
            };

            var card = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 25f,
                HasShadow = true,
                Margin = 10f,
                Padding = new Thickness(20),
                WidthRequest = 320f,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Scale = 0.8f,
                TranslationY = -50f,
                Content = new VerticalStackLayout
                {
                    Spacing = 12,
                    Children =
                    {
                        new Label { Text = $"{user.FirstName} {user.LastName}", FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.Black },
                        new BoxView { HeightRequest = 1, BackgroundColor = Colors.Gray.MultiplyAlpha(0.3f) },
                        new Label { Text = $"Email: {user.Email}", FontSize = 15, TextColor = Colors.Black },
                        new Label { Text = $"Mobile: {user.Mobile}", FontSize = 15, TextColor = Colors.Black },
                        new Label { Text = $"Address: {user.Address}", FontSize = 15, TextColor = Colors.Black },
                    }
                }
            };

            overlay.Children.Add(card);

            if (this.Content is Layout mainLayout)
            {
                mainLayout.Children.Add(overlay);

                await overlay.FadeTo(1, 250, Easing.CubicIn);

                await Task.WhenAll(
                    card.TranslateTo(0, 0, 300, Easing.CubicOut),
                    card.ScaleTo(1.05f, 300, Easing.CubicOut)
                );
                await card.ScaleTo(1f, 100, Easing.CubicIn);

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, ev) =>
                {
                    await Task.WhenAll(
                        card.ScaleTo(0.9f, 150, Easing.CubicIn),
                        card.FadeTo(0, 150),
                        overlay.FadeTo(0, 150)
                    );
                    mainLayout.Children.Remove(overlay);
                };

                overlay.GestureRecognizers.Add(tapGesture);

                card.GestureRecognizers.Add(new TapGestureRecognizer());
            }
        }
    }

    public class CartUpdatedMessage
    {
        public int NewCount { get; }
        public CartUpdatedMessage(int count) => NewCount = count;
    }
}
