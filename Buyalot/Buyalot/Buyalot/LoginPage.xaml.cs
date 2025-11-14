using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace Buyalot;

public partial class LoginPage : ContentPage
{
    private bool isPasswordVisible = false;

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        loginButton.IsEnabled = false;
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;

        try
        {
            if (string.IsNullOrWhiteSpace(emailEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text))
            {
                await ShowTopToast("⚠️ Please enter email and password.", Colors.Red);
                return;
            }

            if (!IsValidEmail(emailEntry.Text))
            {
                await ShowTopToast("⚠️ Please enter a valid email address.", Colors.Red);
                return;
            }

            if (passwordEntry.Text.Length < 6)
            {
                await ShowTopToast("⚠️ Password must be at least 6 characters long.", Colors.Red);
                return;
            }

            await Task.Delay(500);

            var user = await App.Database.AuthenticateUserAsync(emailEntry.Text, passwordEntry.Text);

            if (user == null)
            {
                await ShowTopToast("❌ Incorrect email or password.", Colors.Red);
                return;
            }

            await ShowTopToast("✅ Login successful!", Colors.Green);
            await Task.Delay(1000);
            await Navigation.PushAsync(new Main(user));
        }
        catch (Exception ex)
        {
            await ShowTopToast($"❌ Login failed: {ex.Message}", Colors.Red);
        }
        finally
        {
            loginButton.IsEnabled = true;
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
        }
    }

    private async void OnGoToRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        passwordEntry.IsPassword = !isPasswordVisible;
        togglePasswordButton.Text = isPasswordVisible ? "Hide" : "Show";
    }

    private async Task ShowTopToast(string message, Color backgroundColor)
    {
        var toastFrame = new Frame
        {
            BackgroundColor = backgroundColor,
            CornerRadius = 25,
            Padding = new Thickness(15, 10),
            HasShadow = true,
            Opacity = 0.95,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(20, 60, 20, 0),
            Content = new Label
            {
                Text = message,
                TextColor = Colors.White,
                FontSize = 14,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            },
            TranslationY = -100,
            Scale = 0.95
        };

        if (this.Content is Layout mainLayout)
        {
            mainLayout.Children.Add(toastFrame);
            await toastFrame.TranslateTo(0, 0, 300, Easing.CubicOut);
            await toastFrame.ScaleTo(1, 200);
            await Task.Delay(1000);
            await toastFrame.FadeTo(0, 300);
            mainLayout.Children.Remove(toastFrame);
        }
    }
}
