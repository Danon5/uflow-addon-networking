using Cysharp.Threading.Tasks;

namespace UFlow.Addon.Networking.Core.Runtime {
    public interface INetworkClient : INetworker {
        UniTask ConnectAsync(string ip, ushort port = NetworkServer.DEFAULT_PORT, string key = null);
        UniTask DisconnectAsync();
    }
}