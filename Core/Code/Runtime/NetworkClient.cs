using Cysharp.Threading.Tasks;
using LiteNetLib;
using UnityEngine;

namespace UFlow.Addon.Networking.Core.Runtime {
    public sealed class NetworkClient : INetworkClient {
        private readonly NetManager m_client;
        private bool m_started;
        
        public NetworkConnectionStateId ConnectionStateId { get; private set; }
        public bool StartingOrStarted => ConnectionStateId is NetworkConnectionStateId.Starting or NetworkConnectionStateId.Started;
        public bool StoppingOrStopped => ConnectionStateId is NetworkConnectionStateId.Stopping or NetworkConnectionStateId.Stopped;

        public NetworkClient() {
            var listener = new EventBasedNetListener();
            m_client = new NetManager(listener);
        }

        public async UniTask ConnectAsync(string ip, ushort port = NetworkServer.DEFAULT_PORT, string key = null) {
            if (string.IsNullOrEmpty(ip))
                ip = "localhost";
            if (ip == "localhost")
                ip = "127.0.0.1";
            key ??= NetworkServer.DEFAULT_KEY;
            m_client.Start();
            m_client.Connect(ip, port, key);
#if UFLOW_DEBUG_ENABLED
            Debug.Log($"Started connection attempt. IP: {ip}, Port: {port}, Key: {key}");
#endif
            ConnectionStateId = NetworkConnectionStateId.Starting;
            await UniTask.WaitUntil(() => ConnectionStateId != NetworkConnectionStateId.Starting);
        }

        public async UniTask DisconnectAsync() {
            m_client.Stop();
            ConnectionStateId = NetworkConnectionStateId.Stopping;
            await UniTask.WaitUntil(() => ConnectionStateId != NetworkConnectionStateId.Stopping);
        }
    }
}