using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TelegramCRM.Program;

namespace TelegramCRM
{
    public class CallbackEventArgs
    {
        public int DataId { get; set; }
        public string CallbackAction{ get; set; }
    }
}
