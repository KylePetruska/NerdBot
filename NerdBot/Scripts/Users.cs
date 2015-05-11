using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NerdBot
{
    public class Users : IEquatable<Users>
    {

        private int _numWarnings;

        private DateTime _lastMsgTime;

        private Irc _irc;
        private Logger _logger;

        private MySqlCommand _cmd;

        public int UserLevel { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }

        public enum WarnType
        {
            General,
            Link,
            EmoteSpam,
            Spam,
            Caps
        }

        public Users(string name)
        {
            Name = name.ToLower();
            _irc = formMain.mainForm.chat;
            _logger = new Logger("Users");

            LoadFromDB();
        }

        private void LoadFromDB()
        {
            String sql = "SELECT * FROM users WHERE channel='" + formMain.mainForm.Channel.Substring(1) + "' AND username='" + Name + "'";
            try
            {
                using (_cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
                {
                    using (MySqlDataReader r = _cmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                UserLevel = (int)r["user_level"];
                            }
                        }
                        else
                        {
                            r.Close();
                            AddUserToDB();
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log("Error getting user SQL:", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
            }
        }

        private void AddUserToDB()
        {
            int level = 0;

            if (Name.ToLower() == formMain.mainForm.Channel.Substring(1).ToLower())
            {
                level = 3;
            }
            else if (IsMod())
            {
                level = 2;
            }

            String sql = String.Format("INSERT INTO users (username, user_level, channel) VALUES (\"{0}\", {1}, \"{2}\");", Name, level, formMain.mainForm.Channel.Substring(1));
            try
            {
                using (_cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
                {
                    _cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                _logger.Log("Error adding user SQL:", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
            }
            
        }

        public void SetUserLevel(int level)
        {
            String sql = String.Format("UPDATE users SET user_level={0} WHERE username=\"{1}\" AND channel=\"{2}\";", level, Name, formMain.mainForm.Channel.Substring(1));
            Console.WriteLine(sql);
            try
            {
                using (_cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
                {
                    _cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                _logger.Log("Error setting user level SQL:", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
            }
            finally
            {
                UserLevel = level;
            }
        }

        private bool IsMod()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("http://tmi.twitch.tv/group/user/" + formMain.mainForm.Channel.Substring(1) + "/chatters");
                JObject parsedData = JObject.Parse(result);

                foreach (string mod in parsedData["chatters"]["moderators"])
                {
                    if (mod == Name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Warn(WarnType type)
        {
            _numWarnings++;

            if (_numWarnings >= 4)//TODO: Make this configurable
            {
                Ban(type);
                return;
            }

            string msg;
            msg = GetWarnTypeMessage(type);

            _logger.Log("Warning: " + Name + ". Reason: " + msg, Logger.LogType.Debug);
            TimeOut(30);//TODO: Make this based on the warning number
            _irc.SendMessage(Name + msg + " [WARN]. This is warning #" + _numWarnings, Irc.QueuePriorty.High);
        }

        private void TimeOut(int length)
        {
            _irc.SendMessage("/timeout " + Name + " " + length, Irc.QueuePriorty.High);
        }

        public void Ban(WarnType type)
        {
            string msg;

            msg = GetWarnTypeMessage(type);

            _logger.Log("Banning: " + Name + ". Reason: " + msg, Logger.LogType.Debug);
            _irc.SendMessage("/ban " + Name, Irc.QueuePriorty.High);
            _irc.SendMessage(Name + msg + " [BAN]", Irc.QueuePriorty.High);
        }

        private string GetWarnTypeMessage(WarnType type)
        {
            string warnMsg;

            switch (type)
            {
                case WarnType.General:
                    warnMsg = " this is a general warning. Cut it out.";
                    break;
                case WarnType.Link:
                    warnMsg = " no links in chat please.";
                    break;
                case WarnType.EmoteSpam:
                    warnMsg = " please do not spam emotes.";
                    break;
                case WarnType.Spam:
                    warnMsg = ", what is this a Monty Python skit. Cut the spam out.";
                    break;
                case WarnType.Caps:
                    warnMsg = " --- Loud noises! No need for the caps lock.";
                    break;
                default:
                    warnMsg = " this is a general warning. Stop what you are doing.";
                    break;
            }
            return warnMsg;
        }

        public bool Equals(Users other)
        {
            return this.Name == other.Name;
        }
    }
}
