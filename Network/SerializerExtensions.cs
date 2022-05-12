using UnityEngine;
using LiteNetLib.Utils;

/// <summary>
/// This namespace contains extensions for LiteNetLib serializer. So, i can serialize Unity`s data types in serialized classes.
/// </summary>
namespace BDSM.Network
{
    public static class SerializerExtension_Quaternion
    {
        public static void Put(this NetDataWriter writer, Quaternion quaternion)
        {
            writer.Put(quaternion.x);
            writer.Put(quaternion.y);
            writer.Put(quaternion.z);
            writer.Put(quaternion.w);
        }

        public static Quaternion GetQuaternion(this NetDataReader reader)
        {
            return new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }
    }

    public static class SerializerExtension_Vector2
    {
        public static void Put(this NetDataWriter writer, Vector2 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2(reader.GetFloat(), reader.GetFloat());
        }
    }

    public static class SerializerExtension_Vector2Int
    {
        public static void Put(this NetDataWriter writer, Vector2Int vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2Int GetVector2Int(this NetDataReader reader)
        {
            return new Vector2Int(reader.GetInt(), reader.GetInt());
        }
    }

    public static class SerializerExtension_Vector3
    {
        public static void Put(this NetDataWriter writer, Vector3 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
            writer.Put(vector.z);
        }

        public static Vector3 GetVector3(this NetDataReader reader)
        {
            return new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }
    }

    public static class SerializerExtension_Vector3Int
    {
        public static void Put(this NetDataWriter writer, Vector3Int vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
            writer.Put(vector.z);
        }

        public static Vector3Int GetVector3Int(this NetDataReader reader)
        {
            return new Vector3Int(reader.GetInt(), reader.GetInt(), reader.GetInt());
        }
    }
}
