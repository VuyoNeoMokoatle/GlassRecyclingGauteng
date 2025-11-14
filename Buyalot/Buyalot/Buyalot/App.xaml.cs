using Buyalot;
using Microsoft.Maui.Controls;

namespace Buyalot
{
    public partial class App : Application
    {
        public static DatabaseService? Database { get; private set; }

        public App()
        {
            InitializeComponent();

            Database = new DatabaseService();

            // Initialize tables asynchronously
            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Database != null)
                    await Database.InitAsync();
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }

}