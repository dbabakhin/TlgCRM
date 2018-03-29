using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramCRM.Database;

namespace TelegramCRM
{
    public class CommandProcessor
    {
        public static int ParseCommandIntFirstParametr(string command)
        {
            int accessedUserId;
            if (command.Split(' ').Count() > 1)
            {
                bool res = Int32.TryParse(command.Split(' ')[1].Split(' ')[0], out accessedUserId);
                if (res)
                {
                    return accessedUserId;
                }
            }
            return -1;
        }



        public static void ParseNewTaskCommand(BotTask task, string command, Session session, string appointerUserName)
        {
            BotUser appointer = session.FindObject<BotUser>(CriteriaOperator.Parse($"UserName == '{appointerUserName}'"));
            if (appointer == null)
                throw new Exception($"Пробема при выставлении назначевшего задачу, пользователь в логином {appointerUserName} не найден в базе данных");

            task.Appointer = session.FindObject<BotUser>(CriteriaOperator.Parse($"UserName == '{appointerUserName}'"));
            task.StartDate = DateTime.Now;

            string[] commandAgrs = command.Split(',');
            int? executorId = null;
            var pieceOfCommand = command.Split(' ');
            if (pieceOfCommand.Count() > 1)
            {
                executorId = Convert.ToInt32(pieceOfCommand[1].Split(' ')[0]);
            }
            if (executorId != null)
            {
                BotUser executor = session.FindObject<BotUser>(CriteriaOperator.Parse($"Oid == {executorId}"));
                if (executor != null)
                {
                    task.Executor = session.FindObject<BotUser>(CriteriaOperator.Parse($"Oid == {executorId}"));
                    foreach (string arg in commandAgrs)
                    {
                        if (arg.Contains("name:"))
                        {
                            task.Name = arg.Split(':')[1];
                        }
                        if (arg.Contains("description:"))
                        {
                            task.Description = arg.Split(':')[1];
                        }
                        if (arg.Contains("dt:"))
                        {
                            string minutes = arg.Split(':')[2];
                            string dateString = arg.Split(':')[1] + ':' + minutes;
                            DateTime dt = DateTime.Parse(dateString);
                            task.EndDate = dt;
                        }
                        if (arg.Contains("p:"))
                        {
                            task.Priority = arg.Split(':')[1];
                        }
                    }
                }
                else
                {
                    throw new Exception("Пользователя с таким ID нет в базе данных");
                }
            }
        }

        public static CallbackEventArgs ParseCallbackAgrs(string callbackdata)
        {
            int dataId = Convert.ToInt32(callbackdata.Split(',')[0]);
            string actionType = callbackdata.Split(',')[1];
            return new CallbackEventArgs() { DataId = dataId, CallbackAction = actionType };
        }

        public static string CommandParser(string command)
        {
            if (command != null)
            {
                if (command.Split('/').Count() >= 2)
                {
                    return command.Split('/')[1].Split(' ')[0];
                }
            }
            return null;
        }

        public static int GetDeletedUserId(string commandString)
        {
            return Convert.ToInt32(commandString.Split(' ')[1]);
        }

        public static void ParseNewUserCommand(BotUser user, string commandString)
        {
            if (!commandString.Contains(","))
            {
                string newUserName = commandString.Split(' ')[1];
                user.UserName = newUserName;
            }
            else
            {
                string[] commandAgrs = commandString.Split(',');
                foreach (string arg in commandAgrs)
                {
                    if (arg.Contains("username:"))
                    {
                        user.UserName = arg.Split(':')[1];
                    }
                    if (arg.Contains("firstname:"))
                    {
                        user.FirstName = arg.Split(':')[1];
                    }
                    if (arg.Contains("lastname:"))
                    {
                        user.LastName = arg.Split(':')[1];
                    }
                }
            }
        }



    }
}

