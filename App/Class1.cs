﻿using System;
using System.Collections.Generic;
using System.IO;
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
  
  
        public static KeyValuePair<string,Serilog.Core.Logger> GetLogger(string appSettings, string source = null) {
            source = string.IsNullOrEmpty(source) ? Guid.NewGuid().ToString() : source;
            
            var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(appSettings)
                .Build();

            var logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();


            return new KeyValuePair<string, Serilog.Core.Logger>(source,logger);
        }
        public static  KeyValuePair<string,Serilog.Core.Logger> GetLogger(int options, string source = null) {
            source = string.IsNullOrEmpty(source) ? Guid.NewGuid().ToString() : source;
            var builder = new LoggerConfiguration();
               if (((int)LoggerOptions.AddConsole & options) == (int)LoggerOptions.AddConsole)   
                  builder.WriteTo.Console();
               if (((int)LoggerOptions.AddDebug & options) == (int)LoggerOptions.AddDebug)   
                  builder.WriteTo.Debug();
               if (((int)LoggerOptions.AddFile & options) == (int)LoggerOptions.AddFile)   
                  builder.WriteTo.File($"_{source}.log");
               if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                  (((int)LoggerOptions.AddEventLog & options) == (int)LoggerOptions.AddEventLog)) {
                  builder.WriteTo.EventLog(source, manageEventSource: true);
          }
            var logger = builder.CreateLogger();
               logger.Information("Logger {logFile} generated.",source);
            return new KeyValuePair<string, Serilog.Core.Logger>(source,logger);
        }
    }
}
