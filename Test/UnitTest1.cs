using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetLogger()
        {
        IConfiguration _configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

        var options = (int)App.Logger.LoggerOptions.AddSerilog + (int)App.Logger.LoggerOptions.AddDebug ;

      

        var sut = App.Logger.GetLogger(options,_configuration);

        sut.Value.LogInformation("Test");


        }
    }

/**********************************/









}
