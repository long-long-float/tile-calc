using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
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
            foreach (var i in Enumerable.Range(0, 1))
            {
                var secondaryTile = new SecondaryTile("calc-child" + i, i.ToString(), i.ToString(), new Uri("ms-appx:///Assets/Logo.scale-100.png"), TileSize.Square150x150);
                await secondaryTile.RequestCreateAsync();
            }
        }

        private void showToast(String msg)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            toastXml.GetElementsByTagName("text")[0].AppendChild(toastXml.CreateTextNode(msg));

            var toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var arg = (string)e.Parameter;

            if (arg != "")
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync("state.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
                
                string content;
                using (var stream = (await file.OpenAsync(Windows.Storage.FileAccessMode.Read)).AsStream())
                using (var reader = new StreamReader(stream))
                {
                    content = await reader.ReadToEndAsync();
                }

                List<String> items;
                try
                {
                    items = JsonArray.Parse(content)
                        .Select(item => item.GetString())
                        .ToList();
                }
                catch (System.Exception) //json parse error
                {
                    items = new List<String>();
                }

                items.Add(arg);

                showToast(String.Join(", ", items));

                JsonArray jsonAry = new JsonArray();
                foreach (var item in items) jsonAry.Add(JsonValue.CreateStringValue(item));

                using (var stream = (await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)).AsStream())
                using(var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(jsonAry.Stringify());
                }

                Application.Current.Exit();
            }
        }
    }
}
