using GameServerShenkar.Globals;
using GameServerShenkar.Managers;
using GameServerShenkar.Models;
using GameServerShenkar.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerShenkar.Requests
{
    public class SendMoveRequest
    {
        public static Dictionary<string, object> Get(User CurUser, Dictionary<string, object> Details)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (Details.ContainsKey("Index"))
            {
                GameThread room = RoomsManager.Instance.GetRoom(CurUser.MatchId);
                if(room != null && room.IsRoomActive)
                {
                    response = room.ReceivedMove(CurUser, Details["Index"].ToString());
                    response.Add("IsSucces", true);
                }
                else response.Add("ErrorCode", GlobalEnums.ErrorCodes.RoomClosed);
            }
            else response.Add("ErrorCode", GlobalEnums.ErrorCodes.MissingVariables);

            if (response.ContainsKey("IsSucces") == false)
                response.Add("IsSucces", false);

            return response;
        }
    }
}
