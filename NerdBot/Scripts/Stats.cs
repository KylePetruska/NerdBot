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

namespace NerdBot
{
    public class Stats
    {
        private int _followerOffset;
        private int _newFollowers = 0;

        private Thread _statsThread;
        private Timer _statsUpdateTimer;

        private formMain _main;
        private Logger _logger;

        public Stats()
        {
            _logger = new Logger("Stats");
            _main = formMain.mainForm;
            InitializeStats();
        }

        void InitializeStats()
        {
            GetOffset();
            _main.SetNewFollowers(_newFollowers);
            _statsThread = new Thread(new ThreadStart(StatThread));
            _statsThread.Start();
        }

        private void StatThread()
        {
            _statsUpdateTimer = new Timer(GetStatUpdate, null, 0, 70000);
        }

        private void GetOffset()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("https://api.twitch.tv/kraken/channels/" + _main.Channel.Substring(1) + "/follows");
                JObject parsedData = JObject.Parse(result);
                _followerOffset = (int)parsedData["_total"];
            }  
        }

        private void GetStatUpdate(Object state)
        {
            _logger.Log("Getting Stat Update", Logger.LogType.Debug);
            UpdateCurrentFollowers();
            UpdateNewFollowers();
        }

        private void UpdateCurrentFollowers()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("https://api.twitch.tv/kraken/channels/" + _main.Channel.Substring(1) + "/follows");
                JObject parsedData = JObject.Parse(result);
                string total = parsedData["_total"].ToString();

                if ((int)parsedData["_total"] < _followerOffset)
                    GetOffset();

                _main.SetCurrentFollowers(total);
            }
        }

        private void UpdateNewFollowers()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString("https://api.twitch.tv/kraken/channels/" + _main.Channel.Substring(1) + "/follows?direction=ASC&limit=100&offset=" + _followerOffset);
                JObject parsedData = JObject.Parse(result);
                string name = "<Error>";

                if (parsedData["follows"].ToString() != "[]")
                {
                    if (_main.FollowAlertOn)
                    {
                        foreach (JObject follow in parsedData["follows"])
                        {
                            name = follow["user"]["display_name"].ToString();
                            _logger.Log("Found new follower: " + name + ". Sending greeting.", Logger.LogType.Debug); 
                            _newFollowers++;
                            _followerOffset++;
                            _main.SetNewFollowers(_newFollowers);
                            SendFollowGreeting(name);
                        }
                    }
                }
            }
        }

        private void SendFollowGreeting(string name)
        {
            _main.chat.SendMessage("Thank you for following " + name + "! Enjoy the stream!", Irc.QueuePriorty.High);
        }

    }
}
