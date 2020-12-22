namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IBroadcastConnector<TConnection> : 
        IContextWriter, 
        IBroadcaster<TConnection>
    {
    }
}