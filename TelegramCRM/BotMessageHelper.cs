using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using TelegramCRM.Database;
using static TelegramCRM.Program;

namespace TelegramCRM
{
    public class BotMessageHelper
    {


        public static InlineKeyboardButton[][] TaskButtonListView(List<BotTask> tasksToPrint, string action)
        {
            var ikbList = new List<InlineKeyboardButton[]>();
            var ikb = new List<InlineKeyboardButton>();
            foreach (BotTask task in tasksToPrint)
            {
                ikb.Add(new InlineKeyboardCallbackButton(task.Name + " " + task.EndDate.ToString(), task.Oid.ToString() + "," + action));
                ikbList.Add(ikb.ToArray());
                ikb.Clear();
            }

            return ikbList.ToArray();
        }

        public static string GetTaskTextDetailView(BotTask task)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n       \t id: {0},  \n Название: {1}  \n Описание: {2}   \n Сделать до: {3}    \n Создана: {4}      \n Исполнитель: {5}      \n Назначил:{6}    \n Статус: {7} \n Приоритет: {8} \n ___________________________", task.Oid, task.Name, task.Description, task.EndDate, task.StartDate, task.Executor?.UserName, task.Appointer?.UserName, task.Status, task.Priority);
            return sb.ToString();
        }


        public static InlineKeyboardButton[][] GetTaskEditStateDeleteDetailViewButtonsAllow(int taskId)
        {
            var ikbList = new List<InlineKeyboardButton[]>();
            var ikb = new List<InlineKeyboardButton>();

            ikb.Add(new InlineKeyboardCallbackButton("Изменить задачу", taskId.ToString() + "," + nameof(CallbackActions.edittask)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            ikb.Add(new InlineKeyboardCallbackButton("Изменить статус", taskId.ToString() + "," + nameof(CallbackActions.edittaskstat)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            ikb.Add(new InlineKeyboardCallbackButton("Удалить задачу", taskId.ToString() + "," + nameof(CallbackActions.deletetask)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            ikb.Add(new InlineKeyboardCallbackButton("Добавить файлы", taskId.ToString() + "," + nameof(CallbackActions.addfiles)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            ikb.Add(new InlineKeyboardCallbackButton("Смотреть файлы", taskId.ToString() + "," + nameof(CallbackActions.showfiles)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            return ikbList.ToArray();
        }


        public static InlineKeyboardButton[][] GetTaskEditStateDeleteDetailViewButtonsState(int taskId)
        {
            var ikbList = new List<InlineKeyboardButton[]>();
            var ikb = new List<InlineKeyboardButton>();

            ikb.Add(new InlineKeyboardCallbackButton("Изменить статус", taskId.ToString() + "," + nameof(CallbackActions.edittaskstat)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            ikb.Add(new InlineKeyboardCallbackButton("Смотреть файлы", taskId.ToString() + "," + nameof(CallbackActions.showfiles)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();

            return ikbList.ToArray();
        }


        public static InlineKeyboardButton[][] GetTaskEditDetailViewButtons(int taskid)
        {
            var ikbList = new List<InlineKeyboardButton[]>();
            var ikb = new List<InlineKeyboardButton>();


            ikb.Add(new InlineKeyboardCallbackButton("название", taskid.ToString() + "," + nameof(CallbackActions.edittaskname)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("описание", taskid.ToString() + "," + nameof(CallbackActions.edittaskdesc)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("дату начала", taskid.ToString() + "," + nameof(CallbackActions.edittasksdate)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("дату окончания", taskid.ToString() + "," + nameof(CallbackActions.edittaskedate)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("исполнителя", taskid.ToString() + "," + nameof(CallbackActions.edittaskexecutor)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("приоретет", taskid.ToString() + "," + nameof(CallbackActions.edittaskpriority)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("статус", taskid.ToString() + "," + nameof(CallbackActions.edittaskstat)));
            ikbList.Add(ikb.ToArray());

            return ikbList.ToArray();
        }


        public static string GetHelpMessage()
        {
            return $@"
            /ntask - создать задачу

            /ntask 2 name:TaskName, dt:TaskDescription, dt:24/12/2018 20:33
            
            /help - помощь
            
            /killuser id - удаляет пользователя из базы 
            
            /allusers - список всех пользователей

            /alltask - список всех задач

            /mytask - задачи поставленные мной кроме завершенных и убитых

            /tasktome - задачи поставленные мне кроме завершенных у убитых

            /mytaskall - задачи поставленные мной 

            /tasktomeall - задачи поставленные мне 

            /setuser id - дать права доступа на CRM
    
            /setadm id - Права администратора

            /killmytask - Убивает все задачи поставленные вами
            
            ";
        }

        internal static InlineKeyboardButton[][] GetTaskPrioprityButtons(long taskid)
        {
            var ikbList = new List<InlineKeyboardButton[]>();
            var ikb = new List<InlineKeyboardButton>();
            ikb.Add(new InlineKeyboardCallbackButton("Высокий", taskid + "," + nameof(CallbackActions.phight)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("Средний", taskid + "," + nameof(CallbackActions.pmiddle)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("Низкий", taskid + "," + nameof(CallbackActions.plow)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();


            return ikbList.ToArray();
        }

        internal static InlineKeyboardButton[][] GetTaskPrioprityButtonsWithoultChecking(long taskid)
        {
            var ikbList = new List<InlineKeyboardButton[]>();
            var ikb = new List<InlineKeyboardButton>();
            ikb.Add(new InlineKeyboardCallbackButton("Высокий", taskid + "," + nameof(CallbackActions.phightw)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("Средний", taskid + "," + nameof(CallbackActions.pmiddlew)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();
            ikb.Add(new InlineKeyboardCallbackButton("Низкий", taskid + "," + nameof(CallbackActions.ploww)));
            ikbList.Add(ikb.ToArray());
            ikb.Clear();


            return ikbList.ToArray();
        }
    }
}


