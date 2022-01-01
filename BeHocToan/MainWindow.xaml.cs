using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
//using System.Text.Json;

namespace BeHocToan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random rd = new Random();
        private readonly LibVLC libvlc;
        private int CongDon = 0;
        private bool isCorrect;

        public MainWindow()
        {
            InitializeComponent();
            Core.Initialize();
            libvlc = new LibVLC(enableDebugLogs: true);
        }

        private async void TxtResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _ = int.TryParse(txt1.Text, out int n1);
                _ = int.TryParse(txt2.Text, out int n2);
                _ = int.TryParse(txtResult.Text, out int rs);
                int result = 0;
                if (txtOperator.Text == "+")
                {
                    result = n1 + n2;
                }
                else if (txtOperator.Text == "-")
                {
                    result = n1 - n2;
                }
                if (result == rs)
                {
                    isCorrect = true;
                    txtResult.Background = Brushes.LightGreen;
                    //btnNgauNhien.Focus();
                    imgRabbit.Source = new BitmapImage(new Uri("images/tho_cuoi.png", UriKind.Relative));
                    Uri uri = new Uri(@"pack://application:,,,/Sounds/Correct.wav", UriKind.RelativeOrAbsolute);

                    System.IO.Stream st = Application.GetResourceStream(uri).Stream;
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(st);
                    player.Play();

                    await Task.Delay(2000);
                    string[] loiKhen = new string[] { "Bạn làm tốt lắm !", "Đúng rồi !", "Hãy cố gắng nhé !", "Bạn giỏi lắm !" };
                    string khen = loiKhen[rd.Next(0, loiKhen.Length - 1)];
                    TextToSpeech(khen);
                    CongDon++;
                    if (CongDon >= 3)
                    {
                        await Task.Delay(4000);
                        await Task.Run(() =>
                        {
                            TextToSpeech($"Tốt lắm, bạn đã làm đúng {CongDon} phép toán liên tiếp, hãy cố gắng nhé !");
                        });
                        
                    }
                }
                else
                {
                    isCorrect = false;
                    txtResult.Background = Brushes.LightPink;
                    imgRabbit.Source = new BitmapImage(new Uri("images/tho_khoc.png", UriKind.Relative));

                    Uri uri = new Uri(@"pack://application:,,,/Sounds/Wrong.wav", UriKind.RelativeOrAbsolute);

                    await Task.Delay(2000);
                    string[] loiKhen = new string[] { "Chưa đúng rồi !", "Bạn hãy làm lại !", "Ố ồ chưa đúng rồi !", "Bạn ơi chưa đúng rồi !" };
                    string khen = loiKhen[rd.Next(0, loiKhen.Length - 1)];
                    TextToSpeech(khen);

                    System.IO.Stream st = Application.GetResourceStream(uri).Stream;
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(st);
                    player.Play();
                    CongDon = 0;
                }
            }
        }


        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            txtResult.Background = Brushes.White;
            imgRabbit.Source = new BitmapImage(new Uri("images/tho_bt.png", UriKind.Relative));

            string input = ((TextBox)sender).Text;

            _ = int.TryParse(input, out int res);

            if (((TextBox)sender).Name == "txt1")
            {
                panel1.Children.Clear();
                for (int i = 0; i < res; i++)
                {
                    panel1.Children.Add(new UcRabbitFace());
                }
            }
            else if (((TextBox)sender).Name == "txt2")
            {
                panel2.Children.Clear();
                for (int i = 0; i < res; i++)
                {
                    _ = panel2.Children.Add(new UcRabbitFace());
                }
            }
        }




        private void Txt1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _ = txtOperator.Focus();
            }
        }

        private void txtOperator_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _ = txt2.Focus();
            }
        }

        private void txt2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _ = txtResult.Focus();
            }
        }

        private void txtResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtResult.Background = Brushes.White;
            imgRabbit.Source = new BitmapImage(new Uri("images/tho_bt.png", UriKind.Relative));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //wplayer.URL = AppDomain.CurrentDomain.BaseDirectory + "Voices\\Chao.mp3";
            //wplayer.controls.play();

            //await Task.Delay(4000);
            //wplayer.URL = AppDomain.CurrentDomain.BaseDirectory + "Voices\\GioiThieu.mp3";
            //wplayer.controls.play();
            int hour = DateTime.Now.Hour;
            string buoi = "";
            if (0 < hour && hour < 12 )
            {
                buoi = "sáng";
            }
            else if (12 <= hour && hour < 18 )
            {
                buoi = "chiều";
            }
            if (hour >= 18)
            {
                buoi = "tối";
            }

            string msg = $"Chào buổi {buoi} bạn Gia Linh, hôm nay là {DateTime.Now.DayOfWeek.ToThuVN()}. Nào, mình cùng làm toán nhé. Bạn hãy chọn các nút bên dưới để chúng mình cùng bắt đầu";
            TextToSpeech(msg);
        }

        private void btnPhepCong_Click(object sender, RoutedEventArgs e)
        {
            if (txtResult.Text == "?")
            {
                txtResult.Focus();
                return;
            }
            if (txt1.Text.Length > 0 && txt2.Text.Length > 0)
            {
                if (!isCorrect)
                {
                    txtResult.Focus();
                    return;
                }
            }
            txtOperator.Text = "+";
            txtEqual.Text = "=";
            txtResult.Text = "?";
            _ = txtResult.Focus();
            txtResult.Background = Brushes.White;
            imgRabbit.Source = new BitmapImage(new Uri("images/tho_bt.png", UriKind.Relative));

            // Random
            int x1 = rd.Next(0, 19);
            txt1.Text = x1.ToString();
            int max = 20 - x1;
            int x2 = rd.Next(0, max);
            txt2.Text = x2.ToString();
            // Phát âm
            string str = txt1.Text + " cộng " + txt2.Text + " bằng bao nhiêu ?";
            TextToSpeech(str);
        }

        private void btnPhepTru_Click(object sender, RoutedEventArgs e)
        {
            if (txtResult.Text == "?")
            {
                txtResult.Focus();
                return;
            }
            if (txt1.Text.Length > 0 && txt2.Text.Length > 0)
            {
                if (!isCorrect)
                {
                    txtResult.Focus();
                    return;
                }
            }
            txtOperator.Text = "-";
            txtEqual.Text = "=";
            txtResult.Text = "?";
            _ = txtResult.Focus();
            txtResult.Background = Brushes.White;
            imgRabbit.Source = new BitmapImage(new Uri("images/tho_bt.png", UriKind.Relative));

            // Random
            int x1 = rd.Next(0, 20);
            txt1.Text = x1.ToString();

            int x2 = rd.Next(0, 20);
            while (x2 > x1)
            {
                x2 = rd.Next(0, 20);
            }
            txt2.Text = x2.ToString();
            // Phát âm
            string str = txt1.Text + " trừ " + txt2.Text + " bằng bao nhiêu ?";
            TextToSpeech(str);
        }

        private void TxtResult_PreviewKeyDown(object sender, TextCompositionEventArgs e)
        {

            if (txtResult.Text == "?")
            {
                txtResult.Text = string.Empty;
            }
            
            TextBox txtbox = (TextBox)sender;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(txtbox.Text);
            //var isValid = regex.IsMatch(txtbox.Text);
            //if (!isValid)
            //{
            //    txtResult.Text = string.Empty;
            //    e.Handled = false;
            //}
        }

        private void BtnNgauNhien_Click(object sender, RoutedEventArgs e)
        {
            RaDeNgauNhien();
        }

        private void RaDeNgauNhien()
        {
            if (txtResult.Text == "?")
            {
                txtResult.Focus();
                return;
            }
            if (txt1.Text.Length > 0 && txt2.Text.Length > 0)
            {
                if (!isCorrect)
                {
                    txtResult.Focus();
                    return;
                }
            }
            txtResult.Text = "?";
            _ = txtResult.Focus();
            txtResult.Background = Brushes.White;
            imgRabbit.Source = new BitmapImage(new Uri("images/tho_bt.png", UriKind.Relative));
            // random
            int phepToan = rd.Next(0, 2);
            string kyHieuPhepToan = phepToan == 0 ? "+" : "-";
            //Debug.Print(kyHieuPhepToan);
            txtOperator.Text = kyHieuPhepToan;
            txtEqual.Text = "=";
            if (kyHieuPhepToan == "+")
            {
                int x1 = rd.Next(0, 20);
                txt1.Text = x1.ToString();
                int max = 20 - x1;
                int x2 = rd.Next(0, max);
                txt2.Text = x2.ToString();
                // Phát âm
                string str = txt1.Text + " cộng " + txt2.Text + " bằng bao nhiêu ?";
                TextToSpeech(str);
            }
            else if (kyHieuPhepToan == "-")
            {
                int x1 = rd.Next(0, 20);
                txt1.Text = x1.ToString();
                int x2 = rd.Next(0, 20);
                while (x2 > x1)
                {
                    x2 = rd.Next(0, 20);
                }
                txt2.Text = x2.ToString();
                // Phát âm
                string str = txt1.Text + " trừ " + txt2.Text + " bằng bao nhiêu ?";
                TextToSpeech(str);
            }
        }


        #region Helper methods
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //public void PlayMp3FromUrl(string url)
        //{
        //    using (Stream ms = new MemoryStream())
        //    {
        //        using (Stream stream = WebRequest.Create(url)
        //            .GetResponse().GetResponseStream())
        //        {
        //            byte[] buffer = new byte[32768];
        //            int read;
        //            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                ms.Write(buffer, 0, read);
        //            }
        //        }

        //        ms.Position = 0;
        //        using (WaveStream blockAlignedStream =
        //            new BlockAlignReductionStream(
        //                WaveFormatConversionStream.CreatePcmStream(
        //                    new Mp3FileReader(ms))))
        //        {
        //            using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
        //            {
        //                waveOut.Init(blockAlignedStream);
        //                waveOut.Play();
        //                while (waveOut.PlaybackState == PlaybackState.Playing)
        //                {
        //                    System.Threading.Thread.Sleep(100);
        //                }
        //            }
        //        }
        //    }
        //}

        private string GetTTSUrl(string text)
        {
            string result = Task.Run(async () =>
            {
                string payload = text;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("api-key", "CDN2TIuOWGNhRyUI26JCogWHTHM0f177");
                client.DefaultRequestHeaders.Add("speed", "");
                client.DefaultRequestHeaders.Add("voice", "banmai");
                var response = await client.PostAsync("https://api.fpt.ai/hmi/tts/v5", new StringContent(payload));
                return await response.Content.ReadAsStringAsync();
            }).GetAwaiter().GetResult();
            Debug.Print(result);
            var b = result.Split(',')[0].Split(':'); 
            var kq = "https:" + b[2].Remove(b[2].Length-1);
            return kq;
        }

        private class WebResponse
        {
            public string async { get; set; }
            public string error { get; set; }
            public string message { get; set; }
            public string request_id { get; set; }


        }

        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                RaDeNgauNhien();
            }
        }

        private void TxtResult_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (txtResult.Text == "?")
            {
                txtResult.Text = string.Empty;
            }

            //TextBox txtbox = (TextBox)sender;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private async void TextToSpeech(string text)
        {
            string url = GetTTSUrl(text);
            await Task.Delay(1000);
            var media = new Media(libvlc, new Uri(url));
            var mediaplayer = new LibVLCSharp.Shared.MediaPlayer(media);
            mediaplayer.Play();

        }


        private void BtnPhepCong_MouseEnter(object sender, MouseEventArgs e)
        {
            TextToSpeech("Phép cộng");
        }

        private void BtnPhepTru_MouseEnter(object sender, MouseEventArgs e)
        {
            TextToSpeech("Phép trừ");
        }

        private void BtnNgauNhien_MouseEnter(object sender, MouseEventArgs e)
        {
            TextToSpeech("Ngẫu nhiên");
        }

    }


}
