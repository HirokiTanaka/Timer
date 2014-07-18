
namespace Timer.UserData
{
    public class WorkItem
    {
        public string Name { get; private set; }

        public int Minutes { get; private set; }

        public int Seconds { get { return Minutes * 60; } }

        public WorkItem()
        {
            Name = string.Empty;
            Minutes = 0;
        }

        public WorkItem(string name, int minutes)
        {
            this.Name = name;
            this.Minutes = minutes;
        }

        public static bool IsNullOrEmpty(WorkItem wi)
        {
            return wi == null || (string.IsNullOrEmpty(wi.Name) && wi.Minutes <= 0);
        }
    }
}
