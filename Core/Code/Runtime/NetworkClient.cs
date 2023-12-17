using Cysharp.Threading.Tasks;
using LiteNetLib;
using UnityEngine;

namespace UFlow.Addon.Networking.Core.Runtime {
    public sealed class NetworkClient : INetworkClient {
        public event ConnectionStateChangedDelegate ConnectionStateChangedEvent;
        public event StartingDelegate StartingEvent;
        public event StartedDelegate StartedEvent;
        public event StoppingDelegate StoppingEvent;
        public event StoppedDelegate StoppedEvent;
        private readonly NetManager m_client;
        private NetworkConnectionState m_connectionState;
        
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

        public NetworkClient() {
            var listener = new EventBasedNetListener();
            listener.PeerConnectedEvent += OnPeerConnected;
            listener.PeerDisconnectedEvent += OnPeerDisconnected;
            m_client = new NetManager(listener);
        }

        public void PollEvents() => m_client.PollEvents();

        public async UniTask ConnectAsync(string ip, ushort port = NetworkServer.DEFAULT_PORT, string key = null) {
            if (StartingOrStarted) {
#if UFLOW_DEBUG_ENABLED
                Debug.LogWarning($"Client already starting or started. ConnectionState: {ConnectionState}");
#endif
                return;
            }
            if (string.IsNullOrEmpty(ip))
                ip = "localhost";
            if (ip == "localhost")
                ip = "127.0.0.1";
            key ??= NetworkServer.DEFAULT_KEY;
            m_client.Start();
            m_client.Connect(ip, port, key);
#if UFLOW_DEBUG_ENABLED
            Debug.Log($"Client starting. IP: {ip}, Port: {port}, Key: {key}");
#endif
            ConnectionState = NetworkConnectionState.Starting;
            await UniTask.WaitUntil(() => m_client.IsRunning || ConnectionState == NetworkConnectionState.Stopped);
#if UFLOW_DEBUG_ENABLED
            if (m_client.IsRunning)
                Debug.Log($"Client started. IP: {ip}, Port: {port}, Key: {key}");
#endif
            ConnectionState = m_client.IsRunning ? NetworkConnectionState.Started : NetworkConnectionState.Stopped;
            if (m_client.IsRunning)
                StartedEvent?.Invoke();
        }

        public async UniTask DisconnectAsync() {
            if (StoppingOrStopped) {
#if UFLOW_DEBUG_ENABLED
                Debug.LogWarning($"Client already stopping or stopped. ConnectionState: {ConnectionState}");
#endif
                return;
            }
            m_client.Stop();
#if UFLOW_DEBUG_ENABLED
            Debug.Log("Client stopping.");
#endif
            ConnectionState = NetworkConnectionState.Stopping;
            StoppingEvent?.Invoke();
            await UniTask.WaitUntil(() => !m_client.IsRunning);
#if UFLOW_DEBUG_ENABLED
            Debug.Log("Client stopped.");
#endif
            ConnectionState = NetworkConnectionState.Stopped;
            StoppedEvent?.Invoke();
        }

        private void OnPeerConnected(NetPeer peer) {
#if UFLOW_DEBUG_ENABLED
            Debug.Log("Client connected.");
#endif
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
#if UFLOW_DEBUG_ENABLED
            Debug.Log($"Client disconnected. Reason: {disconnectInfo.Reason}");
#endif
            DisconnectAsync().Forget();
        }
    }
}