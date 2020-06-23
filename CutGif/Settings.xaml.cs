using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CutGif
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            LoadValues();
        }

        private void LoadValues()
        {
            sliderWaitTime.Value = (double)Properties.Settings.Default["WaitTimeBeforeRecord"];
            sliderRecordTime.Value = (double)Properties.Settings.Default["TimeOfRecord"];
            sliderFps.Value = (double)Properties.Settings.Default["Fps"];
        }

        private void ButtonSave(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["WaitTimeBeforeRecord"] = sliderWaitTime.Value;
            Properties.Settings.Default["TimeOfRecord"] = sliderRecordTime.Value;
            Properties.Settings.Default["Fps"] = sliderFps.Value;
            Properties.Settings.Default.Save();

            this.Close();
        }

        private void ButtonCloseWindow(object sender, RoutedEventArgs e) => this.Close();
    }
}
