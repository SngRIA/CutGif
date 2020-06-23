using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CutGif
{
    /// <summary>
    /// Interaction logic for SelectScreenZone.xaml
    /// </summary>
    public partial class SelectScreenZone : Window
    {
        private string tempFileNameBackground;
        private string tempFilePathBackground;

        private System.Windows.Shapes.Rectangle Rectangle;
        private ScreenZone screenZone;

        private bool selectedFirstPoint;

        public delegate void SetCutZoneEventHandler(ScreenZone zone);
        public event SetCutZoneEventHandler SendZone;

        public SelectScreenZone()
        {
            tempFileNameBackground = "bg.png";
            tempFilePathBackground = $"{Environment.CurrentDirectory}/{tempFileNameBackground}";

            selectedFirstPoint = false;

            RecordImageDesktop();
            InitializeComponent();
            InitCanvars();
            SetBackgroundImage();
            
            screenZone = new ScreenZone();

            this.MouseLeftButtonDown += SelectScreenZoneMouseLeftButtonDown;
            this.MouseLeftButtonUp += SelectScreenZoneMouseLeftButtonUp;
            this.MouseMove += SelectScreenZoneMouseMove;
            this.Closed += SelectScreenZoneClosed;
        }

        private void SelectScreenZoneClosed(object sender, EventArgs e)
        {
            CheckDeleteTempFile();

            SendZone(screenZone);
        }

        private void SelectScreenZoneMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(this);

            screenZone.SecondPoint.X = (int)pos.X;
            screenZone.SecondPoint.Y = (int)pos.Y;

            screenZone.Width = (int)Math.Abs(screenZone.FirstPoint.X - pos.X);
            screenZone.Height = (int)Math.Abs(screenZone.FirstPoint.Y - pos.Y);

            selectedFirstPoint = false;

            this.Close();
        }

        private void SelectScreenZoneMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (selectedFirstPoint)
            {
                System.Windows.Point pos = e.GetPosition(this);

                Rectangle.Height = Math.Abs(pos.Y - screenZone.FirstPoint.Y);
                Rectangle.Width = Math.Abs(pos.X - screenZone.FirstPoint.X);
            }
        }

        private void InitCanvars()
        {
            Rectangle = new System.Windows.Shapes.Rectangle();
            Rectangle.Stroke = System.Windows.Media.Brushes.Red;

            overlay.Children.Add(Rectangle);
        }

        private void SelectScreenZoneMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(this);

            Canvas.SetLeft(Rectangle, pos.X);
            Canvas.SetTop(Rectangle, pos.Y);

            screenZone.FirstPoint.X = (int)pos.X;
            screenZone.FirstPoint.Y = (int)pos.Y;

            selectedFirstPoint = true;
        }

        private void SetBackgroundImage()
        {
            this.bgImage.ImageSource = new BitmapImage(new Uri(tempFilePathBackground));
        }

        private void ClearBackgroundImage()
        {
            if (bgImage != null)
            {
                bgImage.ImageSource = null;
                bgImage = null;
            }
        }

        private void RecordImageDesktop()
        {
            CheckDeleteTempFile();

            using (Bitmap bitmap = new Bitmap(1920, 1080))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                }

                bitmap.Save(tempFileNameBackground);
            }
        }

        private bool CheckDeleteTempFile()
        {
            bool result = false;

            if(File.Exists(tempFilePathBackground))
            {
                ClearBackgroundImage();

                GC.Collect(); // Очистка ресурсов, в данном случае, использованного временного изображения

                Thread.Sleep(50); // Даём время на выгрузку
                
                File.Delete(tempFilePathBackground);

                result = true;
            }

            return result;
        }
    }
}
