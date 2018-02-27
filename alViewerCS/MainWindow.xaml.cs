using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Interop;
using alAVCapDllSharp;

namespace alViewerCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables

        static Boolean stopCaptureLoop;
        static AutoResetEvent capEvent = null;
        private delegate void RefreshDelegate(BitmapImage bmpSrc);

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeVar();
        }

        private void InitializeVar()
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeVideoInput();
        }

        private void InitializeVideoInput()
        {
            //create a bitmap to hold the data
            stopCaptureLoop = false;
            capEvent = new AutoResetEvent(false);
            Thread thread = new Thread(CaptureVideoLoop);
            thread.TrySetApartmentState(ApartmentState.MTA); //Because  it call COM 
            thread.Start((new WindowInteropHelper(this)).Handle);
        }

        private void CaptureVideoLoop(object data)
        {
            Capture CapDev = new Capture();

            if (!CapDev.Init((IntPtr)data, 0, 0)) {
                System.Windows.MessageBox.Show("Init device failed!");
                return;
            }

            // Initial bmp
            int width = CapDev.GetWidth();
            int height = CapDev.GetHeight();
            Bitmap rawImg = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);

            // Start to rendering~
            CapDev.StartCapture();
            Object CapLock = new Object();

            while (!stopCaptureLoop) 
            {
                lock (CapLock) {
                    var pixels = rawImg.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    CapDev.GetPixels(pixels.Scan0);
                    rawImg.UnlockBits(pixels);
                }

                // Convert to BitmapImage format
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) 
                {
                    rawImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    // Update UI display through dispatcher
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new RefreshDelegate(UpdateVideoFrame), bitmapImage);
                }
            }

            // CapDev.StopCapture();
            capEvent.Set();
        }

        private void UpdateVideoFrame(BitmapImage bmpSrc)
        {
            if (!stopCaptureLoop) {
                FrameImage.Stretch = Stretch.Uniform;
                FrameImage.Source = bmpSrc;
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FrameImage.Source = null;
            stopCaptureLoop = true;
        }
    }
}
