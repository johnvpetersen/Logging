using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private  Serilog.Core.Logger _logger = null;

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
/*
Create/Invoke Log: https://localhost:5001/Home/InvokeLog?Source=b6beca04-04ef-4765-85ff-ee7c3f0c173b&AddFile=YES&AddEventLog=YES
Retrieve Log: https://localhost:5001/Home/GetLog?Source=b6beca04-04ef-4765-85ff-ee7c3f0c173b&DeleteLog=YES
*/
        public string InvokeLog()
        {
           var retVal = string.Empty;
           var options = HomeController.ParseQueryLoggerOptions(Request.Query);
           if (options == 0)
              return  "You must choose at least 1 logging option: Console, Debug, EventLog, or File."; 
            var source = Request.Query["Source"].ToString();
               
            if (string.IsNullOrEmpty(source))
               source  = Guid.NewGuid().ToString();

            using (var logger = App.Logger.GetLogger(options,source).Value)
            {
               _logger?.Information("This log entry was generated in the InvokeLog Controller Method: {source}",source);
            }
            
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