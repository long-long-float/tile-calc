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
            var buttons = Enumerable.Range(0, 2).Select(x => x.ToString())
                .Concat(new String[] { "+", "=" });

            foreach (var i in buttons)
            {
                var secondaryTile = new SecondaryTile("calc-child" + i, i, i, new Uri("ms-appx:///Assets/Logo.scale-100.png"), TileSize.Square150x150);
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

        private List<String> OPERATORS = new List<String>(){"+", "-", "*", "/"};
        private List<Func<int, int, int>> BEHAVIORS = new List<Func<int, int, int>>()
        {
            (l, r) => l + r, (l, r) => l + r, (l, r) => l * r, (l, r) => l / r
        };

        private void convertToRPN(List<String> tokens, List<String> rpn)
        {
            foreach(var ope in OPERATORS)
            {
                int index = tokens.IndexOf(ope);
                if (index != -1)
                {
                    convertToRPN(tokens.Take(index).ToList(), rpn);
                    convertToRPN(tokens.Skip(index + 1).ToList(), rpn);
                    rpn.Add(ope);

                    return;
                }
            }
            rpn.Add(tokens[0]);
        }

        private int calc(List<String> rpn)
        {
            var stack = new Stack<String>();
            foreach(var token in rpn)
            {
                int i;
                if((i = OPERATORS.IndexOf(token)) != -1)
                {
                    int r = int.Parse(stack.Pop()), l = int.Parse(stack.Pop());
                    stack.Push(BEHAVIORS[i](l, r).ToString());
                }
                else
                {
                    stack.Push(token);
                }
            }
            return int.Parse(stack.Pop());
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

                switch (arg)
                {
                    case "=":
                        List<String> rpn = new List<String>();
                        convertToRPN(items, rpn);
                        showToast(calc(rpn).ToString());
                        items.Clear();
                        break;
                    default:
                        break;
                }

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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await folder.GetFileAsync("state.json");
            }
            catch(FileNotFoundException)
            {
                return;
            }

            await file.DeleteAsync();
            showToast("スタックファイルを削除しました");
        }
    }
}
