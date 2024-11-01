using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game
{
    public class GameLogic : JobSerializer
    {
        public static GameLogic Instance { get; } = new GameLogic();

        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();


        int _roomId = 1;

        public void Update()
        {
            Flush();

            foreach (GameRoom room in _rooms.Values)
            {
                room.Update();
            }
        }

        public GameRoom Add(int mapId)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Push(gameRoom.Init, mapId, 10);

            gameRoom.RoomId = _roomId;
            _rooms.Add(_roomId, gameRoom);
            _roomId++;

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            return _rooms.Remove(roomId);
        }

        public bool MoveRoom(Player player, int afterMapId)
        {
            if (!_rooms.TryGetValue(player.Room.RoomId, out var room))
            {
                return false;
            }

            room.LeaveGame(player.Id);

            var afterRoom = _rooms.FirstOrDefault(x => x.Value.Map.MapId == afterMapId);

            if (afterRoom.Value == null)
            {
                room.EnterGame(player);
                return false;
            }

            afterRoom.Value.EnterGame(player);
            return true;
        }

        public GameRoom FindByMapId(int roomId)
        {
            return _rooms.FirstOrDefault(x => x.Value.Map.MapId == roomId).Value;
        }

        public GameRoom FindByRoomId(int roomId)
        {
            GameRoom room = null;
            if (_rooms.TryGetValue(roomId, out room))
                return room;

            return null;
        }
    }
}