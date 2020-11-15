using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private  Serilog.Core.Logger _logger;

        public static int ParseQueryLoggerOptions(IQueryCollection queryCollection) {
            var retVal = 0;
            if (queryCollection["AddConsole"].ToString() == "YES")
               retVal = retVal +   (int)App.Logger.LoggerOptions.AddConsole;
            if (queryCollection["AddDebug"].ToString() == "YES")
               retVal = retVal +   (int)App.Logger.LoggerOptions.AddDebug;
            if (queryCollection["AddEventLog"].ToString() == "YES")
               retVal = retVal +   (int)App.Logger.LoggerOptions.AddEventLog;
            if (queryCollection["AddFile"].ToString() == "YES")
               retVal = retVal +   (int)App.Logger.LoggerOptions.AddFile;
               return retVal;

        }

        public string InvokeLog()
        {
           var retVal = string.Empty;
           var options = HomeController.ParseQueryLoggerOptions(Request.Query);
           if (options == 0)
              return  "You must choose at least 1 logging option: Console, Debug, EventLog, or File."; 
            var source = Request.Query["Source"].ToString();
               
            if (string.IsNullOrEmpty(source))
               source  = Guid.NewGuid().ToString();

            _logger = App.Logger.GetLogger(options,source).Value;        

            _logger?.Information("This log entry was generated in the InvokeLog Controller Method: {source}",source);
            _logger?.Dispose();
            
            return source;
        }

        public string GetLog()
        {
            var source = Request.Query["Source"].ToString();
            if (string.IsNullOrEmpty(source))
               return null;

            var logFile = $"_{source}.log";
            var content  = System.IO.File.ReadAllText(logFile);
            var delLog = Request.Query["DeleteLog"].ToString();
            if (delLog == "YES")
               System.IO.File.Delete(logFile);
            return content;
        }
    }
}