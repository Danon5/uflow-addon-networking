using Cysharp.Threading.Tasks;

namespace UFlow.Addon.Networking.Core.Runtime {
    public interface INetworkServer {
        UniTask StartAsync();
        UniTask StopAsync();
    }
}