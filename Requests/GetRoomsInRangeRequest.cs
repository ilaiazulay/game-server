using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using GameServerShenkar.Globals;
using GameServerShenkar.Managers;
using GameServerShenkar.Models;
using GameServerShenkar.Services;
using GameServerShenkar.Threads;

namespace GameServerShenkar.Requests
{
    internal class GetRoomsInRangeRequest
    {
        public static Dictionary<string, object> Get(User CurUser, Dictionary<string, object> Details)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            Dictionary<string, GameThread> activeRooms = RoomsManager.Instance.GetAllRooms();
            if (activeRooms != null && activeRooms.Count > 0)
            {
                List<object> roomsList = new List<object>();
                foreach (var roomEntry in activeRooms)
                {
                    GameThread room = roomEntry.Value;
                    Dictionary<string, object> roomDetails = new Dictionary<string, object>();
                    // Add room details to roomDetails dictionary
                    roomsList.Add(roomDetails);
                }
                response.Add("Rooms", roomsList);
            }
            else
            {
                response.Add("ErrorCode", GlobalEnums.ErrorCodes.RoomClosed);
            }

            return response;
        }
    }
}
