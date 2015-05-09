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
        private String _nick, _admin, _user, _oauth = "";
        private bool testMode = true;

        public TcpClient irc;

        private StreamReader _read;
        private StreamWriter _write;

        private List<Users> _users = new List<Users>();

        private ConcurrentQueue<string> _highPriority = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> _normalPriority = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> _lowPriority = new ConcurrentQueue<string>();

        private Thread _listener;
        private Thread _keepAlive;

        private Timer _messageQueue;

        private Commands _commands;
        private AutoBroadcast _autoBroadcast;
        private Logger _logger;
        private Greeting _greeting;

        private formMain _main;

        private string[] emoteDelim = 
        { ":)", ":(", ":P", ";)", "Kappa", ":o", ":z", "B)", ":\\", ";p", ":p", 
          "R)", "o_O", ":D", ">(", "<3", "KappaHD", "MiniK", "duDudu", "riPepperonis" };

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

            _commands = new Commands(_main.Channel.Substring(1));
            _greeting = new Greeting(this);

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
                //BuildUserList();
                SendMessage("Nerdbot9000 is now online! Hello fellow nerds!", QueuePriorty.Low);
            }
        }

        private void StartThreads()
        {
            _listener = new Thread(new ThreadStart(Listen));
            _listener.Start();

            _keepAlive = new Thread(new ThreadStart(KeepAlive));
            _keepAlive.Start();

            _messageQueue = new Timer(HandleMessageQueue, null, 0, 1000);

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
            String[] msg = message.Split(' ');

            if (msg[0].Equals("PING"))
            {
                _logger.Log("Got a PING", Logger.LogType.Debug);
                SendRaw("PONG " + msg[1]);
            }
            else if (msg[1].Equals("PRIVMSG"))
            {
                _user = CapName(GetUser(message));
                AddUser(_user);
                String temp = message.Substring(message.IndexOf(":", 1) + 1);
                _logger.Log(_user + " sent a message", Logger.LogType.Debug);
                HandleMessage(temp, _user);
            }
            else if (msg[1].Equals("JOIN"))
            {
                _user = CapName(GetUser(message));
                AddUser(_user);

                _logger.Log(_user + " joined", Logger.LogType.Debug);

                if (_user.ToLower() == Admin.ToLower())
                    return;
                if (_user.ToLower() == _nick.ToLower())
                    return;

                _greeting.HandleGreeting(_user);
            }
            else if (msg[1].Equals("PART"))
            {
                RemoveUser(GetUserFromList(CapName(GetUser(message))));
                _logger.Log(_user + " left", Logger.LogType.Debug);
            }
            else if (msg[1].Equals("MODE")) //:jtv MODE #tuumn +o tuumn
            {
                Users user = GetUserFromList(msg[4]);

                if (user == null)
                {
                    _logger.Log("User was null when getting mode change", Logger.LogType.Error);
                    return;
                }

                if (msg[3].Equals("+o"))
                {
                    if (user.UserLevel < 2)
                    {
                        _logger.Log("User " + user.Name + " was promoted to a moderator.", Logger.LogType.Debug);
                        user.SetUserLevel(2);
                    }
                }
                else if (msg[3].Equals("-o"))
                {
                    _logger.Log("User " + user.Name + " was demoted to regular user.", Logger.LogType.Debug);
                    user.SetUserLevel(0);
                }
            }
        }

        private void HandleMessage(String message, String user)
        {
            string space = " ";
            char[] delim = space.ToCharArray();
            String[] msg = message.Split(delim, 2);

            Users testUser = GetUserFromList(user);

            if (testUser == null)
            {
                _logger.Log("User was null when handling message", Logger.LogType.Error);
                return;
            }

            if (testUser.UserLevel == 0)
            {
                if (testUser.LastMessageTime.AddSeconds(10) >= DateTime.Now)
                {
                    if (message == testUser.LastMessage)
                    {
                        testUser.Warn(Users.WarnType.Spam);
                        return;
                    }
                }
            }

            testUser.LastMessage = message;
            testUser.LastMessageTime = DateTime.Now;

            if (_main.BanLinks)
            {
                if (testUser.UserLevel == 0)
                {
                    if (ContainsLink(message))
                    {
                        testUser.Warn(Users.WarnType.Link);
                        return;
                    }
                }
            }

            if (_main.EmoteLimit)
            {
                if (testUser.UserLevel == 0)
                {
                    String[] emote = message.Split(emoteDelim, StringSplitOptions.None);

                    if (emote.Length >= 5)//TODO: Make this configurable
                    {
                        testUser.Warn(Users.WarnType.EmoteSpam);
                        return;
                    }
                }
            }

            if (msg[0].Equals("!commands") )
            {

                if (msg.Length == 2)
                {
                    String[] cmd = msg[1].Split(delim, 5);

                    if (testUser.UserLevel >= 2)
                    {
                        //New add command format: !commands add [!command] [level] [output]
                        if (cmd[0].Equals("add"))
                        {
                            SendMessage("Adding command: " + cmd[1] + " Level: " + cmd[2] + " Output: " + cmd[3], QueuePriorty.Low);
                            _commands.AddCommand(cmd[1], Convert.ToInt16(cmd[2]), cmd[2]);
                        }
                        else if (cmd[0].Equals("remove"))
                        {
                            if (_commands.CmdExists(cmd[0]))
                            {
                                SendMessage("Removing command: " + cmd[1], QueuePriorty.Low);
                                _commands.RemoveCommand(cmd[1]);
                            }
                            else
                            {
                                SendMessage("Command not found. Could not removed.", QueuePriorty.Low);
                            }
                        }
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

                if (testUser.UserLevel >= 2)
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

        public void AddUser(string name)
        {
            if (name == "jtv")
                return;

            lock (_users)
            {
                if (GetUserFromList(name) == null)
                {
                    _users.Add(new Users(name));
                }
            }
        }

        private void RemoveUser(Users name)
        {
            lock (_users)
            {
                if (_users.Contains(name))
                {
                    _users.Remove(name);
                }
            }
        }

        private void BuildUserList()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("http://tmi.twitch.tv/group/user/" + formMain.mainForm.Channel.Substring(1) + "/chatters");
                JObject parsedData = JObject.Parse(result);
                Console.WriteLine(result);
                foreach (JProperty chatters in parsedData["chatters"])
                {
                    foreach (JArray chatterType in chatters)
                    {
                        foreach (string chatter in chatterType)
                        {
                            AddUser(chatter);
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
            if (testMode)
                return true;

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

        private Users GetUserFromList(string name)
        {
            foreach (Users user in _users)
            {
                if (user.Name.ToLower() == name.ToLower())
                {
                    return user;
                }
            }
            return null;
        }
    }
}
