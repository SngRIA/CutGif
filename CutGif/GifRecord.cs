using AnimatedGif;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

using Point = System.Drawing.Point;

namespace CutGif
{
    class GifRecord : Recorder
    {
        public event StartRecordEventHandler RecordStart;
        public event EndRecordEventHandler RecordEnd;
        public event SaveRecordEndEventHandler SaveRecord;
        public event SaveUpdateStatusEventHandler SaveUpdateStatus;

        private ScreenZone _screenZone;

        private string tempPath;

        public GifRecord()
        {
            tempPath = $"{Environment.CurrentDirectory}//tempScreenshots";

            CheckTempDirectory();

            _screenZone = new ScreenZone { 
                FirstPoint = new Point(150, 150), 
                SecondPoint = new Point(300, 300) 
            };
        }

        /// <summary>
        /// Начинает запись экрана по заданным настройкам
        /// </summary>
        /// <param name="TimeOfRecord"> Время записи </param>
        /// <param name="WaitTimeBeforeRecord"> Задержка перед началом записи </param>
        /// <returns></returns>
        public bool StartRecord(TimeSpan TimeOfRecord, TimeSpan WaitTimeBeforeRecord)
        {
            RecordStart();

            List<string> filePath = new List<string>();

            Thread.Sleep((int)WaitTimeBeforeRecord.TotalMilliseconds);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                SavePicture(elapsedMilliseconds);

                filePath.Add($"{tempPath}//{elapsedMilliseconds}.png");

            } while (stopwatch.ElapsedMilliseconds <= TimeOfRecord.TotalMilliseconds);

            RecordEnd();

            Save(filePath);

            return true;
        }

        /// <summary>
        /// Установка зоны захвата экрана
        /// </summary>
        /// <param name="screenZone"> Параметрами захвата </param>
        public void SetZoneRecord(ScreenZone screenZone)
        {
            _screenZone = screenZone;
        }

        private void SavePicture(long pictureTime)
        {
            using (Bitmap bitmap = new Bitmap(_screenZone.Width, _screenZone.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(_screenZone.FirstPoint.X, _screenZone.FirstPoint.Y, 0, 0, bitmap.Size);
                }

                bitmap.Save($"{tempPath}//{pictureTime}.png");
            }
        }
        private void Save(List<string> framesPath)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Gif Files(*.Gif)|*.Gif";
            dialog.FileName = "file";

            if (dialog.ShowDialog().Value)
            {
                Task.Factory.StartNew(() =>
                {
                    using (var gif = new AnimatedGifCreator(dialog.FileName, int.Parse(Properties.Settings.Default["Fps"].ToString())))
                    {
                        int value = 0;
                        foreach (var img in framesPath)
                        {
                            gif.AddFrame(img, delay: -1, quality: GifQuality.Bit8);
                            SaveUpdateStatus(++value, framesPath.Count);
                        }
                    }
                }).ContinueWith((a1) => {
                    ClearTempFiles();
                    SaveRecord();
                });
            }
        }

        private bool CheckTempDirectory()
        {
            bool result = true;

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);

                result = false;
            }

            return result;
        }

        private void ClearTempFiles()
        {
            if(CheckTempDirectory())
            {
                foreach (var file in Directory.GetFiles(tempPath))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
