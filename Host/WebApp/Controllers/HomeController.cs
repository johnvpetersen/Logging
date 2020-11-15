using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private  Serilog.Core.Logger _logger;

        public HomeController()
        {
        }


        public static int ParseQueryLoggerOptions(IQueryCollection queryCollection) {
            var retVal = 0;
            retVal = retVal + queryCollection["AddFile"].ToString() == "YES" ? (int)App.Logger.LoggerOptions.AddFile : 0;
            retVal = retVal + queryCollection["AddConsole"].ToString() == "YES" ? (int)App.Logger.LoggerOptions.AddConsole : 0;
            retVal = retVal + queryCollection["AddDebug"].ToString() == "YES" ? (int)App.Logger.LoggerOptions.AddDebug : 0;
            retVal = retVal + queryCollection["AddEventLog"].ToString() == "YES" ? (int)App.Logger.LoggerOptions.AddEventLog : 0;
            return retVal;

        }

        public string Index()
        {
           var retVal = string.Empty;
           var options = HomeController.ParseQueryLoggerOptions(Request.Query);
           if (options == 0)
              retVal =  "You must choose at least 1 logging option: Console, Debug, EventLog, or File."; 
            var source = Request.Query["Source"].ToString();
            if (string.IsNullOrEmpty(source))
              retVal += " You must choose at least 1 logging option: Console, Debug, EventLog, or File."; 

            if (retVal.Length > 0)
               return retVal.Trim();

            _logger = App.Logger.GetLogger(options,source).Value;        

            _logger?.Information(source);
            _logger?.Dispose();
            
            return source;
        }

        public string GetLog()
        {
            var id = Request.Query["Log"].ToString();
            if (string.IsNullOrEmpty(id))
               return null;

           var options = (int)App.Logger.LoggerOptions.AddFile + (int)App.Logger.LoggerOptions.AddDebug + (int)App.Logger.LoggerOptions.AddEventLog;
           _logger = App.Logger.GetLogger(options,id).Value;        

            _logger?.Information("Will be deleting Log: {id}",id);
            _logger?.Dispose();


            var logFile = $"_{id}.log";
            var content  = System.IO.File.ReadAllText(logFile);
            var delLog = Request.Query["DeleteLog"].ToString();
            if (delLog == "YES")
               System.IO.File.Delete(logFile);
            return content;
        }

    }
}
