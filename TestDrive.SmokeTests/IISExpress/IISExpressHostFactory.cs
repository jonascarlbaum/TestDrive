using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace TestDrive.SmokeTests.IISExpress
{
    public class IISExpressHostFactory
    {
        public static IISExpressHost CreateDefaultInstance()
        {
            var targetProjectFolderName = ConfigurationManager.AppSettings["TargetProjectFolderName"];
            var applicationPath = GetApplicationPath(targetProjectFolderName);

            var portNumber = Convert.ToInt32(ConfigurationManager.AppSettings["Server:Port"]);

            return new IISExpressHost(applicationPath, portNumber);
        }

        private static string GetApplicationPath(string targetProjectFolderName)
        {
            var assembly = Assembly.GetAssembly(typeof(Web.Global));
            var index = assembly.Location.LastIndexOf("\\TestDrive.SmokeTests\\");
            var webPath = $"{assembly.Location.Remove(index)}\\TestDrive.Web";
            
            return webPath;
        }
    }
}
