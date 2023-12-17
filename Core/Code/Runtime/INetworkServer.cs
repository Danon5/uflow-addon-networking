using Cysharp.Threading.Tasks;

namespace UFlow.Addon.Networking.Core.Runtime {
    public interface INetworkServer : INetworker {
        UniTask StartAsync(ushort port = NetworkServer.DEFAULT_PORT, string key = NetworkServer.DEFAULT_KEY);
        UniTask StopAsync();
    }
}