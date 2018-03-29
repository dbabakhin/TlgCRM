using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramCRM.Database;

namespace TelegramCRM
{
    partial class Program
    {

        private static readonly string apiKey = "559244515:AAEKvJKLQ-jBkilGwGNU7EJmKlQ2f43xOzY";
        private static readonly Telegram.Bot.TelegramBotClient Bot = new Telegram.Bot.TelegramBotClient(apiKey);
        private static HashSet<ChatStatement> ChatStatements = new HashSet<ChatStatement>();

        static void Main(string[] args)
        {
            var connectionString = SQLiteConnectionProvider.GetConnectionString("botdatabase");
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
            Console.WriteLine("Bot work");
            Console.ReadLine();
            Bot.StopReceiving();
            Console.WriteLine("Bot stopped");
            Console.ReadLine();
        }

        private static async void ChangeTaskProprity(int taskId, string priority, long chatId, bool useCheck)
        {
            using (Session s = new Session())
            {
                var res = s.GetObjectByKey<BotTask>(taskId);
                if (res != null)
                {
                    res.Priority = priority;
                    res.Save();
                    await Bot.SendTextMessageAsync(chatId, $"Приоритет успешно изменен");
                    if (useCheck)
                        await CheckTaskToFill(res, chatId);
                }
            }
        }

        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            CallbackEventArgs args = CommandProcessor.ParseCallbackAgrs(e.CallbackQuery.Data);
            long chatId = e.CallbackQuery.Message.Chat.Id;
            switch (args.CallbackAction)
            {


                case nameof(CallbackActions.phight):
                    ChangeTaskProprity(args.DataId, "Высокий", chatId, true);
                    break;
                case nameof(CallbackActions.pmiddle):
                    ChangeTaskProprity(args.DataId, "Средний", chatId, true);
                    break;
                case nameof(CallbackActions.plow):
                    ChangeTaskProprity(args.DataId, "Низкий", chatId, true);
                    break;
                case nameof(CallbackActions.phightw):
                    ChangeTaskProprity(args.DataId, "Высокий", chatId, false);
                    break;
                case nameof(CallbackActions.pmiddlew):
                    ChangeTaskProprity(args.DataId, "Средний", chatId, false);
                    break;
                case nameof(CallbackActions.ploww):
                    ChangeTaskProprity(args.DataId, "Низкий", chatId, false);
                    break;

                case nameof(CallbackActions.deletetask):
                    {
                        try
                        {
                            ExecuteActionDeleteTask(args);
                            await Bot.SendTextMessageAsync(chatId, $"Задача успешно удалена");
                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(chatId, $"Не удалось удалить задачу");
                        }
                        break;
                    }
                case nameof(CallbackActions.edittask):
                    {
                        ExecuteActionEditTask(args, chatId);
                        break;
                    }


                case nameof(CallbackActions.edittaskname):
                    {
                        AddChatStatement(chatId, ChatStatemtns.NextMessageIsTaksName, args.DataId);
                        await Bot.SendTextMessageAsync(chatId, $"Введите новое название для задачи");
                        break;
                    }
                case nameof(CallbackActions.edittaskdesc):
                    {
                        AddChatStatement(chatId, ChatStatemtns.NextMessageIsTaskDesc, args.DataId);
                        await Bot.SendTextMessageAsync(chatId, $"Введите новое описание");
                        break;
                    }
                case nameof(CallbackActions.statenew):
                    ChangeTaskState(args.DataId, chatId, "Новая");
                    break;
                case nameof(CallbackActions.stateread):
                    ChangeTaskState(args.DataId, chatId, "Прочитана");
                    break;
                case nameof(CallbackActions.statework):
                    ChangeTaskState(args.DataId, chatId, "В работе");
                    break;
                case nameof(CallbackActions.statedone):
                    ChangeTaskState(args.DataId, chatId, "Завершена");
                    break;
                case nameof(CallbackActions.statecorect):
                    ChangeTaskState(args.DataId, chatId, "Проверена");
                    break;
                case nameof(CallbackActions.statekill):
                    ChangeTaskState(args.DataId, chatId, "Убита");
                    break;
                case nameof(CallbackActions.edittaskstat):
                    {
                        var ikbList = new List<InlineKeyboardButton[]>();
                        var ikb = new List<InlineKeyboardButton>();
                        ikb.Add(new InlineKeyboardCallbackButton("Новая", args.DataId.ToString() + "," + nameof(CallbackActions.statenew)));
                        ikbList.Add(ikb.ToArray());
                        ikb.Clear();
                        ikb.Add(new InlineKeyboardCallbackButton("Прочитана", args.DataId.ToString() + "," + nameof(CallbackActions.stateread)));
                        ikbList.Add(ikb.ToArray());
                        ikb.Clear();
                        ikb.Add(new InlineKeyboardCallbackButton("В работе", args.DataId.ToString() + "," + nameof(CallbackActions.statework)));
                        ikbList.Add(ikb.ToArray());
                        ikb.Clear();
                        ikb.Add(new InlineKeyboardCallbackButton("Выполнена", args.DataId.ToString() + "," + nameof(CallbackActions.statedone)));
                        ikbList.Add(ikb.ToArray());
                        ikb.Clear();
                        ikb.Add(new InlineKeyboardCallbackButton("Проверена", args.DataId.ToString() + "," + nameof(CallbackActions.statecorect)));
                        ikbList.Add(ikb.ToArray());
                        ikb.Clear();
                        ikb.Add(new InlineKeyboardCallbackButton("Убита", args.DataId.ToString() + "," + nameof(CallbackActions.statekill)));
                        ikbList.Add(ikb.ToArray());
                        ikb.Clear();
                        await Bot.SendTextMessageAsync(chatId, "Выберите новый статус", Telegram.Bot.Types.Enums.ParseMode.Default, true, false, 0, new InlineKeyboardMarkup(ikbList.ToArray()));
                        break;
                    }
                case nameof(CallbackActions.tasklistview):
                    {
                        break;
                    }
                case nameof(CallbackActions.edittasksdate):
                    {
                        AddChatStatement(chatId, ChatStatemtns.NextMessageIsTaskSDate, args.DataId);
                        await Bot.SendTextMessageAsync(chatId, $"Введите новую дату начала");
                        break;
                    }
                case nameof(CallbackActions.edittaskedate):
                    {
                        AddChatStatement(chatId, ChatStatemtns.NextMessageIsTaskEDate, args.DataId);
                        await Bot.SendTextMessageAsync(chatId, $"Введите новую дату окончания");
                        break;
                    }
                case nameof(CallbackActions.edittaskexecutor):
                    {
                        AddChatStatement(chatId, ChatStatemtns.NextMessageIsTaskExecutorId, args.DataId);
                        await Bot.SendTextMessageAsync(chatId, $"Введите ID исполнителя");

                        break;
                    }
                case nameof(CallbackActions.edittaskpriority):
                    {
                        await Bot.SendTextMessageAsync(chatId, $"Введите приоритет задачи", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, new InlineKeyboardMarkup(BotMessageHelper.GetTaskPrioprityButtonsWithoultChecking(args.DataId)));
                        break;
                    }
                case nameof(CallbackActions.taskdetailviewall):
                    {
                        using (Session s = new Session())
                        {
                            BotTask task = s.GetObjectByKey<BotTask>(args.DataId);
                            await Bot.SendTextMessageAsync(chatId, BotMessageHelper.GetTaskTextDetailView(task), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, new InlineKeyboardMarkup(BotMessageHelper.GetTaskEditStateDeleteDetailViewButtonsAllow(args.DataId)));
                            break;
                        }
                    }
                case nameof(CallbackActions.taskdetailviewdeny):
                    {
                        Session s = new Session();
                        BotTask task = s.GetObjectByKey<BotTask>(args.DataId);
                        await Bot.SendTextMessageAsync(chatId, BotMessageHelper.GetTaskTextDetailView(task));
                        break;
                    }
                case nameof(CallbackActions.taskdetailviewstat):
                    {
                        using (Session s = new Session())
                        {
                            BotTask task = s.GetObjectByKey<BotTask>(args.DataId);
                            if (task.Status == "Новая")
                            {
                                task.Status = "Прочитана";
                                task.Save();
                            }
                            await Bot.SendTextMessageAsync(chatId, BotMessageHelper.GetTaskTextDetailView(task), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, new InlineKeyboardMarkup(BotMessageHelper.GetTaskEditStateDeleteDetailViewButtonsState(args.DataId)));
                            break;
                        }
                    }
                case nameof(CallbackActions.addfiles):
                    {
                        await Bot.SendTextMessageAsync(chatId, "Отправьте один файл");
                        AddChatStatement(chatId, ChatStatemtns.NextMessageIsFiles, args.DataId);
                        break;
                    }
                case nameof(CallbackActions.showfiles):
                    {
                        ExecuteActionShowFiles(args.DataId, chatId);
                        break;
                    }
            }
        }


        private static async Task<bool> CheckTaskToFill(BotTask task, long chatId)
        {
            if (string.IsNullOrEmpty(task.Name))
            {
                ChatStatements.Add(new ChatStatement() { ChatId = chatId, BotTaskId = task.Oid, Statament = ChatStatemtns.NextMessageIsNewTaskName });
                await Bot.SendTextMessageAsync(chatId, $"Введите название задачи");
                return false;
            }
            if (string.IsNullOrEmpty(task.Description))
            {
                ChatStatements.Add(new ChatStatement() { ChatId = chatId, BotTaskId = task.Oid, Statament = ChatStatemtns.NextMessageIsNewTaskDesc });
                await Bot.SendTextMessageAsync(chatId, $"Введите описание задачи");
                return false;
            }
            if (task.EndDate == null || task.EndDate == new DateTime())
            {
                ChatStatements.Add(new ChatStatement() { ChatId = chatId, BotTaskId = task.Oid, Statament = ChatStatemtns.NextMessageIsNewTaskDt });
                await Bot.SendTextMessageAsync(chatId, $"Введите дату окончания задачи");
                return false;
            }
            if (task.Priority == null)
            {
                // ChatStatements.Add(new ChatStatement() { ChatId = chatId, BotTaskId = task.Oid, Statament = ChatStatemtns.NextMessageIsNewTaskPriority });
                await Bot.SendTextMessageAsync(chatId, $"Введите приоритет задачи", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, new InlineKeyboardMarkup(BotMessageHelper.GetTaskPrioprityButtons(task.Oid)));
                return false;
            }
            if (task.Executor == null)
            {
                ChatStatements.Add(new ChatStatement() { ChatId = chatId, BotTaskId = task.Oid, Statament = ChatStatemtns.NextMessageIsNewTaskExecutorId });
                await Bot.SendTextMessageAsync(chatId, $"Введите ID исполнителя");
                return false;
            }

            await Bot.SendTextMessageAsync(chatId, $"Задача {task.Name} успешно создана!");
            return true;
        }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            long chatId = e.Message.Chat.Id;
            var statement = from data in ChatStatements
                            where data.ChatId == chatId
                            select data;
            var chatStatement = statement.FirstOrDefault();
            if (chatStatement != null)
            {
                using (Session s = new Session())
                {
                    BotTask editedTask = s.GetObjectByKey<BotTask>(chatStatement.BotTaskId);
                    string messageText = e.Message.Text;
                    try
                    {
                        switch (chatStatement.Statament)
                        {
                            case ChatStatemtns.NextMessageIsTaksName:
                                editedTask.Name = messageText;
                                await Bot.SendTextMessageAsync(chatId, $"Название задачи изменено");
                                break;
                            case ChatStatemtns.NextMessageIsTaskDesc:
                                editedTask.Description = messageText;
                                await Bot.SendTextMessageAsync(chatId, $"Примечание задачи изменено");
                                break;
                            case ChatStatemtns.NextMessageIsTaskSDate:
                                editedTask.StartDate = DateTime.Parse(messageText);
                                await Bot.SendTextMessageAsync(chatId, $"Дата начала задачи изменена");
                                break;
                            case ChatStatemtns.NextMessageIsTaskEDate:
                                editedTask.EndDate = DateTime.Parse(messageText);
                                await Bot.SendTextMessageAsync(chatId, $"Дата окончания задачи изменена");
                                break;
                            case ChatStatemtns.NextMessageIdTaskPriorirty:
                                editedTask.Priority = messageText;
                                await Bot.SendTextMessageAsync(chatId, $"Приоритет задачи изменен");
                                break;
                            case ChatStatemtns.NextMessageIsTaskExecutorId:

                                BotUser newExecutror = s.GetObjectByKey<BotUser>(Convert.ToInt32(messageText));
                                if (newExecutror != null)
                                {
                                    editedTask.Executor = newExecutror;
                                    await Bot.SendTextMessageAsync(chatId, $"Исполтитель задачи изменен");
                                    if (editedTask.Executor != null)
                                        await Bot.SendTextMessageAsync(editedTask.Executor.ChatId, "Вам была назначена задача " + editedTask.Name + " выполнить до " + editedTask.EndDate);

                                }
                                else
                                {
                                    await Bot.SendTextMessageAsync(chatId, $"Не удалось найти пользователя с таким ID");
                                }
                                break;
                            case ChatStatemtns.NextMessageIsTaskStatus:
                                await Bot.SendTextMessageAsync(chatId, $"Стутус задачи изменен");
                                break;
                            case ChatStatemtns.NextMessageIsFiles:

                                List<BotFile> attachedFile = new List<BotFile>();

                                switch (e.Message.Type)
                                {
                                    case Telegram.Bot.Types.Enums.MessageType.DocumentMessage:
                                        if (e.Message.Document != null)
                                        {
                                            BotFile f = new BotFile(s);
                                            f.FileId = e.Message.Document.FileId;
                                            f.FileName = e.Message.Document.FileName;
                                            f.Task = editedTask;
                                            f.Save();
                                            await Bot.SendTextMessageAsync(chatId, $"Файл {f.FileName} успешно прикреплен к задаче {editedTask.Name}");
                                        }
                                        break;
                                    default:
                                        await Bot.SendTextMessageAsync(chatId, "Не удалось загрузить файл");
                                        break;
                                }
                                break;

                            case ChatStatemtns.NextMessageIsNewTaskName:
                                editedTask.Name = messageText;
                                editedTask.Save();
                                await Bot.SendTextMessageAsync(chatId, $"Название имзменено");
                                await CheckTaskToFill(editedTask, chatId);
                                break;
                            case ChatStatemtns.NextMessageIsNewTaskDesc:
                                editedTask.Description = messageText;
                                editedTask.Save();
                                await Bot.SendTextMessageAsync(chatId, $"Описание изменено");
                                await CheckTaskToFill(editedTask, chatId);
                                break;
                            case ChatStatemtns.NextMessageIsNewTaskDt:
                                try
                                {
                                    DateTime dt = DateTime.Parse(messageText);
                                    editedTask.EndDate = dt;
                                    editedTask.Save();
                                    await Bot.SendTextMessageAsync(chatId, $"Дата для выполнения изменена");
                                }
                                catch
                                {
                                    await Bot.SendTextMessageAsync(chatId, $"Неверный формат даты");
                                }
                                await CheckTaskToFill(editedTask, chatId);
                                break;
                            case ChatStatemtns.NextMessageIsNewTaskExecutorId:
                                BotUser executor = s.GetObjectByKey<BotUser>(Convert.ToInt32(messageText));
                                if (executor != null)
                                {
                                    editedTask.Executor = executor;
                                    editedTask.Save();
                                    await Bot.SendTextMessageAsync(chatId, $"Исполнитель назначен");
                                    if (editedTask.Executor != null)
                                        await Bot.SendTextMessageAsync(editedTask.Executor.ChatId, "Вам была назначена задача " + editedTask.Name + " выполнить до " + editedTask.EndDate);
                                }
                                else
                                {
                                    await Bot.SendTextMessageAsync(chatId, $"Не удалось найти пользователя с таким ID");
                                }
                                if (editedTask.Executor != null)
                                    await Bot.SendTextMessageAsync(editedTask.Executor.ChatId, "Вам поставлена новая задача", Telegram.Bot.Types.Enums.ParseMode.Default, true, false, 0, new InlineKeyboardMarkup(BotMessageHelper.TaskButtonListView(new List<BotTask>() { editedTask }, nameof(CallbackActions.taskdetailviewstat))));
                                await CheckTaskToFill(editedTask, chatId);
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        await Bot.SendTextMessageAsync(chatId, exception.Message);

                    }
                    editedTask.Save();

                    ChatStatements.Remove(chatStatement);
                }
            }
            else
            {

                BotUser currentUser = PermissionSystem.GetUserByTlgId(e.Message.From.Id);
                string insertedCommand = CommandProcessor.CommandParser(e.Message.Text);
                string commandText = e.Message.Text;
                switch (insertedCommand)
                {
                    case nameof(BotCommandList.start):
                        {
                            try
                            {
                                CommandStartExec(e);
                                await Bot.SendTextMessageAsync(chatId, $"Вы успешно добавлены в базу данных CRM, ожидайте пока администратор даст вам права на использование");
                            }
                            catch
                            {
                                await Bot.SendTextMessageAsync(chatId, $"Произошла ошибка добавления, обратитесь к разработчику");
                            }
                            break;
                        }
                    case nameof(BotCommandList.killmytask):
                        if (currentUser != null && PermissionSystem.HasAccess(currentUser.Oid))
                             CommandKillMyTaskExec(chatId, currentUser.Oid);
                        break;
                    case nameof(BotCommandList.help):
                        {
                            ShowHelp(chatId);
                            break;
                        }
                    case nameof(BotCommandList.allusers):
                        {
                            if (currentUser != null && PermissionSystem.HasAccess(currentUser.Oid))
                            {
                                CommandAllUsersExec(chatId);
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.killuser):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.IsAdministrative(currentUser.Oid))
                                {
                                    CommandKillUsersExec(chatId, commandText);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.ntask):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.HasAccess(currentUser.Oid))
                                {
                                    CommandCreateTaskExec(chatId, commandText, e.Message.From.Username);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.alltask):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.HasAccess(currentUser.Oid))
                                {
                                    CommandShowAllTaskExec(chatId);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.setuser):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.IsAdministrative(currentUser.Oid))
                                {
                                    CommandSetUserExec(chatId, commandText);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.setadm):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.IsAdministrative(currentUser.Oid))
                                {
                                    CommandSetAdmExec(chatId, commandText);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.mytask):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.HasAccess(currentUser.Oid))
                                {
                                    CommandMyTaskExec(e.Message.From, chatId, true);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.tasktome):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.HasAccess(currentUser.Oid))
                                {
                                    CommandTaskToMeExec(e.Message.From, chatId, true);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.mytaskall):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.HasAccess(currentUser.Oid))
                                {
                                    CommandMyTaskExec(e.Message.From, chatId, false);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    case nameof(BotCommandList.tasktomeall):
                        {
                            if (currentUser != null)
                            {
                                if (PermissionSystem.HasAccess(currentUser.Oid))
                                {
                                    CommandTaskToMeExec(e.Message.From, chatId, false);
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(chatId, $"У вас нет прав на это действие");
                            }
                            break;
                        }
                    default:
                        {
                            await Bot.SendTextMessageAsync(chatId, $"Команда {insertedCommand} не распознана");
                            break;
                        }
                }
            }
        }

        private static async void CommandKillMyTaskExec(long chatId, int oid)
        {
            using (Session s = new Session())
            {
                var currUser = s.GetObjectByKey<BotUser>(oid);
                if (currUser != null)
                {
                    var tasks = s.Query<BotTask>().Where(t => t.Appointer.Oid == oid);
                    if (tasks != null)
                    {
                        foreach (var t in tasks)
                        {
                            t.Delete();
                        }
                    }
                    await Bot.SendTextMessageAsync(chatId, $"Поставленные вами задачи удалены");

                }
            }
        }

        private static void AddChatStatement(long chatId, ChatStatemtns statement, int dataId)
        {
            ChatStatements.Add(new ChatStatement() { ChatId = chatId, Statament = statement, BotTaskId = dataId });
        }

        private async static void ChangeTaskState(int taskId, long chatId, string stateName)
        {
            using (Session s = new Session())
            {
                BotTask task = s.GetObjectByKey<BotTask>(taskId);
                if (task != null)
                {
                    task.Status = stateName;
                    task.Save();
                    await Bot.SendTextMessageAsync(chatId, "Статус задачи изменен");
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, "Не удалось поменять статус");
                }
            }
        }

        private async static void ExecuteActionShowFiles(int taskId, long chatId)
        {
            using (Session s = new Session())
            {
                BotTask task = s.GetObjectByKey<BotTask>(taskId);
                if (task != null)
                {
                    if (task.Files != null)
                    {
                        foreach (var file in task.Files)
                        {
                            await Bot.SendDocumentAsync(chatId, new FileToSend(file.FileId));
                        }
                    }
                }
            }
        }

        private async static void ExecuteActionEditTask(CallbackEventArgs args, long chatId)
        {
            await Bot.SendTextMessageAsync(chatId, "Выберите что хотите изменить", Telegram.Bot.Types.Enums.ParseMode.Default, true, false, 0, new InlineKeyboardMarkup(BotMessageHelper.GetTaskEditDetailViewButtons(args.DataId)));
        }

        private static void ExecuteActionDeleteTask(CallbackEventArgs args)
        {
            using (Session s = new Session())
            {
                s.Delete(s.GetObjectByKey<BotTask>(args.DataId));
            }
        }

        private static async void CommandTaskToMeExec(User fromUser, long chatId, bool filteredToDoneAndKill)
        {

            using (Session s = new Session())
            {
                IQueryable<BotTask> tasks = null;
                if (filteredToDoneAndKill)
                {
                    tasks = from task in s.Query<BotTask>()
                            where task.Executor.TelegramId == fromUser.Id
                            && task.Status != "Убита" && task.Status != "Выполнена"
                            select task;
                }
                else
                {
                    tasks = from task in s.Query<BotTask>()
                            where task.Executor.TelegramId == fromUser.Id
                            select task;
                }
                if (tasks.FirstOrDefault() != null)
                {
                    var taskToMe = tasks.ToList();
                    await Bot.SendTextMessageAsync(chatId, "Задачи поставленные вам", Telegram.Bot.Types.Enums.ParseMode.Default, true, false, 0, new InlineKeyboardMarkup(BotMessageHelper.TaskButtonListView(taskToMe, nameof(CallbackActions.taskdetailviewstat))));
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, $"У вас пока нет задач");
                }
            }
        }

        private static async void CommandMyTaskExec(User userFrom, long chatId, bool filteredToDoneAndKill)
        {
            using (Session session = new Session())
            {
                var users = from user in session.Query<BotUser>()
                            where user.TelegramId == userFrom.Id
                            select user;
                var existingUser = users.FirstOrDefault();
                if (existingUser != null)
                {
                    IQueryable<BotTask> userTasks = null;
                    if (filteredToDoneAndKill)
                    {
                        userTasks = from task in session.Query<BotTask>()
                                    where task.Appointer.Oid == existingUser.Oid || task.Appointer.UserName == existingUser.UserName
                                    || task.Appointer.TelegramId == existingUser.TelegramId && task.Status != "Выполнена" && task.Status != "Убита"
                                    select task;
                    }
                    else
                    {
                        userTasks = from task in session.Query<BotTask>()
                                    where task.Appointer.Oid == existingUser.Oid || task.Appointer.UserName == existingUser.UserName
                                    || task.Appointer.TelegramId == existingUser.TelegramId
                                    select task;
                    }
                    if (userTasks != null)
                    {
                        var currUserTasks = userTasks.ToList();
                        await Bot.SendTextMessageAsync(chatId, "Задачи поставленные мной", Telegram.Bot.Types.Enums.ParseMode.Default, true, false, 0, new InlineKeyboardMarkup(BotMessageHelper.TaskButtonListView(currUserTasks, nameof(CallbackActions.taskdetailviewall))));
                    }
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, $"Нет данных");
                }
            }
        }

        private static async void CommandSetAdmExec(long chatId, string commandText)
        {
            int userAccessId = CommandProcessor.ParseCommandIntFirstParametr(commandText);
            if (userAccessId != -1)
            {
                Session session = new Session();
                var users = from user in session.Query<BotUser>()
                            where user.Oid == userAccessId
                            select user;
                var existingUser = users.FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.IsAdministrative = true;
                    existingUser.Save();
                    await Bot.SendTextMessageAsync(chatId, $"Пользователю {existingUser.UserName} предоставлены права администратора");
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, $"Пользователь с ID {userAccessId} не найден");
                }
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId, $"Не верный формат команды, ожидалось число");
            }
        }

        private static async void CommandSetUserExec(long chatId, string command)
        {
            int userAccessId = CommandProcessor.ParseCommandIntFirstParametr(command);
            if (userAccessId != -1)
            {
                Session session = new Session();
                var users = from user in session.Query<BotUser>()
                            where user.Oid == userAccessId
                            select user;
                var existingUser = users.FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.HasAccess = true;
                    existingUser.Save();
                    await Bot.SendTextMessageAsync(chatId, $"Пользователю {existingUser.UserName} предоставлены права на CRM");
                }
                else
                {
                    await Bot.SendTextMessageAsync(chatId, $"Пользователь с ID {userAccessId} не найден");
                }
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId, $"Не верный формат команды, ожидалось число");
            }

        }

        private static void CommandStartExec(MessageEventArgs e)
        {
            Session session = new Session();
            User userTlg = e.Message.From;
            var existingUser = from user in session.Query<BotUser>()
                               where user.TelegramId == userTlg.Id
                               select user;
            var userInDb = existingUser.FirstOrDefault();
            if (userInDb == null)
            {
                userInDb = new BotUser(session);
                userInDb.UserName = userTlg.Username;
                userInDb.FirstName = userTlg.FirstName;
                userInDb.LastName = userTlg.LastName;
                userInDb.ChatId = e.Message.Chat.Id;
                userInDb.TelegramId = userTlg.Id;
            }
            else
            {
                userInDb.UserName = userTlg.Username;
                userInDb.FirstName = userTlg.FirstName;
                userInDb.LastName = userTlg.LastName;
                userInDb.ChatId = e.Message.Chat.Id;
            }
            userInDb.Save();
        }

        private async static void CommandShowAllTaskExec(long chatId)
        {
            Session session = new Session();
            var tasks = session.Query<BotTask>().ToList();

            if (tasks.Count() > 0)
            {
                await Bot.SendTextMessageAsync(chatId, "Все задачи", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, new InlineKeyboardMarkup(BotMessageHelper.TaskButtonListView(tasks, nameof(CallbackActions.taskdetailviewdeny))));
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId, "Задач пока нет");
            }
        }

        private static async void CommandCreateTaskExec(long chatId, string command, string appointerUserName)
        {
            try
            {
                using (Session session = new Session())
                {
                    BotTask newTask = new BotTask(session);
                    CommandProcessor.ParseNewTaskCommand(newTask, command, session, appointerUserName);

                    newTask.Status = "Новая";
                    newTask.Save();
                    var res = await CheckTaskToFill(newTask, chatId);
                    if (res)
                    {
                        await Bot.SendTextMessageAsync(chatId, $"Задача {newTask.Name} успешно добавлена в базу данных");
                        if (newTask.Executor != null)
                            await Bot.SendTextMessageAsync(newTask.Executor.ChatId, "Вам поставлена новая задача", Telegram.Bot.Types.Enums.ParseMode.Default, true, false, 0, new InlineKeyboardMarkup(BotMessageHelper.TaskButtonListView(new List<BotTask>() { newTask }, nameof(CallbackActions.taskdetailviewstat))));
                    }
                }
            }
            catch (Exception e)
            {
                await Bot.SendTextMessageAsync(chatId, $"Ошибка при создании задачи, {e.Message}");
            }
        }

        private static async void ShowHelp(long chatId)
        {
            await Bot.SendTextMessageAsync(chatId, BotMessageHelper.GetHelpMessage());
        }

        private static async void CommandKillUsersExec(long chatId, string commandText)
        {
            int userToDeletingId = CommandProcessor.GetDeletedUserId(commandText);
            Session session = new Session();
            var deletingUser = session.GetObjectByKey<BotUser>(userToDeletingId);
            if (deletingUser != null)
            {
                deletingUser.Delete();
                await Bot.SendTextMessageAsync(chatId, $"Пользователь c id {userToDeletingId} успешно удален");
            }
            else
            {
                await Bot.SendTextMessageAsync(chatId, $"Пользователь c id {userToDeletingId} не найден");
            }
        }

        private static async void CommandAllUsersExec(long chatId)
        {
            using (Session session = new Session())
            {
                var users = session.Query<BotUser>();
                StringBuilder usersList = new StringBuilder();
                foreach (BotUser user in users)
                {
                    usersList.AppendFormat("\n\t id: {0},\n\t username: {1} \n\t first name: {2}\n\t last name: {3}\n\t telegram ID: {4} \n\t Право на CRM {5} \n\t Администратор {6} \n *************\n", user.Oid, user.UserName, user.FirstName, user.LastName, user.TelegramId, user.HasAccess, user.IsAdministrative);
                }

                await Bot.SendTextMessageAsync(chatId, usersList.ToString());
            }
        }


    }
}
