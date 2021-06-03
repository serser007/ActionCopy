using System;

namespace VoidNetworking
{
    internal class PacketIdGenerator
    {
        private long packetId = Int64.MinValue;

        public long Generate() => packetId++;
    }
}
