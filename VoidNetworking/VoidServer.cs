using Lidgren.Network;

namespace VoidNetworking
{
    public class VoidServer: VoidPeer
    {
        protected override NetPeer CurrentPeer => server;

        private readonly NetServer server;

        public VoidServer(int port, float timeout=DefaultTimeOut, string appId = "V0idNetworkingApp") : base(appId)
        {
            var config = new NetPeerConfiguration(appId) {Port = port, ConnectionTimeout = timeout, PingInterval = DefaultPingInterval};
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            server = new NetServer(config);
            KeepOrCreateSynchronizationContext();
            server.RegisterReceivedCallback(OnMessageReceived);
        }

        public void Start() => server.Start();

        protected override NetOutgoingMessage GenerateOutgoingMessage() => server.CreateMessage();
    }
}
