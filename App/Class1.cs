using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;

namespace App
{
    public class Logger
    {

       public enum LoggerOptions {
           AddConsole = 1,
           AddDebug = 2,
           AddSerilog = 4,
           AddEventLog = 8
       } 
  
        public static  KeyValuePair<string,Microsoft.Extensions.Logging.ILogger> GetLogger(int options, IConfiguration configuration) {
            
            var logFile = Guid.NewGuid().ToString();
            
            var logger =     LoggerFactory.Create(
                builder => {
                     if (((int)LoggerOptions.AddConsole & options) == (int)LoggerOptions.AddConsole)   
                        builder.AddConsole();
                     if (((int)LoggerOptions.AddDebug & options) == (int)LoggerOptions.AddDebug)   
                        builder.AddDebug();
                     if (((int)LoggerOptions.AddSerilog & options) == (int)LoggerOptions.AddSerilog)   
                        builder.AddSerilog(new LoggerConfiguration().WriteTo.File($"_{logFile}.log").CreateLogger());
                     if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                           (((int)LoggerOptions.AddEventLog & options) == (int)LoggerOptions.AddEventLog)) {
                        var eventLogSettings = new EventLogSettings();
                        configuration.GetSection("EventLogSettings").Bind(eventLogSettings);
                        builder.AddEventLog(eventLogSettings);
                     }   
                }
            ).CreateLogger(logFile);
            configuration.GetSection("Logging").Bind(logger);
            logger.LogInformation("Logger {logFile} generated.",logFile);
            return new KeyValuePair<string, Microsoft.Extensions.Logging.ILogger>(logFile,logger);
        }
    }
}
