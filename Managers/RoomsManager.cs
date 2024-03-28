using GameServerShenkar.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerShenkar.Managers
{
    public class RoomsManager
    {
        private Dictionary<string, GameThread> activeRooms;
        public Dictionary<string, GameThread> ActiveRooms { get => activeRooms; }


        #region Singleton
        private static RoomsManager instance;

        internal static RoomsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new RoomsManager();
                return instance;
            }
        }

        #endregion

        public RoomsManager()
        {
            activeRooms = new Dictionary<string, GameThread>();
        }

        public void AddRoom(string MatchId,GameThread Room)
        {
            if (ActiveRooms == null)
                activeRooms = new Dictionary<string, GameThread>();

            if (ActiveRooms.ContainsKey(MatchId))
                ActiveRooms[MatchId] = Room;
            else ActiveRooms.Add(MatchId,Room);
        }

        public void RemoveRoom(string MatchId)
        {
            if (ActiveRooms != null && ActiveRooms.ContainsKey(MatchId))
                ActiveRooms.Remove(MatchId);
        }

        public GameThread GetRoom(string MatchId)
        {
            if (ActiveRooms != null && ActiveRooms.ContainsKey(MatchId))
                return ActiveRooms[MatchId];
            return null;
        }

        public Dictionary<string, GameThread> GetAllRooms()
        {
            return ActiveRooms;
        }

        public bool IsRoomExist(string MatchId)
        {
            if (GetRoom(MatchId) != null)
                return true;
            else return false;
        }
    }
}
