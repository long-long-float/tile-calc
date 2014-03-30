using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace button
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach(var i in Enumerable.Range(0, 1))
            {
                var secondaryTile = new SecondaryTile("calc-child" + i, i.ToString(), i.ToString(), new Uri("ms-appx:///Assets/Logo.scale-100.png"), TileSize.Square150x150);
                await secondaryTile.RequestCreateAsync();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var arg = (string)e.Parameter;

            if (arg != "")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("tile-calc://" + arg));

                Application.Current.Exit();
            }
        }
    }
}
