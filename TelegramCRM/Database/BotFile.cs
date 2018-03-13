using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCRM.Database
{
    public class BotFile : XPObject
    {
        public BotFile(Session session) : base(session)
        {
        }

        public string FileId
        {
            get; set;
        }

        public string FileName
        {
            get; set;
        }

        private BotTask task;
        [Association("File-Task")]
        public BotTask Task
        {
            get => task;
            set => SetPropertyValue(nameof(Task), ref task, value);
        }
    }
}
