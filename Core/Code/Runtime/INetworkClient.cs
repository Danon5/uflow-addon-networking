using Cysharp.Threading.Tasks;

namespace UFlow.Addon.Networking.Core.Runtime {
    public interface INetworkClient {
        NetworkConnectionStateId ConnectionStateId { get; }
        bool StartingOrStarted { get; }
        bool StoppingOrStopped { get; }
        UniTask ConnectAsync(string ip, ushort port = NetworkServer.DEFAULT_PORT, string key = null);
        UniTask DisconnectAsync();
    }
}