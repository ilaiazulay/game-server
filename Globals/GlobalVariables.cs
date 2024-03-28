using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerShenkar.Globals
{
    internal class GlobalVariables
    {
        private static string _serverUrl = "ws://localhost:7890"; 
        public static string ServerUrl { get { return _serverUrl; } }

        private static int _turnTime = 10;
        public static int TurnTime { get { return _turnTime; } }

        private static int _timeOutTime = 360;
        public static int TimeOutTime { get { return _timeOutTime; } }

    }
}
