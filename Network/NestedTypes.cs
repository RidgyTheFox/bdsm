using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

namespace BDSM.Network.NestedTypes
{
    public struct PlayerState : INetSerializable
    {
        public uint pid { get; set; }
        public string selectedBusShortName { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(pid);
            l_writer.Put(selectedBusShortName);
            l_writer.Put(position);
            l_writer.Put(rotation);
        }
        public void Deserialize(NetDataReader l_reader)
        {
            pid = l_reader.GetUInt();
            selectedBusShortName = l_reader.GetString();
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

    public struct NetDateAndTime : INetSerializable
    {
        public uint day { get; set; }
        public uint hours { get; set; }
        public uint minutes { get; set; }
        public uint seconds { get; set; }

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(day);
            l_writer.Put(hours);
            l_writer.Put(minutes);
            l_writer.Put(seconds);

        }
        public void Deserialize(NetDataReader l_reader)
        {
            day = l_reader.GetUInt();
            hours = l_reader.GetUInt();
            minutes = l_reader.GetUInt();
            seconds = l_reader.GetUInt();
        }
    }

    public struct BusState : INetSerializable
    {
        public bool isEngineTurnedOn { get; set; }
        public bool isHighBeamTurnedOn { get; set; }
        public bool isLeftBlinkerBlinking { get; set; }
        public bool isRightBlinkerBlinking { get; set; }
        public bool isBothBlinkersBlinking { get; set; }
        public bool isReverseGear { get; set; }
        public bool isBraking { get; set; }
        public bool isDriverLightsTurnedOn { get; set; }
        public bool isInsideLightsTurnedOn { get; set; }
        public bool isFrontDoorOpened { get; set; }
        public bool isMiddleDoorOpened { get; set; }
        public bool isRearDoorOpened { get; set; }

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(isEngineTurnedOn);
            l_writer.Put(isHighBeamTurnedOn);
            l_writer.Put(isLeftBlinkerBlinking);
            l_writer.Put(isRightBlinkerBlinking);
            l_writer.Put(isBothBlinkersBlinking);
            l_writer.Put(isReverseGear);
            l_writer.Put(isBraking);
            l_writer.Put(isDriverLightsTurnedOn);
            l_writer.Put(isInsideLightsTurnedOn);
            l_writer.Put(isFrontDoorOpened);
            l_writer.Put(isMiddleDoorOpened);
            l_writer.Put(isRearDoorOpened);
        }

        public void Deserialize(NetDataReader l_reader)
        {
            isEngineTurnedOn = l_reader.GetBool();
            isHighBeamTurnedOn = l_reader.GetBool();
            isLeftBlinkerBlinking = l_reader.GetBool();
            isRightBlinkerBlinking = l_reader.GetBool();
            isBothBlinkersBlinking = l_reader.GetBool();
            isReverseGear = l_reader.GetBool();
            isBraking = l_reader.GetBool();
            isDriverLightsTurnedOn = l_reader.GetBool();
            isInsideLightsTurnedOn = l_reader.GetBool();
            isFrontDoorOpened = l_reader.GetBool();
            isMiddleDoorOpened = l_reader.GetBool();
            isRearDoorOpened = l_reader.GetBool();
        }
    }
}
