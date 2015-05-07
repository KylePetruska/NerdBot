using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace NerdBot
{
    public class Irc
    {
        private String _nick, _admin, _user, _greeting, _oauth = "";

        public TcpClient irc;

        private StreamReader _read;
        private StreamWriter _write;

        private List<string> _users = new List<string>();
        private List<string> _modList = new List<string>();

        private ConcurrentQueue<string> _highPriority = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> _normalPriority = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> _lowPriority = new ConcurrentQueue<string>();

        private Thread _listener;
        private Thread _keepAlive;

        private Timer _messageQueue;

        private Commands _commands;
        private AutoBroadcast _autoBroadcast;
        private Logger _logger;

        private formMain _main;

        private JoinGreetFrequency joinGreetFrequency { get; set; }

        public enum JoinGreetFrequency
        {
            Everytime,
            Daily,
            Once
        }

        public enum QueuePriorty
        {
            Low,
            Normal,
            High
        }

        public Irc()
        {
            _main = formMain.mainForm;
            _logger = new Logger("Irc");

            Admin = _main.Channel;

            _nick = Properties.Settings.Default.name.ToLower();
            _oauth = Properties.Settings.Default.oauthKey;
            _greeting = Properties.Settings.Default.joinGreeting;
            joinGreetFrequency = (JoinGreetFrequency)Properties.Settings.Default.joinGreetFrequency;

            _commands = new Commands(_main.Channel);

            Connect();
        }

        private String Admin
        {
            get { return _admin; }

            set
            {
                if (value.StartsWith("#"))
                {
                    _admin = value.Substring(1).ToLower();
                }
                else
                {
                    _admin = value.ToLower();
                }
            }
        }

        private void Connect()
        {
            _logger.Log("Connecting to IRC", Logger.LogType.Debug);

            if (irc != null)
                irc.Close();

            irc = new TcpClient();

            while (!irc.Connected)
            {
                try
                {
                    irc.Connect("irc.twitch.tv", 6667);

                    _read = new StreamReader(irc.GetStream());
                    _write = new StreamWriter(irc.GetStream());

                    _write.AutoFlush = true;

                    SendRaw("PASS " + _oauth);
                    SendRaw("NICK " + _nick);
                    //sendRaw("USER " + nick); - Not needed
                    SendRaw("JOIN " + _main.Channel);
                }
                catch (SocketException e)
                {
                    _logger.Log("Unable to connect to IRC:", Logger.LogType.Error);
                    _logger.Log(e.ToString(), Logger.LogType.Error);
                    return;
                }
                catch (Exception e)
                {
                    _logger.Log("Unable to connect to IRC:", Logger.LogType.Error);
                    _logger.Log(e.ToString(), Logger.LogType.Error);
                    return;
                }
            }

            if (irc.Connected)
            {
                _logger.Log("IRC is connected. Starting Threads and other what-nots", Logger.LogType.Debug);
                StartThreads();
                BuildUserList();
                GetMods();
                SendMessage("Nerdbot9000 is now online! Hello fellow nerds!", QueuePriorty.Low);
            }
        }

        private void StartThreads()
        {
            _listener = new Thread(new ThreadStart(Listen));
            _listener.Start();

            _keepAlive = new Thread(new ThreadStart(KeepAlive));
            _keepAlive.Start();

            _messageQueue = new Timer(HandleMessageQueue, null, 0, 3000);

            _autoBroadcast = new AutoBroadcast(this, Properties.Settings.Default.autoBroadcastInterval);

            _logger.Log("Threads started", Logger.LogType.Debug);
        }

        private void Listen()
        {
            try
            {
                while (irc.Connected)
                {
                    ParseMessage(_read.ReadLine());
                }
            }
            catch (IOException e)
            {
                _logger.Log("Unable to listen:", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
            }
            catch (Exception e)
            {
                _logger.Log("Unable to listen:", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);
                Listen();
            }
        }

        private void KeepAlive()
        {
            while (true)
            {
                Thread.Sleep(30000);
                SendRaw("PING 1245");
                _logger.Log("Pinging Twitch IRC", Logger.LogType.Debug);
            }
        }

        private void ParseMessage(String message)
        {
            _logger.Log("Parsing Message", Logger.LogType.Debug);
           
            String[] msg = message.Split(' ');

            if (msg[0].Equals("PING"))
            {
                _logger.Log("Got a PING", Logger.LogType.Debug);
                SendRaw("PONG " + msg[1]);
            }
            else if (msg[1].Equals("PRIVMSG"))
            {
                _user = CapName(GetUser(message));
                AddUserToList(_user);
                String temp = message.Substring(message.IndexOf(":", 1) + 1);
                _logger.Log(_user + " sent a message", Logger.LogType.Debug);
                HandleMessage(temp, _user);
            }
            else if (msg[1].Equals("JOIN"))
            {
                _user = CapName(GetUser(message));
                AddUserToList(_user);

                _logger.Log(_user + " joined", Logger.LogType.Debug);

                if (_user.ToLower() == Admin.ToLower())
                    return;
                if (_user.ToLower() == _nick.ToLower())
                    return;

                if (_user.ToLower() != Admin.ToLower() || _user.ToLower() != _nick.ToLower())
                {
                    SendGreeting(_user);
                }
            }
            else if (msg[1].Equals("PART"))
            {
                RemoveUserFromList(CapName(GetUser(message)));
                _logger.Log(_user + " left", Logger.LogType.Debug);
            }
        }

        private void SendGreeting(String user)
        {

            if (joinGreetFrequency == JoinGreetFrequency.Everytime)
            {
                _logger.Log("Sending " + user + " a greeting", Logger.LogType.Debug);
                SendMessage(_greeting.Replace("@user", user), QueuePriorty.High);
                return;
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
                            SendMessage(_greeting.Replace("@user", user), QueuePriorty.High);
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
                                if (r["username"].ToString() == user.ToLower())
                                {
                                    DateTime lastGreet = (DateTime)r["last_greeted"];
                                    if (lastGreet.AddDays(1) <= DateTime.Now.Date)
                                    {
                                        _logger.Log("Sending " + user + " a greeting", Logger.LogType.Debug);

                                        SendMessage(_greeting.Replace("@user", user), QueuePriorty.High);

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

        private void HandleMessage(String message, String user)
        {
            string space = " ";
            char[] delim = space.ToCharArray();
            String[] msg = message.Split(delim, 2);

            if (_main.BanLinks)
            {
                if (ContainsLink(message))
                {
                    if (!IsMod(user))
                    {
                        SendMessage("Hey " + user + ", no links! [warning]", QueuePriorty.Normal);
                        SendMessage("/timeout " + user + " 30", QueuePriorty.High);
                    }
                }
            }

            if (_main.EmoteLimit)
            {
                string[] emoteDelim = {":)", ":(", ":P", ";)", "Kappa"};

                String[] emote = message.Split(emoteDelim, StringSplitOptions.None);

                if (emote.Length >= 5)
                {
                    SendMessage("Hey " + user + ", chill out on the emote spam! [warning]", QueuePriorty.Normal);
                    SendMessage("/timeout " + user + " 30", QueuePriorty.High);
                }
            }

            if (msg[0].Equals("!commands") )
            {

                if (msg.Length == 2)
                {
                    String[] cmd = msg[1].Split(delim, 3);

                    if (IsMod(user))
                    {
                        if (cmd[0].Equals("add"))
                        {
                            SendMessage("Adding command: " + cmd[1] + " Output: " + cmd[2], QueuePriorty.Low);
                            _commands.AddCommand(cmd[0], 1, cmd[2]);
                        }
                        else if (cmd[0].Equals("remove"))
                        {
                            if (_commands.CmdExists(cmd[0]))
                            {
                                SendMessage("Removing command: " + cmd[1], QueuePriorty.Low);
                                _commands.RemoveCommand(cmd[0]);
                            }
                            else
                            {
                                SendMessage("Command not found. Could not removed.", QueuePriorty.Low);
                            }
                        }
                    }
                    else
                    {
                        SendMessage("You do not have permission to do that", QueuePriorty.Low);
                    }
                }
                else
                {
                    SendMessage(_commands.GetList(), QueuePriorty.Normal);
                }

            }
            else if (msg[0] == "!autobroadcast")
            {
                String[] cmd = msg[1].Split(delim, 2);

                if (IsMod(user))
                {
                    if (cmd[0].Equals("add"))
                    {
                        SendMessage("Adding new autobroadcast message: " + cmd[1], QueuePriorty.Low);
                        _autoBroadcast.AddMessage(cmd[1]);
                    }
                    else if (cmd[0].Equals("remove"))
                    {
                        SendMessage("Removing autobroadcast message: " + cmd[1], QueuePriorty.Low);
                        _autoBroadcast.RemoveMessage(cmd[1]);
                    }
                }
            }
            else if (msg[0] == "!uptime")
            {
                if (IsStreamOnline())
                {
                    TimeSpan uptime = GetUptime();
                    SendMessage("Streaming for " + uptime.Hours + " hour(s), " + uptime.Minutes + " minute(s), and " + uptime.Seconds + " second(s).", QueuePriorty.Low);
                }
            }
            else
            {
                //Handle Custom Commands
                for (int i = 0; i < _commands.custCmds.GetLength(0); i++)
                {
                    if (_commands.custCmds[i, 0] != "")
                    {
                        if (msg[0] == _commands.custCmds[i, 0])
                        {
                            SendMessage(_commands.custCmds[i, 1].ToString(), QueuePriorty.Normal);
                            break;
                        }
                    }
                }
            }
        }

        private bool ContainsLink(String message)
        {
            string urlPattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]";
            Regex reg = new Regex(urlPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (reg.IsMatch(message))
            {
                return true;
            }
            return false;
        }

        private void AddUserToList(String nick)
        {
            lock (_users)
            {
                if (!_users.Contains(nick))
                {
                    _users.Add(nick);
                }
            }
        }

        private void RemoveUserFromList(String nick)
        {
            lock (_users)
            {
                if (_users.Contains(nick))
                {
                    _users.Remove(nick);
                }
            }
        }

        private void BuildUserList()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("http://tmi.twitch.tv/group/user/tuumn/chatters");
                JObject parsedData = JObject.Parse(result);
                foreach (JProperty chatters in parsedData["chatters"])
                {
                    foreach (JArray chatterType in chatters)
                    {
                        foreach (string chatter in chatterType)
                        {
                            _users.Add(chatter);
                        }
                    }
                }
            }
        }

        private String CapName(String user)
        {
            return char.ToUpper(user[0]) + user.Substring(1);
        }

        private String GetUser(String message)
        {
            String[] temp = message.Split('!');
            _user = temp[0].Substring(1);
            return CapName(_user);
        }

        private void SendRaw(String message)
        {

            int attempt = 0;
            try
            {  
              _write.WriteLine(message);
              attempt = 0;
            }
            catch (Exception e)
            {
                _logger.Log("Could not SendRaw to IRC:", Logger.LogType.Error);
                _logger.Log(e.ToString(), Logger.LogType.Error);

                attempt++;
                if (attempt >= 5)
                {
                    irc.Close();
                    Connect();
                    attempt = 0;
                }
            }

        }

        public void SendMessage(String message, QueuePriorty priority)
        {
            if (!IsStreamOnline())
                return;

            if (priority == QueuePriorty.High)
            {
                _highPriority.Enqueue(message);
            }
            else if (priority == QueuePriorty.Normal)
            {
                _normalPriority.Enqueue(message);
            }
            else if (priority == QueuePriorty.Low)
            {
                _lowPriority.Enqueue(message);
            }
            else
            {
                _lowPriority.Enqueue(message);
            }
        }

        public bool IsStreamOnline()
        {
            if (irc.Connected)
            {
                using (var client = new WebClient())
                {
                    String json_data = "";
                    try
                    {
                        client.Proxy = null;
                        json_data = client.DownloadString("https://api.twitch.tv/kraken/streams/" + _main.Channel.Substring(1));
                        JObject stream = JObject.Parse(json_data);
                        if (stream["stream"].HasValues)
                        {
                            return true;
                        }
                    }
                    catch (SocketException e)
                    {
                        _logger.Log("Unable to connect to twitch API to check stream status.", Logger.LogType.Error);
                        _logger.Log(e.ToString(), Logger.LogType.Error);
                    }
                    catch (Exception e)
                    {
                        _logger.Log("Unable to connect to twitch API to check stream status.", Logger.LogType.Error);
                        _logger.Log(e.ToString(), Logger.LogType.Error);
                    }
                }

                return false;
            }
            return false;
        }

        private void HandleMessageQueue(Object state)
        {
            string rawmsg = ":" + _nick + "!" + _nick + "@" + _nick + ".tmi.twitch.tv " + "PRIVMSG " + _main.Channel + " :";

            if (IsStreamOnline())
            {
                String message;

                if (_highPriority.TryDequeue(out message))
                {
                    rawmsg += message;
                    SendRaw(rawmsg);
                }
                else if (_normalPriority.TryDequeue(out message))
                {
                    rawmsg += message;
                    SendRaw(rawmsg);
                }
                else if (_lowPriority.TryDequeue(out message))
                {
                    rawmsg += message;
                    SendRaw(rawmsg);
                }

                _messageQueue.Change(500, Timeout.Infinite);
            }
            else
            {
                _logger.Log("Stream is offline", Logger.LogType.Error);
                //Stream is offline, checking again in twenty seconds
                _messageQueue.Change(20000, Timeout.Infinite);
            }
        }

        public void GetMods()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("http://tmi.twitch.tv/group/user/" + _main.Channel.Substring(1) + "/chatters");
                JObject parsedData = JObject.Parse(result);
                foreach (string mod in parsedData["chatters"]["moderators"])
                {
                    _modList.Add(mod);
                }
            }
        }

        public bool IsMod(string user)
        {
            foreach (string mod in _modList)
            {
                if (user.ToUpper() == mod.ToUpper())
                    return true;
            }
            return false;
        }

        private TimeSpan GetUptime()
        {
            if (irc.Connected)
            {
                using (var client = new WebClient())
                {
                    String json_data = "";
                    try
                    {
                        client.Proxy = null;
                        json_data = client.DownloadString("https://api.twitch.tv/kraken/streams/" + _main.Channel.Substring(1));
                        JObject stream = JObject.Parse(json_data);
                        if (stream["stream"].HasValues)
                        {
                            DateTime startTime = (DateTime)stream["stream"]["created_at"];

                            TimeSpan uptime = DateTime.Now - startTime;
                            return uptime;
                        }
                        client.Dispose();
                    }
                    catch (SocketException e)
                    {
                        _logger.Log("Unable to connect to twitch API.", Logger.LogType.Error);
                        _logger.Log(e.ToString(), Logger.LogType.Error);
                    }
                    catch (Exception e)
                    {
                        _logger.Log("Unable to connect to twitch API.", Logger.LogType.Error);
                        _logger.Log(e.ToString(), Logger.LogType.Error);
                    }
                }
            }
            return TimeSpan.Zero;
        }

        public void ChangeAutoBroadcastInterval(int interval)
        {
            _autoBroadcast.ChangeInterval(interval);
        }
    }
}
