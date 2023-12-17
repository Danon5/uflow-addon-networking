namespace UFlow.Addon.Networking.Core.Runtime {
    public interface INetworker {
        event ConnectionStateChangedDelegate ConnectionStateChangedEvent;
        event StartingDelegate StartingEvent;
        event StartedDelegate StartedEvent;
        event StoppingDelegate StoppingEvent;
        event StoppedDelegate StoppedEvent;
        NetworkConnectionState ConnectionState { get; }
        bool StartingOrStarted { get; }
        bool StoppingOrStopped { get; }
        void PollEvents();
    }
}