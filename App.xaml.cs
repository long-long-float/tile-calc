using System;
using Windows.UI.Xaml;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

// 空のアプリケーション テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234227 を参照してください

namespace tile_calc
{
    /// <summary>
    /// 既定の Application クラスに対してアプリケーション独自の動作を実装します。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 単一アプリケーション オブジェクトを初期化します。これは、実行される作成したコードの
        /// 最初の行であり、main() または WinMain() と論理的に等価です。
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            pushToast();

            Application.Current.Exit();
        }

        private void pushToast()
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            toastXml.GetElementsByTagName("text")[0].AppendChild(toastXml.CreateTextNode("Hello from toast!"));

            var toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async void createSecondaryTile()
        {
            var secondaryTile = new SecondaryTile("calc-child", "tile-calc", "hogehoge", new Uri("ms-appx:///Assets/Logo.scale-100.png"), TileSize.Square150x150);

            await secondaryTile.RequestCreateAsync();
        }
    }
}
