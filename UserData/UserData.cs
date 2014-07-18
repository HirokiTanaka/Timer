using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.UserData
{
    public partial class UserData
    {
        private int InitSecond { get; set; }

        public int PreWorkFinishedSecond { get; private set; }

        private WorkSet MyWorkSet { get; set; }

        public bool HasWorkSet
        {
            get { return MyWorkSet != null && MyWorkSet.HasWorkItem; }
        }

        public IEnumerable<WorkItem> GetAllWorkItems()
        {
            return MyWorkSet.WorkItems;
        }

        public int TotalWorkSeconds
        {
            get { return HasWorkSet ? MyWorkSet.GetTotalSeconds() : 0; }
        }

        private int CurrentWorkIdx { get; set; }

        private WorkItem CurrentWorkItem
        {
            get { return MyWorkSet.GetWorkItem(CurrentWorkIdx); }
        }

        public string CurrentWorkName
        {
            get
            {
                var item = CurrentWorkItem;
                return item == null ? string.Empty : item.Name;
            }
        }

        public int CurrentWorkSeconds
        {
            get
            {
                var item = CurrentWorkItem;
                return item == null ? 0 : item.Seconds;
            }
        }

        public bool IsAllFinished
        {
            get { return MyWorkSet.WorkItemCount <= CurrentWorkIdx; }
        }

        private Stopwatch _myStopWatch;

        private static UserData _instance;

        private UserData()
        {
            _myStopWatch = new Stopwatch();
            ReadData();
        }

        public static UserData GetInstance()
        {
            return _instance ?? (_instance = new UserData());
        }

        public void Edit(WorkSet workSet)
        {
            InitSecond = 0;
            PreWorkFinishedSecond = 0;
            CurrentWorkIdx = 0;
            MyWorkSet = workSet;
        }

        public void SaveData()
        {
            CountStop();
            WriteData();
        }

        public void FinishWorkItem()
        {
            PreWorkFinishedSecond = GetSecond();
            if (IsAllFinished)
                return;

            CurrentWorkIdx++;
        }

        public int GetSecond()
        {
            var ret = InitSecond;

            if (_myStopWatch.IsRunning)
                ret += (int)_myStopWatch.Elapsed.TotalSeconds;

            return ret;
        }

        public int GetNextLimitSecond()
        {
            if (!HasWorkSet) return 0;

            return MyWorkSet.GetTotalSeconds(CurrentWorkIdx);
        }

        public void CountStart()
        {
            if (_myStopWatch.IsRunning) return;

            _myStopWatch.Reset();
            _myStopWatch.Start();
        }

        public void CountStop()
        {
            if (!_myStopWatch.IsRunning) return;

            _myStopWatch.Stop();
            InitSecond += (int)_myStopWatch.Elapsed.TotalSeconds;
        }
    }
}