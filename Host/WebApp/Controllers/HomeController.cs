using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private  Serilog.Core.Logger _logger;
        private readonly string _logID;

        public HomeController()
        {
        }

        public string Index()
        {
            var id = Request.Query["Log"].ToString();
            if (!String.IsNullOrEmpty(id)) {
                       var options = (int)App.Logger.LoggerOptions.AddFile + (int)App.Logger.LoggerOptions.AddDebug + (int)App.Logger.LoggerOptions.AddEventLog;
                       _logger = App.Logger.GetLogger(options,id).Value;        
            }

            _logger?.Information(id);
            _logger?.Dispose();
            
            return id;
        }

        public string GetLog()
        {
            var id = Request.Query["Log"].ToString();
            if (string.IsNullOrEmpty(id))
               return null;

           var options = (int)App.Logger.LoggerOptions.AddFile + (int)App.Logger.LoggerOptions.AddDebug + (int)App.Logger.LoggerOptions.AddEventLog;
           _logger = App.Logger.GetLogger(options,id).Value;        

            _logger?.Information("Will be deleting Log: {id}",id);


            var logFile = $"_{id}.log";
            var content  = System.IO.File.ReadAllText(logFile);
            var delLog = Request.Query["DeleteLog"].ToString();
            if (delLog == "YES")
               System.IO.File.Delete(logFile);
            return content;
        }

    }
}
