using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramCRM.Database;

namespace TelegramCRM
{
    public class PermissionSystem
    {

        public static bool IsAdministrative(int userId)
        {
            Session s = new Session();
            BotUser checkedUser = s.GetObjectByKey<BotUser>(userId);
            if (checkedUser == null)
                return false;

            return checkedUser.IsAdministrative;
        }

        public static bool HasAccess(int userId)
        {
            Session s = new Session();
            BotUser checkedUser = s.GetObjectByKey<BotUser>(userId);
            if (checkedUser == null)
                return false;

            return checkedUser.HasAccess;
        }

        public static BotUser GetUserByTlgId(int telegramId)
        {
            Session s = new Session();
            var users = from user in s.Query<BotUser>()
                        where user.TelegramId == telegramId
                        select user;
            return users.FirstOrDefault();
        }

    }
}
