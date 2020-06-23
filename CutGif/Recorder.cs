using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutGif
{
    public delegate void StartRecordEventHandler();
    public delegate void EndRecordEventHandler();
    public delegate void SaveRecordEndEventHandler();
    public delegate void SaveUpdateStatusEventHandler(int value, int maxValue);
    interface Recorder
    {
        bool StartRecord(TimeSpan TimeOfRecord, TimeSpan WaitTimeBeforeRecord);
        void SetZoneRecord(ScreenZone screenZone);

        event StartRecordEventHandler RecordStart;
        event EndRecordEventHandler RecordEnd;
        event SaveRecordEndEventHandler SaveRecord;
        event SaveUpdateStatusEventHandler SaveUpdateStatus;
    }
}
