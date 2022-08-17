using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

/// <summary>
/// This namespace contain some classes that you can use in other classes for serialization. Its something like a data type in packets.
/// </summary>
namespace BDSM.Network.NestedTypes
{
    /// <summary>
    /// This type contains all client-side information about player.
    /// </summary>
    public struct PlayerState : INetSerializable
    {
        public uint pid { get; set; }                       // PID or Player identifier. Its a unicum number that represents player. PID is equal to the socket identifier on the server to which this player is connected. That is, two players can have the same ID, but only if they are not on the server at the same time.
        public string selectedBusShortName { get; set; }    // Current player bus.
        public Vector3 position { get; set; }               // Current player position. If player in garage, position will be -999.0f -999.0f -999.0f
        public Quaternion rotation { get; set; }            // Current player rotation.

        // Small notes about wheels sync: i know, that kind of a sync sucks at all... This shit should be refactored.
        public Quaternion wheelFL { get; set; }             // Front-left wheel rotation.
        public Quaternion wheelFR { get; set; }             // Front-right wheel rotation.
        public Quaternion wheelRL { get; set; }             // Rear-left wheel rotation.
        public Quaternion wheelRR { get; set; }             // Rear-right wheel rotation.

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(pid);
            l_writer.Put(selectedBusShortName);
            l_writer.Put(position);
            l_writer.Put(rotation);
            l_writer.Put(wheelFL);
            l_writer.Put(wheelFR);
            l_writer.Put(wheelRL);
            l_writer.Put(wheelRR);
        }
        public void Deserialize(NetDataReader l_reader)
        {
            pid = l_reader.GetUInt();
            selectedBusShortName = l_reader.GetString();
            position = l_reader.GetVector3();
            rotation = l_reader.GetQuaternion();
            wheelFL = l_reader.GetQuaternion();
            wheelFR = l_reader.GetQuaternion();
            wheelRL = l_reader.GetQuaternion();
            wheelRR = l_reader.GetQuaternion();
        }

    }

    /// <summary>
    /// This class represents current server state and providing some useful information on client side.
    /// </summary>
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

    /// <summary>
    /// This nested type for time syncing betwen server and client.
    /// </summary>
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

    /// <summary>
    /// This class represents current bus state of specified player.
    /// </summary>
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

    /// <summary>
    /// This class represents current state of one of the front wheels of bus. But this is not in use yet. Just for future...
    /// </summary>
    public struct WheelState : INetSerializable
    {
        public float wheelAngle { get; set; }       //
                                                    // There is data only for one wheel, but we can just mirror this data on other fron wheel, and mirror rotation on rear wheels.
        public float wheelRotation { get; set; }    //

        public void Serialize(NetDataWriter l_writer)
        {
            l_writer.Put(wheelAngle);
            l_writer.Put(wheelRotation);
        }
        public void Deserialize(NetDataReader l_reader)
        {
            wheelAngle = l_reader.GetFloat();
            wheelRotation = l_reader.GetFloat();
        }
    }
}
