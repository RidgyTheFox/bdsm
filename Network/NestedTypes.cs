using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

namespace BDSM.Network.NestedTypes
{
    public struct PlayerState : INetSerializable
    {
        public uint pid { get; set; }
        public uint busId { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(pid);
            l_writer.Put(busId);
            l_writer.Put(position);
            l_writer.Put(rotation);
        }
        public void Deserialize(NetDataReader l_reader)
        {
            pid = l_reader.GetUInt();
            busId = l_reader.GetUInt();
            position = l_reader.GetVector3();
            rotation = l_reader.GetQuaternion();
        }

    }

    public struct ServerState : INetSerializable
    {
        public string serverName { get; set; }
        public Enums.AvailableMaps currentMap { get; set; }
        public uint playersLimit { get; set; }
        public uint currentAmountOfPlayers { get; set; }

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(serverName);
            l_writer.Put((uint)currentMap);
            l_writer.Put(playersLimit);
            l_writer.Put(currentAmountOfPlayers);

        }

        public void Deserialize(NetDataReader l_reader)
        {
            serverName = l_reader.GetString();
            currentMap = (Enums.AvailableMaps)l_reader.GetUInt();
            playersLimit = l_reader.GetUInt();
            currentAmountOfPlayers = l_reader.GetUInt();
        }
    }
}
