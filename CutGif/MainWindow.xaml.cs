using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CutGif
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private Recorder Recorder;
        public MainWindow()
        {
            InitializeComponent();

            Recorder = new GifRecord();
            Recorder.RecordStart += RecorderRecordStart;
            Recorder.RecordEnd += RecorderRecordEnd;
            Recorder.SaveRecord += RecorderSaveRecord;
            Recorder.SaveUpdateStatus += RecorderSaveUpdateStatus;
        }

        private void RecorderSaveUpdateStatus(int value, int maxValue)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (progressBar.Value == 0)
                    progressBar.Maximum = maxValue;
                progressBar.Value = value;
            });
        }

        private void RecorderSaveRecord()
        {
            // После сохранения файла
            buttonStart.IsEnabled = true;
            MessageBox.Show("Save");
        }

        private void RecorderRecordStart()
        {
            // После начала записи
        }

        private void RecorderRecordEnd()
        {
            // После окончания записи
            progressBar.Value = 0;
        }

        private void RecordSetCutZone(ScreenZone zone)
        {
            Recorder.SetZoneRecord(zone);

            double timeOfRecord = (double)Properties.Settings.Default["TimeOfRecord"];
            double waitTimeBeforeRecord = (double)Properties.Settings.Default["WaitTimeBeforeRecord"];

            Recorder.StartRecord(TimeSpan.FromSeconds(timeOfRecord), TimeSpan.FromSeconds(waitTimeBeforeRecord));
        }

        private void ButtonOpenSettings(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Owner = this;
            settings.ShowDialog(); // Вызов как диалога для блокировки основного окна
        }

        private void ButtonStartRecord(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;

            this.Hide();

            var screenZoneWindow = new SelectScreenZone();
            screenZoneWindow.Owner = this;
            screenZoneWindow.Show();

            this.Show();

            screenZoneWindow.SendZone += RecordSetCutZone;
        }
    }
}
