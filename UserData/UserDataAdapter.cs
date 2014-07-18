using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Timer.UserData
{
    public partial class UserData
    {
        private string _dataFilePath;
        private string dataFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_dataFilePath))
                    return _dataFilePath;

                var exeFolder = Environment.GetCommandLineArgs()[0];
                exeFolder = exeFolder.Substring(0, exeFolder.LastIndexOf('\\'));
                _dataFilePath = exeFolder + "\\UserData.xml";
                return _dataFilePath;
            }
        }

        public void WriteData()
        {
            // XML設定
            var setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "    ";
            setting.Encoding = new UTF8Encoding(false);

            using (XmlWriter w = XmlWriter.Create(dataFilePath, setting))
            {
                w.WriteStartElement("UserData");
                w.WriteElementString("Second", GetSecond().ToString());
                w.WriteElementString("PreWorkFinishedSecond", PreWorkFinishedSecond.ToString());
                w.WriteElementString("WorkIdx", CurrentWorkIdx.ToString());

                w.WriteStartElement("WorkItems");
                if (HasWorkSet)
                {
                    foreach (WorkItem workItem in GetAllWorkItems())
                    {
                        w.WriteStartElement("WorkItem");
                        w.WriteElementString("Name", workItem.Name);
                        w.WriteElementString("Minutes", workItem.Minutes.ToString());
                        w.WriteEndElement();
                    }
                }

                w.WriteEndElement();
                w.WriteEndElement();
            }
        }

        public void ReadData()
        {
            if (!File.Exists(dataFilePath))
            {
                InitSecond = 0;
                PreWorkFinishedSecond = 0;
                CurrentWorkIdx = 0;
                MyWorkSet = new WorkSet(new List<WorkItem>());
                return;
            }

            using (XmlReader r = XmlReader.Create(dataFilePath))
            {
                while (r.Read())
                {
                    if ((r.NodeType == XmlNodeType.Element))
                    {
                        switch (r.Name)
                        {
                            case "Second":
                                var second = 0;
                                int.TryParse(r.ReadString(), out second);
                                InitSecond = second;
                                break;
                            case "PreWorkFinishedSecond":
                                var preWorkFinishedSecond = 0;
                                int.TryParse(r.ReadString(), out preWorkFinishedSecond);
                                PreWorkFinishedSecond = preWorkFinishedSecond;
                                break;
                            case "WorkIdx":
                                var workIdx = 0;
                                int.TryParse(r.ReadString(), out workIdx);
                                CurrentWorkIdx = workIdx;
                                break;
                            case "WorkItems":
                                var workItems = new List<WorkItem>();
                                while (r.Read())
                                {
                                    if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "WorkItems"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        var item = ReadWorkItemData(r);
                                        if (item == null || WorkItem.IsNullOrEmpty(item))
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            workItems.Add(item);
                                        }
                                    }
                                }
                                MyWorkSet = new WorkSet(workItems);
                                break;
                        }
                    }
                }
            }
        }

        private WorkItem ReadWorkItemData(XmlReader reader)
        {
            var name = string.Empty;
            var minutes = 0;

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "WorkItems"))
                {
                    return null;
                }

                if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "WorkItem"))
                {
                    break;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Name":
                            name = reader.ReadString();
                            break;
                        case "Minutes":
                            int.TryParse(reader.ReadString(), out minutes);
                            minutes = Math.Max(0, minutes);
                            break;
                    }
                }
            }

            return new WorkItem(name, minutes);;
        }
    }
}
