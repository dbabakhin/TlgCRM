using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCRM.Database
{
    public class BotTask : XPObject
    {
        public BotTask(Session session) : base(session)
        {
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTime StartDate
        {
            get; set;
        }

        public DateTime EndDate
        {
            get; set;
        }

        public BotUser Appointer
        {
            get; set;
        }

        public BotUser Executor
        {
            get; set;
        }
        public string Status
        {
            get; set;
        }

        public string Priority
        {
            get; set;
        }
    }



    public enum TaskPriority
    {
        high, normal, low
    }
}
