using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.Widget;

namespace PwdGen
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private static byte BackPressCounter;
        public override bool DispatchKeyEvent(KeyEvent? e)
        {
            // 禁用返回键，点击两次退出
            if (e?.KeyCode == Keycode.Back)
                if (e.Action == KeyEventActions.Down)
                {
                    if (BackPressCounter == 1)
                        App.QuitMethod!();
                    else
                    {
                        BackPressCounter++;
                        Toast.MakeText(Android.App.Application.Context, "再按一次退出", ToastLength.Long)!.Show();
                        Task.Run(async () =>
                        {
                            await Task.Delay(2000);
                            BackPressCounter = 0;
                        });
                    }
                    return false;
                }
            return base.DispatchKeyEvent(e);
        }
    }
}
