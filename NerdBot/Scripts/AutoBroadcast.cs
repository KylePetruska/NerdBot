using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

namespace NerdBot
{
    public class AutoBroadcast
    {
        private MySqlCommand _cmd;

        private Thread _autoBroadcast;
        private static Timer _autoBroadcastTimer;
        private List<string> _messages = new List<string>();
        private int _messageNum = 0;
        private int _interval = 120000;
        private Irc _irc;
        private Logger _logger;

        public AutoBroadcast(Irc irc, int interval)
        {
            _logger = new Logger("AutoBroadcast");
            _irc = irc;
            _interval = (int)TimeSpan.FromMinutes(interval).TotalMilliseconds;

            LoadMessages();

            _autoBroadcast = new Thread(new ThreadStart(AutoBroadcastThread));
            _autoBroadcast.Start();
        }

        private void AutoBroadcastThread()
        {
            _autoBroadcastTimer = new Timer(HandleAutoBroadcast, null, _interval, _interval);
        }

        private void HandleAutoBroadcast(Object state)
        {
            if (formMain.mainForm.AutoBroadcastOn)
            {
                if (_irc.IsStreamOnline())
                {
                    if (_messageNum > _messages.Count - 1)
                        _messageNum = 0;

                    _irc.SendMessage(_messages[_messageNum], Irc.QueuePriorty.Normal);
                    _messageNum++;
                }   
            }
        }

        private void LoadMessages()
        {
            _messages = new List<string>();
            String sql = "SELECT * FROM autobroadcast WHERE channel='" + formMain.mainForm.Channel.Substring(1) + "'";
            using (_cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                using (MySqlDataReader r = _cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        _messages.Add(r["message"].ToString());
                    }
                }
            }
        }

        public void AddMessage(string message)
        {
            String sql = String.Format("INSERT INTO autobroadcast (channel, message) VALUES (\"{0}\", \"{1}\");", formMain.mainForm.Channel.Substring(1), message);
            Console.WriteLine(sql);
            using (_cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                _cmd.ExecuteNonQuery();
            }
            _messages.Add(message);
        }

        public void RemoveMessage(string message)
        {
            String sql = String.Format("DELETE FROM autobroadcast WHERE channel= \'{0}\' AND message=\'{1}\';", formMain.mainForm.Channel.Substring(1), message);
            Console.WriteLine(sql);
            using (_cmd = new MySqlCommand(sql, formMain.mainForm.db.myDB))
            {
                _cmd.ExecuteNonQuery();
            }
            _messages.Remove(message);
        }

        public void ChangeInterval(int interval)
        {
            _autoBroadcastTimer.Change(interval, interval);
        }
    }
}
