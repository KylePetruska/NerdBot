using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace NerdBot
{
    public interface ICommands
    {
        
    }

    public class Commands : ICommands
    {
        private MySqlCommand cmd;

        public string[,] custCmds = new string[20,3];

        private string _channel;

        public Commands(string channel)
        {
            _channel = channel;
            InitializeCustomCommands();
        }

        private void InitializeCustomCommands()
        {
            custCmds = new string[20, 3];

            int cmdNum = 0;
            String sql = "SELECT * FROM commands WHERE channel='" + _channel + "'";
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        custCmds[cmdNum, 0] = r["command"].ToString();
                        custCmds[cmdNum, 1] = r["output"].ToString();
                        custCmds[cmdNum, 2] = r["level"].ToString();
                        cmdNum++;
                    }
                }
            }
        }

        public bool CmdExists(String command)
        {
            String sql = "SELECT * FROM commands";
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        if (r["command"].ToString().Equals(command, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void AddCommand(String command, int level, String output)
        {
            String sql = String.Format("INSERT INTO commands (command, level, output) VALUES (\"{0}\", {1}, \"{2}\");", command, level, output);
            //String sql = "INSERT INTO commands (command, level, output) VALUES (\"" + command + "\", " + level + ", \"" + output + "\");";
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                cmd.ExecuteNonQuery();
            }
            InitializeCustomCommands();
        }

        public void RemoveCommand(String command)
        {
            String sql = "DELETE FROM commands WHERE command= \"" + command + "\";";
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                cmd.ExecuteNonQuery();
            }
            InitializeCustomCommands();
        }

        public int LevelRequired(String command)
        {
            String sql = String.Format("SELECT * FROM commands WHERE command = \"{0}\";", command);
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        return int.Parse(r["level"].ToString());
                    }
                    return 0;
                }
            }
        }

        public string GetList()
        {
            StringBuilder list = new StringBuilder();
            String sql = "SELECT * FROM commands WHERE channel='" + _channel + "'";
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Append(r["command"].ToString() + ", ");
                    }
                }
            }
            //Clean up
            return "Current list of commands: " + list.ToString() + "!commands, !allow, !uptime, !follower";
        }

        public string GetOutput(String command)
        {
            String sql = "SELECT * FROM commands WHERE command = \"" + command + "\";";
            using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        return r["output"].ToString();
                    }
                    return "";
                }
            }
        }
    }
}

