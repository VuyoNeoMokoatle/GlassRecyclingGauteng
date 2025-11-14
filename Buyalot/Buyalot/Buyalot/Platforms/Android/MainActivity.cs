using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Buyalot
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize |
                               ConfigChanges.Orientation |
                               ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout |
                               ConfigChanges.SmallestScreenSize |
                               ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var window = Window;
            if (window != null)
            {
                // Fully qualify Android.Graphics.Color
                window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#ffffff"));
            }
        }
    }
}
