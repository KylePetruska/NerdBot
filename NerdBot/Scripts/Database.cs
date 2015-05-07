using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;

namespace NerdBot
{
    public class Database
    {
        public MySqlConnection myDB;
        private MySqlCommand _cmd;
        private string _channel, _dbIp, _dbPass, _dbTable, _dbUser;
        private Logger _logger;

        public Database()
        {
            _logger = new Logger("Database");

            _channel = NerdBot.Properties.Settings.Default.channel;
            _dbIp = Properties.Settings.Default.databaseIP;
            _dbPass = Properties.Settings.Default.databasePass;
            _dbTable = Properties.Settings.Default.databaseTable;
            _dbUser = Properties.Settings.Default.databaseUser;
            
            InitializeDB();
        }

        private void InitializeDB()
        {
            myDB = new MySqlConnection("Server=" + _dbIp + ";Database=" + _dbTable + ";Uid=" + _dbUser + ";password=" + _dbPass + ";");

            try
            {
                myDB.Open();
            }

            catch (MySqlException e)
            {
                _logger.Log("Unable to connect to database", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
            }

            if (myDB.State == System.Data.ConnectionState.Open)
            {
                String sql = "CREATE TABLE IF NOT EXISTS commands (id INTEGER PRIMARY KEY, command TEXT, level INTEGER DEFAULT 0, output TEXT DEFAULT null);";

                using (_cmd = new MySqlCommand(sql, myDB))
                {
                    _cmd.ExecuteNonQuery();
                }

                sql = "CREATE TABLE IF NOT EXISTS autobroadcast (id INTEGER PRIMARY KEY, channel TEXT, message TEXT);";

                using (_cmd = new MySqlCommand(sql, myDB))
                {
                    _cmd.ExecuteNonQuery();
                }
            }
        }

        private bool TableExists(String table)
        {
            String sql = "SELECT COUNT(*) FROM sqlite_master WHERE name = '" + table + "';";
            try
            {
                using (_cmd = new MySqlCommand(sql, myDB))
                {
                    using (MySqlDataReader r = _cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (int.Parse(r["COUNT(*)"].ToString()) != 0)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            catch (SqlException e)
            {
                _logger.Log("Unable to connect to database", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
                return false;
            }
        }

        private bool TableHasData(String table)
        {
            String sql = "SELECT * FROM '" + table + "';";

            using (_cmd = new MySqlCommand(sql, myDB))
            {
                using (MySqlDataReader r = _cmd.ExecuteReader())
                {
                    if (r.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}