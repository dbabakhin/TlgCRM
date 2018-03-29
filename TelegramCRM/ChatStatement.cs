using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramCRM
{
    public class ChatStatement
    {
        public long ChatId
        {
            get; set;
        }

        public ChatStatemtns Statament
        {
            get; set;
        }

        public int BotTaskId
        {
            get; set;
        }
    }

    public enum ChatStatemtns
    {
        NextMessageIsnewTaskData,NextMessageIsNewTaskName, NextMessageIsNewTaskDesc, NextMessageIsNewTaskDt, NextMessageIsNewTaskExecutorId, NextMessageIsNewTaskPriority,NextMessageIsFiles,NextMessageIsTaksName, NextMessageIsTaskDesc, NextMessageIsTaskSDate, NextMessageIsTaskEDate, NextMessageIdTaskPriorirty, NextMessageIsTaskExecutorId, NextMessageIsTaskStatus
    }
}
