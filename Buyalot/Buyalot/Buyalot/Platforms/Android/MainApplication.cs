using Android.App;
using Android.Runtime;
using Microsoft.Maui.Handlers;
using Android.Graphics.Drawables;

namespace Buyalot
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            RemoveEntryUnderlines();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        private void RemoveEntryUnderlines()
        {
#if ANDROID
            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                if (handler.PlatformView is Android.Widget.EditText editText)
                {

                    editText.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
                    editText.SetPadding(20, editText.PaddingTop, 20, editText.PaddingBottom); // optional padding
                }
            });
#endif
        }
    }
}
