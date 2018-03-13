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
            sb.AppendFormat("\n id: {0},  \n Название: {1}  \n Описание: {2}   \n Сделать до: {3}    \n Создана: {4}      \n Исполнитель: {5}      \n Назначил:{6}    \n   Статус: {7} \n ******************", task.Oid, task.Name, task.Description, task.EndDate, task.StartDate, task.Executor?.UserName, task.Appointer?.UserName, task.Status);
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
            /help - помощь
            
            /killuser id - удаляет пользователя из базы 
            
            /ntask (запятые обязательны) id(обязательно) name:названиеЗадачи, description:описаниеЗадачи, dt:10/12/2018 23:25

            /allusers - список всех пользователей

            /mytask - задачи поставленные мной

            /tasktome - задачи поставленные мне

            /setuser id - дать права доступа на CRM
    
            /setadm id - Права администратора
            
            ";
        }

    }
}


