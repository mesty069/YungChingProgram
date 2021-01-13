using System.Diagnostics;
using System.Web;
using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using NLog;
using YungChingProgram.Models.Database;

namespace YungChingProgram._GeneralLibrary
{
    public class LogManagement
    {
        #region Enum

        /// <summary>
        /// 事件層級(Trace、Debug、Info、Warn、Error、Fatal)
        /// </summary>
        public enum EventLevel
        {
            Trace, Debug, Info, Warn, Error, Fatal
        }

        public enum SystemName
        {
            系統操作, 基本作業, 匯出入作業
        }

        #endregion

        private class RecordLogInfo
        {
            public string ADAcnt { get; set; }
            public string LogSystem { get; set; }
            public string SystemName { get; set; }
            public string LogLocation { get; set; }
            public string Action { get; set; }
            public string LogType { get; set; }
            public string EventLevel { get; set; }
            public string Message { get; set; }
            public string Json { get; set; }
            public string Exception { get; set; }

        }

        /// <summary>
        /// Log的類別(AP、OP、AA、Interfacing)
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 系統AP日誌：紀錄所有 AP相關之 Log資訊，若系統發生錯誤，必須在問題發生當下紀錄錯誤相關資訊以便IT人員找尋問題發生原因。
            /// </summary>
            AP,
            /// <summary>
            /// 系統操作日誌：紀錄所有使用者從登入以後到登出以前所有的操作軌跡，包括跳轉頁面、操作行為等。
            /// </summary>
            OP,
            /// <summary>
            /// 稽核日誌：紀錄使用者資訊登入/登出資訊，作為安全控管之用。
            /// </summary>
            AA,
            /// <summary>
            /// 系統介接日誌：以一次對外介接為一筆紀錄，主要紀錄外部介接相關資訊，以利問題查找及資訊記錄。
            /// </summary>
            Interfacing
        }

        /// <summary>
        /// 使用者操作行為
        /// </summary>
        public enum LogAction
        {
            //資料庫行為
            SQL_Query,
            SQL_Insert,
            SQL_Update,
            SQL_Delete,
            SQL_InsertOrUpdate,
            //使用者行為
            USER_Login,
            USER_Logout,
            //追蹤行為
            USER_Trace_EnterPage,
            //操作行為
            USER_Action_Query,
            USER_Action_Insert,
            USER_Action_Update,
            USER_Action_Delete,
            USER_Action_InsertOrUpdate,
            //系統行為
            SYS_CallApi,
            SYS_FunctionStart,
            SYS_FunctionEnd,
            SYS_FunctionCheckPoint,
            //錯誤行為
            Error_NullException,
            Error_FunctionError,
            Error_ValueOutOfDefine,
            Error_InsertDataExist,
            Error_UpdateDataNotExist,
            Error_DeleteDataNotExist,
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 將訊息寫入LOG(包含同時寫入NLog與Database log)
        /// </summary>
        /// <param name="adAccount">目前使用者帳號</param>
        /// <param name="logType">Log的類別</param>
        /// <param name="eventLevel">Log等級</param>
        /// <param name="logAction">Log動作</param>
        /// <param name="logMessage">Log訊息</param>
        /// <param name="logJsonObject">如有操作物件，直接將物件填入，會自動產生對應的Json</param>
        /// <param name="logException">如有Exception物件可直接填入，會自動正規化</param>
        public void LogInfoWriter(string adAccount, LogType logType, EventLevel eventLevel, LogAction logAction, SystemName systemName, string logMessage, object logJsonObject, Exception logException)
        {
            //將LogInfo物件轉為RecordLogInfo物件
            var recordLogInfo = ConvertLogInfo(adAccount, logType, eventLevel, logAction, systemName, logMessage, logJsonObject, logException);
            try
            {
                var st = new StackTrace(true);
                //取得呼叫當前方法上一層(GetFrame(1))類別的屬性
                var methodInfo = st.GetFrame(1).GetMethod();
                //無法追朔呼叫當前方法的父類別
                if (methodInfo.DeclaringType == null)
                {
                    NLogWriter("StackTrace is null", EventLevel.Fatal);
                    NLogWriter(JsonConvert.SerializeObject(recordLogInfo), EventLevel.Fatal);
                    return;
                }
                //取得呼叫當前方法的父類別Full class name
                recordLogInfo.LogSystem = methodInfo.DeclaringType.FullName;
                //取得呼叫當前方法的父類別Function name
                recordLogInfo.LogLocation = methodInfo.Name;
                //寫入NLog
                NLogWriter(recordLogInfo);
            }
            catch (Exception e)
            {
                NLogWriter(BuildExceptionMessage(e), EventLevel.Fatal);
                NLogWriter(JsonConvert.SerializeObject(recordLogInfo), EventLevel.Fatal);
            }
        }

        #region NLog writer

        /// <summary>
        /// 請避免使用此方法，請用LogInfoWriter，此方法僅提供特殊情況使用。
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="logLevel"></param>
        public void NLogWriter(string logMessage, EventLevel logLevel)
        {
            switch (logLevel)
            {
                case EventLevel.Trace:
                    Logger.Trace(logMessage);
                    break;
                case EventLevel.Debug:
                    Logger.Debug(logMessage);
                    break;
                case EventLevel.Info:
                    Logger.Info(logMessage);
                    break;
                case EventLevel.Warn:
                    Logger.Warn(logMessage);
                    break;
                case EventLevel.Error:
                    Logger.Error(logMessage);
                    break;
                case EventLevel.Fatal:
                    Logger.Fatal(logMessage);
                    break;
            }
        }

        private void NLogWriter(RecordLogInfo recordLogInfo)
        {
            //RecordLogInfo物件轉為Json寫入NLog中
            var recordMessage = JsonConvert.SerializeObject(recordLogInfo);
            switch (recordLogInfo.EventLevel)
            {
                case "Trace":
                    NLogWriter(recordMessage, EventLevel.Trace);
                    break;
                case "Debug":
                    NLogWriter(recordMessage, EventLevel.Debug);
                    break;
                case "Info":
                    NLogWriter(recordMessage, EventLevel.Info);
                    break;
                case "Warn":
                    NLogWriter(recordMessage, EventLevel.Warn);
                    break;
                case "Error":
                    NLogWriter(recordMessage, EventLevel.Error);
                    break;
                case "Fatal":
                    NLogWriter(recordMessage, EventLevel.Fatal);
                    break;
            }
        }

        #endregion

        //將LogInfo物件轉為Json，便於寫入NLog
        private RecordLogInfo ConvertLogInfo(string adAccount, LogType logType, EventLevel eventLevel, LogAction logAction, SystemName systemName, string logMessage, object logJsonObject, Exception logException)
        {
            var recordLogInfo = new RecordLogInfo
            {
                ADAcnt = adAccount,
                LogType = logType.ToString(),
                EventLevel = eventLevel.ToString(),
                SystemName = systemName.ToString(),
                Action = logAction.ToString(),
                Message = logMessage,
                Exception = BuildExceptionMessage(logException)
            };
            try
            {
                try
                {
                    recordLogInfo.Json = JsonConvert.SerializeObject(logJsonObject);
                    return recordLogInfo;
                }
                catch (Exception)
                {
                    foreach (var item in (IList)logJsonObject)
                    {
                        recordLogInfo.Json += JsonConvert.SerializeObject(item);
                    }
                    return recordLogInfo;
                }
            }
            catch (Exception e)
            {
                var message = new StringBuilder();
                message.AppendLine();
                message.AppendLine("ConvertLogInfo error, object convert json error!");
                message.AppendLine(BuildExceptionMessage(e));
                NLogWriter(message.ToString(), EventLevel.Error);
                return recordLogInfo;
            }
        }

        /// <summary>
        /// Builds the exception message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>After build exception message</returns>
        private static string BuildExceptionMessage(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            var logException = exception;
            if (exception.InnerException != null)
            {
                logException = exception.InnerException;
            }

            var message = new StringBuilder();
            message.AppendLine();
            if (HttpContext.Current != null)
            {
                message.AppendLine("Error in Path : " + HttpContext.Current.Request.Path);
                // Get the QueryString along with the Virtual Path
                message.AppendLine("Raw Url : " + HttpContext.Current.Request.RawUrl);
            }
            // Type of Exception
            message.AppendLine("Type of Exception : " + logException.GetType().Name);
            // Get the error message
            var exceptionMessage = logException.Message;
            message.AppendLine("Message : " + logException.Message);
            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                if (exceptionMessage.IndexOf("InnerException", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    if (logException.InnerException != null)
                    {
                        message.AppendLine("Inner Message : " + logException.InnerException.Message);
                    }
                }
            }
            // Source of the message
            message.AppendLine("Source : " + logException.Source);
            // Stack Trace of the error
            message.AppendLine("Stack Trace : " + logException.StackTrace);
            // Method where the error occurred
            message.AppendLine("TargetSite : " + logException.TargetSite);
            return message.ToString();
        }
    }
}