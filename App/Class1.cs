using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Serilog;


namespace App
{
    public class Logger
    {

       public enum LoggerOptions {
           AddConsole = 1,
           AddDebug = 2,
           AddFile = 4,
           AddEventLog = 8
       } 
  
        public static  KeyValuePair<string,Serilog.Core.Logger> GetLogger(int options, IConfiguration configuration, string id = null) {
            id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
            var builder = new LoggerConfiguration();
               if (((int)LoggerOptions.AddConsole & options) == (int)LoggerOptions.AddConsole)   
                  builder.WriteTo.Console();
               if (((int)LoggerOptions.AddDebug & options) == (int)LoggerOptions.AddDebug)   
                  builder.WriteTo.Debug();
               if (((int)LoggerOptions.AddFile & options) == (int)LoggerOptions.AddFile)   
                  builder.WriteTo.File($"_{id}.log");
               if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                  (((int)LoggerOptions.AddEventLog & options) == (int)LoggerOptions.AddEventLog)) {
                  builder.WriteTo.EventLog(id, manageEventSource: true);
          }
            var logger = builder.CreateLogger();
            logger.Information("Logger {logFile} generated.",id);
            return new KeyValuePair<string, Serilog.Core.Logger>(id,logger);
        }
    }
}
