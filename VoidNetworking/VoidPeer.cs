using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Lidgren.Network;
using MessagePack;
using VoidNetworking.ModulesFramework;

namespace VoidNetworking
{
    public abstract class VoidPeer: ActionQueue
    {
        protected const float DefaultTimeOut = 10;
        protected const float DefaultPingInterval = 1;
        public readonly Routing Routes = new Routing();

        public event Action<NetConnection> OnConnected;
        public event Action<NetConnection> OnDisconnected;
        public event Action<IPEndPoint> OnServerDiscovery;
        protected abstract NetPeer CurrentPeer { get; }

        private readonly PacketIdGenerator packetIdGenerator = new PacketIdGenerator();
        private readonly Routing responseRouting = new Routing();

        protected readonly string AppId;

        internal VoidPeer(string appId)
        {
            AppId = appId;
            RegisterModules();
        }

        public void Shutdown() => CurrentPeer.Shutdown("");

        public void SendMessage<T>(long routeId, T message, NetConnection connection, RouteType routeType=RouteType.Route)
        {
            SendMessage<object, T>(routeId, message, connection, null, routeType);
        }

        public void SendMessage<TResponse, T>(
            long routeId, T message, NetConnection connection, Routing.PacketReceivedRequestHandler<TResponse> responseHandler, RouteType routeType = RouteType.Route)
        {
            VoidLog.LogMessage($"prepare message to transfer by route(type:{routeType}, id:{routeId})");
            if (CurrentPeer is NetServer && connection?.Status != NetConnectionStatus.Connected)
            {
                VoidLog.LogWarning($"connection is not alive. Status:{connection?.Status}. Skipping message");
                return;
            }
            if (CurrentPeer is NetClient client && client.ConnectionStatus != NetConnectionStatus.Connected)
            {
                VoidLog.LogWarning($"Client is not alive. Status:{client.ConnectionStatus}");
                return;
            }

            VoidLog.LogMessage($"generating id...");
            var packetId = packetIdGenerator.Generate();
            if (responseHandler != null)
                responseRouting.RegisterRoute(packetId, responseHandler);
            var outgoingMessage = GenerateOutgoingMessage();
            outgoingMessage.Write((int)routeType);
            outgoingMessage.Write(routeId);
            outgoingMessage.Write(packetId);
            VoidLog.LogMessage($"encoding message...");
            switch (message)
            {
                case NetBuffer netBuffer:
                    outgoingMessage.Write(netBuffer);
                    break;
                case byte[] byteArray:
                    outgoingMessage.Write(byteArray);
                    break;
                default:
                {
                    var data = MessagePackSerializer.Serialize(message);
                    outgoingMessage.Write(data);
                    break;
                }
            }

            VoidLog.LogMessage($"prepare to send...{outgoingMessage.LengthBits}bits");
            switch (CurrentPeer)
            {
                case NetClient clientPeer:
                    clientPeer.SendMessage(outgoingMessage, NetDeliveryMethod.ReliableOrdered, 0);
                    break;
                case NetServer serverPeer:
                    serverPeer.SendMessage(outgoingMessage, connection, NetDeliveryMethod.ReliableOrdered, 0);
                    break;
            }
            VoidLog.LogMessage($"message sent with id: {packetId}");
        }

        internal void OnConnectedHappened(NetConnection connection) => OnConnected?.Invoke(connection);
        internal void OnDisconnectedHappened(NetConnection connection) => OnDisconnected?.Invoke(connection);

        protected abstract NetOutgoingMessage GenerateOutgoingMessage();

        protected void OnMessageReceived(object peer)
        {
            var message = CurrentPeer.ReadMessage();
            var connection = message.SenderConnection;
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DiscoveryRequest:
                    var server = (NetServer) CurrentPeer;
                    var response = server.CreateMessage();
                    response.Write(AppId);
                    server.SendDiscoveryResponse(response, message.SenderEndPoint);
                    break;
                case NetIncomingMessageType.DiscoveryResponse:
                    if (message.ReadString(out var appId) && appId == AppId)
                        OnServerDiscovery?.Invoke(message.SenderEndPoint);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    if (connection.Status == NetConnectionStatus.Connected)
                        EnqueueAction(() => OnConnected?.Invoke(connection));
                    else if (connection.Status == NetConnectionStatus.Disconnected)
                        EnqueueAction(() => OnDisconnected?.Invoke(connection));
                    break;
                case NetIncomingMessageType.Data:
                    VoidLog.LogMessage($"received {message.LengthBits}bits");
                    try
                    {
                        var routeType = (RouteType)message.ReadInt32();
                        var routeId = message.ReadInt64();
                        var packetId = message.ReadInt64();
                        var data = message.ReadBytes(message.LengthBytes - message.PositionInBytes);
                        var action = routeType == RouteType.Route
                            ? Routes.CreateCallAction(routeId, packetId, data, connection)
                            : responseRouting.CreateCallAction(routeId, packetId, data, connection);
                        EnqueueAction(action);
                    }
                    catch (Exception e)
                    {
                        VoidLog.LogError(e.ToString());
                    }
                    break;
            }
        }

        protected static void KeepOrCreateSynchronizationContext()
        {
            if (SynchronizationContext.Current is null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        private void RegisterModules()
        {
            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes<ModuleAttribute>().Any() && type.IsAbstract && type.IsSealed)
                .SelectMany(type => type.GetMethods(BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public))
                .Where(method => method.GetCustomAttributes<ModuleReceiverAttribute>().Any())
                .ToList()
                .ForEach(method =>
                {
                    var receiver = method.GetCustomAttribute<ModuleReceiverAttribute>();
                    var parameters = method.GetParameters();

                    var is3Args = parameters.Length == 3 && parameters[1].ParameterType == typeof(long) &&
                                  parameters[2].ParameterType == typeof(NetConnection);
                    var is2Args = parameters.Length == 2 && parameters[1].ParameterType == typeof(NetConnection);

                    if (!is3Args && !is2Args)
                        return;

                    var delegateType = is3Args
                        ? typeof(Routing.PacketReceivedRequestHandler<>)
                        : typeof(Routing.PacketReceivedHandler<>);
                    
                    var genericDelegateType = delegateType.MakeGenericType(parameters[0].ParameterType);
                    var receiverDelegate = Delegate.CreateDelegate(genericDelegateType, method);
                    
                    var registerRouteMethod = Routes.GetType()
                        .GetMethods()
                        .FirstOrDefault(m => m.Name == "RegisterRoute" && m.GetParameters()[1].ParameterType.Name == delegateType.Name) 
                        ?.MakeGenericMethod(parameters[0].ParameterType);
                    registerRouteMethod?.Invoke(Routes, new object[] {receiver.RouteId, receiverDelegate});
                });
        }
    }
}
