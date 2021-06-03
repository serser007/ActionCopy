using System;
using System.Collections.Generic;
using Lidgren.Network;
using MessagePack;

namespace VoidNetworking
{
    public class Routing
    {
        public delegate void PacketReceivedHandler<in T>(T data, NetConnection connection);
        public delegate void PacketReceivedRequestHandler<in T>(T data, long requestId, NetConnection connection);

        private readonly Dictionary<long, PacketReceivedRequestHandler<byte[]>> routes = new Dictionary<long, PacketReceivedRequestHandler<byte[]>>();

        public void RegisterRoute<T>(long routeId, PacketReceivedHandler<T> callback)
        {
            RegisterRoute<T>(routeId, (arg1, arg2, arg3) => callback?.Invoke(arg1, arg3));
        }

        public void RegisterRoute<T>(long routeId, PacketReceivedRequestHandler<T> callback)
        {
            routes[routeId] = (data, packetId, connection) =>
            {
                if ((object) data is T byteArray)
                    callback?.Invoke(byteArray, packetId, connection);
                else if (typeof(T) == typeof(NetBuffer))
                    callback?.Invoke((T) (object) new NetBuffer {Data = data, LengthBytes = data.Length}, packetId, connection);
                else
                    callback?.Invoke(MessagePackSerializer.Deserialize<T>(data), packetId, connection);
            };
        }

        internal Action CreateCallAction(long routeId, long packetId, byte[] data, NetConnection connection)
        {
            return () =>
            {
                if (routes.TryGetValue(routeId, out var action))
                    action?.Invoke(data, packetId, connection);
            };
        }
    }
}
