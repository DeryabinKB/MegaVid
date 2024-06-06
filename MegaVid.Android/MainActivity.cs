using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Rg.Plugins.Popup;
using Android.Runtime;
using Android.Views;

namespace MegaVid.Droid
{
    [Activity(Label = "MegaVid", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Must be called before base.OnCreate to hide the title bar
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            this.Window.RequestFeature(WindowFeatures.NoTitle);

            base.OnCreate(savedInstanceState);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Popup.Init(this);

            // Must be called after base.OnCreate
            this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.ImmersiveSticky);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
