namespace UFlow.Addon.Networking.Core.Runtime {
    public delegate void ConnectionStateChangedDelegate(NetworkConnectionState connectionState);
    public delegate void StartingDelegate();
    public delegate void StartedDelegate();
    public delegate void StoppingDelegate();
    public delegate void StoppedDelegate();
}