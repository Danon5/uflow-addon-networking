using Cysharp.Threading.Tasks;
using LiteNetLib;
using UnityEngine;

namespace UFlow.Addon.Networking.Core.Runtime {
    public sealed class NetworkServer : INetworkServer {
        public event ConnectionStateChangedDelegate ConnectionStateChangedEvent;
        public event StartingDelegate StartingEvent;
        public event StartedDelegate StartedEvent;
        public event StoppingDelegate StoppingEvent;
        public event StoppedDelegate StoppedEvent;
        public const ushort DEFAULT_PORT = 7777;
        public const string DEFAULT_KEY = "Key";
        private readonly NetManager m_server;
        private NetworkConnectionState m_connectionState;
        private string m_key;

        public NetworkConnectionState ConnectionState {
            get => m_connectionState;
            private set {
                if (m_connectionState == value) return;
                m_connectionState = value;
                ConnectionStateChangedEvent?.Invoke(m_connectionState);
            }
        }
        public bool StartingOrStarted => ConnectionState is NetworkConnectionState.Starting or NetworkConnectionState.Started;
        public bool StoppingOrStopped => ConnectionState is NetworkConnectionState.Stopping or NetworkConnectionState.Stopped;
        
        public NetworkServer() {
            var listener = new EventBasedNetListener();
            listener.ConnectionRequestEvent += OnConnectionRequest;
            listener.PeerConnectedEvent += OnPeerConnected;
            listener.PeerDisconnectedEvent += OnPeerDisconnected;
            m_server = new NetManager(listener);
        }

        public void PollEvents() => m_server.PollEvents();
        
        public async UniTask StartAsync(ushort port = DEFAULT_PORT, string key = DEFAULT_KEY) {
            if (StartingOrStarted) {
#if UFLOW_DEBUG_ENABLED
                Debug.LogWarning($"Server already starting or started. ConnectionState: {ConnectionState}");
#endif
                return;
            }
            m_key = key;
            m_server.Start(port);
#if UFLOW_DEBUG_ENABLED
            Debug.Log($"Server starting. Port: {port}");
#endif
            ConnectionState = NetworkConnectionState.Starting;
            StartingEvent?.Invoke();
            await UniTask.WaitUntil(() => m_server.IsRunning || ConnectionState == NetworkConnectionState.Stopped);
#if UFLOW_DEBUG_ENABLED
            if (m_server.IsRunning)
                Debug.Log($"Server started. Port: {port}");
#endif
            ConnectionState = m_server.IsRunning ? NetworkConnectionState.Started : NetworkConnectionState.Stopped;
            if (m_server.IsRunning)
                StartedEvent?.Invoke();
        }
        public async UniTask StopAsync() {
            if (StoppingOrStopped) {
#if UFLOW_DEBUG_ENABLED
                Debug.LogWarning($"Server already stopping or stopped. ConnectionState: {ConnectionState}");
#endif
                return;
            }
            m_server.Stop(true);
#if UFLOW_DEBUG_ENABLED
            Debug.Log("Server stopping.");
#endif
            ConnectionState = NetworkConnectionState.Stopping;
            StoppingEvent?.Invoke();
            await UniTask.WaitUntil(() => !m_server.IsRunning);
#if UFLOW_DEBUG_ENABLED
            Debug.Log("Server stopped.");
#endif
            ConnectionState = NetworkConnectionState.Stopped;
            StoppedEvent?.Invoke();
        }

        private void OnConnectionRequest(ConnectionRequest request) {
            request.AcceptIfKey(m_key);
        }
        
        private void OnPeerConnected(NetPeer peer) {
#if UFLOW_DEBUG_ENABLED
            Debug.Log($"Peer connected. ID: {peer.Id}");
#endif
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
#if UFLOW_DEBUG_ENABLED
            Debug.Log($"Peer disconnected. ID: {peer.Id}");
#endif
        }
    }
}