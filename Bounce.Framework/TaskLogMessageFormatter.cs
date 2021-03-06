using System;

namespace Bounce.Framework {
    internal class TaskLogMessageFormatter : ILogMessageFormatter {
        private readonly ITask task;

        public TaskLogMessageFormatter(ITask task) {
            this.task = task;
        }

        public string FormatLogMessage(DateTime now, LogLevel logLevel, object message) {
            return String.Format("{0} {1} {2} {3}", now, task, logLevel.ToString().ToLower(), message);
        }
    }
}