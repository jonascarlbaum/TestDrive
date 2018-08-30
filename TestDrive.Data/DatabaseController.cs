using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Mvc;
using EPiServer;
using EPiServer.ServiceLocation;

namespace TestDrive.Data
{
    public class DatabaseController : Controller
    {
        public ActionResult RestoreDB()
        {
            var resetTimer = Stopwatch.StartNew();
            ResetEPiDatabase
               (ConfigurationManager.AppSettings["database:test:database-backup"],
                ConfigurationManager.AppSettings["database:test:database-name"]);
            resetTimer.Stop();
			
            return Json(
                new
                {
                    ResetEPiDatabaseFromBackupFileIn = resetTimer.ElapsedMilliseconds + " ms"
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RestoreContentRich()
        {
            var resetTimer = Stopwatch.StartNew();
            ResetEPiDatabase
               (ConfigurationManager.AppSettings["database:content-rich:database-backup"],
                ConfigurationManager.AppSettings["database:content-rich:database-name"]);
            resetTimer.Stop();
			
            return Json(
                new
                {
                    ResetEPiDatabaseFromBackupFileIn = resetTimer.ElapsedMilliseconds + " ms"
                }, JsonRequestBehavior.AllowGet);
        }

        private void ResetEPiDatabase(string backup, string databaseName)
        {
            RestoreFromBackupFile(backup, databaseName);
            ServiceLocator.Current.GetInstance<IContentCacheRemover>().Clear();
        }

        private void RestoreFromBackupFile(string backup, string databaseName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EPiServerDB"].ConnectionString;

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                const string useMaster = "USE master";
                var useMasterCommand = new SqlCommand(useMaster, con);
                useMasterCommand.ExecuteNonQuery();

                var alter1 = @"ALTER DATABASE [" + databaseName + "] SET Single_User WITH Rollback Immediate";
                var alter1Cmd = new SqlCommand(alter1, con);
                alter1Cmd.ExecuteNonQuery();

                var restore = @"RESTORE DATABASE [" + databaseName + "] FROM DISK = N'" + backup +
                              @"' WITH REPLACE,  NOUNLOAD,  STATS = 10";
                var restoreCmd = new SqlCommand(restore, con);
                restoreCmd.ExecuteNonQuery();

                var alter2 = @"ALTER DATABASE [" + databaseName + "] SET Multi_User";
                var alter2Cmd = new SqlCommand(alter2, con);
                alter2Cmd.ExecuteNonQuery();
            }
        }
    }
}
