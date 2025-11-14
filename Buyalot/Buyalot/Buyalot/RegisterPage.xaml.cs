using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace Buyalot
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            registerButton.IsEnabled = false;
            loadingIndicator.IsVisible = true;
            loadingIndicator.IsRunning = true;

            try
            {
                if (string.IsNullOrWhiteSpace(firstNameEntry.Text) ||
                    string.IsNullOrWhiteSpace(lastNameEntry.Text) ||
                    string.IsNullOrWhiteSpace(emailEntry.Text) ||
                    string.IsNullOrWhiteSpace(passwordEntry.Text) ||
                    string.IsNullOrWhiteSpace(mobileEntry.Text) ||
                    string.IsNullOrWhiteSpace(addressEntry.Text))
                {
                    await ShowTopToast("⚠️ Please fill all fields.", Colors.Red);
                    return;
                }

                if (!IsValidEmail(emailEntry.Text))
                {
                    await ShowTopToast("⚠️ Enter a valid email address.", Colors.Red);
                    return;
                }

                if (!IsStrongPassword(passwordEntry.Text))
                {
                    await ShowTopToast("⚠️ Password must include upper & lowercase letters, numbers, and symbols.", Colors.Red);
                    return;
                }

                if (!Regex.IsMatch(mobileEntry.Text, @"^0\d{9}$"))
                {
                    await ShowTopToast("⚠️ Mobile must start with 0 and contain exactly 10 digits.", Colors.Red);
                    return;
                }

                var existingUser = await App.Database.GetUserByEmailAsync(emailEntry.Text);
                if (existingUser != null)
                {
                    await ShowTopToast("⚠️ Email already registered.", Colors.Red);
                    return;
                }

                var user = new User
                {
                    FirstName = firstNameEntry.Text,
                    LastName = lastNameEntry.Text,
                    Email = emailEntry.Text,
                    Password = passwordEntry.Text,
                    Mobile = mobileEntry.Text,
                    Address = addressEntry.Text
                };

                await App.Database.AddUserAsync(user);

                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;

                await ShowTopToast("✅ Registration successful! Redirecting to login...", Colors.Green);
                await Task.Delay(1000);
                await Navigation.PushAsync(new LoginPage(), false);
            }
            catch (Exception ex)
            {
                await ShowTopToast($"❌ Registration failed: {ex.Message}", Colors.Red);
            }
            finally
            {
                registerButton.IsEnabled = true;
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
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

        private async void OnGoToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(), false);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private bool IsStrongPassword(string password)
        {
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$";
            return Regex.IsMatch(password, pattern);
        }

        private void OnMobileTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                string digitsOnly = Regex.Replace(entry.Text ?? "", @"[^\d]", "");
                if (digitsOnly.Length > 10)
                    digitsOnly = digitsOnly.Substring(0, 10);
                if (entry.Text != digitsOnly)
                    entry.Text = digitsOnly;
            }
        }
    }
}
