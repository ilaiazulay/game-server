using Armetis.Utils;
using GameServerShenkar.Globals;
using GameServerShenkar.Managers;
using GameServerShenkar.Models;
using GameServerShenkar.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServerShenkar.Threads
{
    public class GameThread
    {
        #region Variables

        private string matchId;
        private int turnTime = GlobalVariables.TurnTime;
        private int _timeOutTime = GlobalVariables.TimeOutTime;
        private int moveCounter;
        private bool isRoomActive;
        public bool IsRoomActive { get => isRoomActive; }
        private bool isDetroyThread;
        
        private int turnIndex;

        private List<string> playersOrder;
        private Dictionary<string, User> users;
        private Thread currentThread;

        private RoomTime roomTime;

        #endregion

        #region Constructor 
        public GameThread(string MatchId, MatchData CurMatchData)
        {
            matchId = MatchId;
            isRoomActive = false;
            moveCounter = 0;
            turnIndex = 0;
            roomTime = new RoomTime(turnTime, _timeOutTime);
            //TODO: Init Destroy Time

            playersOrder = new List<string>();
            users = new Dictionary<string, User>();
            foreach(string userId in CurMatchData.PlayersData.Keys)
            {
                User curUser = SessionsManager.Instance.GetUser(userId);
                if (curUser != null)
                {
                    curUser.CurUserState = User.UserState.Playing;
                    curUser.MatchId = matchId;
                    SessionsManager.Instance.UpdateUser(curUser);

                    users.Add(userId,curUser);
                    playersOrder.Add(userId);
                }
            }
        }

        #endregion

        #region Thread
        public void GameLoop()
        {
            while (IsRoomActive)
            {
                try
                {
                    if (roomTime.IsCurrentTimeActive() == false)
                        ChangeTurn();

                    if (isDetroyThread && roomTime.IsRoomTimeOut() == false)
                    {
                        Console.WriteLine("Killed Room");
                        CloseRoom();
                    }

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    Console.WriteLine("MatchThread " + e.Message);
                    CloseRoom();
                }
            }
        }

        #endregion

        #region Requests
        public void StartGame()
        {
            turnIndex = UtilFunctions.GetRandomNumber(0,1);

            Dictionary<string, object> sendData = new Dictionary<string, object>()
            {
                {"Service","StartGame"},
                {"MI",matchId},
                {"TT",UtilFunctions.GetUtcTime()},
                {"MTT",turnTime},
                {"CP",playersOrder[turnIndex]},
                {"Players",playersOrder},
                {"MC",moveCounter}
            };

            string toSend = JsonLogic.Serialize(sendData);
            BroadcastToRoom(toSend);

            isRoomActive = true;
            isDetroyThread = true;
            currentThread = new Thread(GameLoop);
            currentThread.Start();
            roomTime.ResetTimer();
        }

        public Dictionary<string,object> ReceivedMove(User curUser,string boardIndex)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            if (playersOrder[turnIndex] == curUser.UserId)
            {
                PassTurn();
                response = new Dictionary<string, object>()
                {
                    {"Service","BroadcastMove"},
                    {"SenderId",curUser.UserId},
                    {"Index",boardIndex},
                    {"CP",playersOrder[turnIndex]},
                    {"MC",moveCounter},
                };

                string toSend = JsonLogic.Serialize(response);
                BroadcastToRoom(toSend);

                //TODO: Save Moves to DataBase
            }
            else response.Add("ErrorCode", GlobalEnums.ErrorCodes.NotPlayerTurn);

            return response;
        }

        //TODO: Stop Game

        #endregion

        #region Global Functions

        private void PassTurn()
        {
            moveCounter++;
            turnIndex = turnIndex == 0 ? 1 : 0;
            roomTime.ResetTimer();
        }

        private void BroadcastToRoom(string ToSend)
        {
            foreach (string userId in users.Keys)
                users[userId].SendMessage(ToSend);
        }

        private void ChangeTurn()
        {
            PassTurn();
            Dictionary<string, object> notifyData = new Dictionary<string, object>()
            {
                {"Service","PassTurn"},
                {"CP",playersOrder[turnIndex]},
                {"MC",moveCounter}
            };

            string toSend = JsonLogic.Serialize(notifyData);
            BroadcastToRoom(toSend);
        }

        private void CloseRoom()
        {
            Console.WriteLine("Closed Room " + DateTime.UtcNow.ToShortTimeString());
            isRoomActive = false;
            RoomsManager.Instance.RemoveRoom(matchId);
        }

        public Dictionary<string, object> SendChat(User curUser, string message)
        {
            Dictionary<string, object> broadcastData = new Dictionary<string, object>()
            {
                {"Service","SendChat"},
                {"Sender",curUser.UserId},
                {"MatchId",curUser.MatchId},
                {"Message",message}
            };

            string toSend = JsonLogic.Serialize(broadcastData);
            BroadcastToRoom(toSend);

            return new Dictionary<string, object>();
        }


        #endregion
    }
}
