using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCRM.Database
{
    public class BotUser : XPObject
    {
        public BotUser(Session session) : base(session)
        {
        }

        public string FirstName
        {
            get; set;
        }
        public string LastName
        {
            get; set;
        }
        public string UserName
        {
            get; set;
        }
        public int TelegramId
        {
            get; set;
        }
        public bool IsAdministrative
        {
            get; set;
        }
        public bool HasAccess
        {
            get; set;
        }
        public long ChatId
        {
            get; set;
        }
    }
}
