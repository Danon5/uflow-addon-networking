using Cysharp.Threading.Tasks;
using LiteNetLib;

namespace UFlow.Addon.Networking.Core.Runtime {
    public sealed class NetworkServer : INetworkServer {
        public const ushort DEFAULT_PORT = 7777;
        public const string DEFAULT_KEY = "Key";
        private readonly NetManager m_server;
        private bool m_started;

        public NetworkServer() {
            var listener = new EventBasedNetListener();
            m_server = new NetManager(listener);
        }

        public UniTask StartAsync() {
            return default;
        }
        public UniTask StopAsync() {
            return default;
        }
    }
}