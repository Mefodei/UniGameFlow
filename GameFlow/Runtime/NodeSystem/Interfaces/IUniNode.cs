namespace UniGame.UniNodes.NodeSystem.Runtime.Interfaces
{
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IUniNode : 
        INode,
        IState
    {

        IContext Context { get; }

    }
}