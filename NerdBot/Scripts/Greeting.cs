using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace NerdBot
{
    public class Greeting
    {
        private string _greeting;

        private Logger _logger;
        private Irc _irc;
        private formMain _main;

        private JoinGreetFrequency joinGreetFrequency { get; set; }

        public enum JoinGreetFrequency
        {
            Everytime,
            Daily,
            Once
        }

        public Greeting(Irc irc)
        {
            _main = formMain.mainForm;
            _logger = new Logger("Greeting");
            _irc = irc;

            joinGreetFrequency = (JoinGreetFrequency)Properties.Settings.Default.joinGreetFrequency;
            _greeting = Properties.Settings.Default.joinGreeting;
        }

        public void HandleGreeting(String user)
        {

            if (joinGreetFrequency == JoinGreetFrequency.Everytime)
            {
                SendGreeting(user);
            }
            else
            {
                MySqlCommand cmd;

                String sql = "SELECT * FROM users WHERE channel='" + _main.Channel.Substring(1) + "' AND username='" + user.ToLower() + "'";

                using (cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
                {
                    using (MySqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.HasRows)
                        {
                            r.Close();
                            MySqlCommand cmd2;
                            SendGreeting(user);
                            string date = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                            String insert = String.Format("INSERT INTO users (channel, username, last_greeted) VALUES (\"{0}\", \"{1}\", \"{2}\");", _main.Channel.Substring(1), user.ToLower(), date);

                            using (cmd2 = new MySqlCommand(insert, formMain.mainForm.db.myDB))
                            {
                                cmd2.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            while (r.Read())
                            {
                                if (r["username"].ToString().ToLower() == user.ToLower())
                                {
                                    if (joinGreetFrequency == JoinGreetFrequency.Daily)
                                    {
                                        DateTime lastGreet = (DateTime)r["last_greeted"];
                                        if (lastGreet.AddDays(1) <= DateTime.Now.Date)
                                        {
                                            _logger.Log("Sending " + user + " a greeting", Logger.LogType.Debug);

                                            SendGreeting(user);

                                            r.Close();
                                            MySqlCommand cmd2;
                                            string date = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                                            String insert = String.Format("UPDATE users SET last_greeted=\"{0}\" WHERE channel=\"{1}\" AND username=\"{2}\";", date, _main.Channel.Substring(1), user.ToLower());

                                            using (cmd2 = new MySqlCommand(insert, formMain.mainForm.db.myDB))
                                            {
                                                cmd2.ExecuteNonQuery();
                                            }
                                            break;
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SendGreeting(String user)
        {
            _logger.Log("Sending " + user + " a greeting", Logger.LogType.Debug);
            _irc.SendMessage(_greeting.Replace("@user", user), Irc.QueuePriorty.High);
        }
    }
}
