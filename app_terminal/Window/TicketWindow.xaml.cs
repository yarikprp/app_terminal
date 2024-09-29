using QRCoder;  
using System.Drawing; 
using System.IO; 
using System.Windows;  
using System.Windows.Media.Imaging; 
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Media;

namespace app_terminal.Windows
{
    public partial class TicketWindow : Window
    {
        public string TicketInfo { get; set; }

        public TicketWindow(string ticketInfo)
        {
            InitializeComponent();
            TicketInfo = ticketInfo;

            GenerateQRCode(ticketInfo);
            DataContext = this;
        }

        private void GenerateQRCode(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20))                         
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                qrCodeImage.Save(memoryStream, ImageFormat.Png);
                                memoryStream.Position = 0; 

                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = memoryStream;
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze(); 

                                ImageQRCode.Source = bitmapImage;
                            }
                        }
                    }
                }
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintButton.Visibility = Visibility.Hidden;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(this.Content as Visual, "Печать Билета");
            }

            PrintButton.Visibility = Visibility.Visible;
        }
    }
}