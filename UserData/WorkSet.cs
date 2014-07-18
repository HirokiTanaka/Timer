using System.Collections.Generic;
using System.Linq;

namespace Timer.UserData
{
    public class WorkSet
    {
        public IEnumerable<WorkItem> WorkItems { get; private set; }

        public bool HasWorkItem
        {
            get { return WorkItems != null && 1 <= WorkItems.Count(); }
        }

        public WorkSet(IEnumerable<WorkItem> workItems)
        {
            this.WorkItems = workItems;
        }

        private int? _totalSeconds;
        public int GetTotalSeconds()
        {
            return (_totalSeconds ?? (_totalSeconds = (HasWorkItem ? WorkItems.Sum(x => x.Seconds) : 0))).Value;
        }

        public int GetTotalSeconds(int idx)
        {
            if (!HasWorkItem) return 0;

            return WorkItems.Where((x, i) => i <= idx).Sum(x => x.Seconds);
        }

        public WorkItem GetWorkItem(int idx)
        {
            return (HasWorkItem && idx < WorkItems.Count()) ? WorkItems.ElementAt(idx) : null;
        }

        public int WorkItemCount
        {
            get { return HasWorkItem ? WorkItems.Count() : 0; }
        }
    }
}
