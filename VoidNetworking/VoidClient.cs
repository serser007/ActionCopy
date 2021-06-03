using Lidgren.Network;

namespace VoidNetworking
{
    public class VoidClient: VoidPeer
    {
        public bool IsAlive => ConnectionStatus != NetConnectionStatus.Disconnected && ConnectionStatus != NetConnectionStatus.None;

        protected override NetPeer CurrentPeer => client;

        private readonly NetClient client;
        private NetConnectionStatus ConnectionStatus => client?.ConnectionStatus ?? NetConnectionStatus.Disconnected;

        public VoidClient(float timeout = DefaultTimeOut, string appId="V0idNetworkingApp") : base(appId)
        {
            var config = new NetPeerConfiguration(appId) {ConnectionTimeout = timeout, PingInterval = DefaultPingInterval };
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
            KeepOrCreateSynchronizationContext();
            client.RegisterReceivedCallback(OnMessageReceived);
            client.Start();
        }

        public void SendDiscoveryRequest(int port)
        {
            client.DiscoverLocalPeers(port);
        }

        public (NetConnection connection, bool isSuccess) Connect(string host, int port)
        {
            try
            {
                var connection = client.Connect(host, port);
                return (connection, connection != null);
            }
            catch
            {
                return (null, false);
            }
        }

        public void Disconnect()
        {
            try
            { 
                client.Disconnect("");
            }
            catch
            {
                // ignored
            }
        }

        public void SendMessage<T>(long routeId, T message, RouteType routeType=RouteType.Route) => SendMessage(routeId, message, null, routeType);

        public void SendMessage<TResponse, T>(long routeId, T message, Routing.PacketReceivedRequestHandler<TResponse> responseHandler, RouteType routeType=RouteType.Route)
        {
            SendMessage(routeId, message, null, responseHandler, routeType);
        }

        protected override NetOutgoingMessage GenerateOutgoingMessage() => client.CreateMessage();
    }
}
