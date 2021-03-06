﻿namespace UniGame.UniNodes.GameFlow.Runtime.Nodes
{
    using System.Collections.Generic;
    using Commands;
    using Cysharp.Threading.Tasks;
    using NodeSystem.Runtime.Core;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniModules.UniGameFlow.NodeSystem.Runtime.Core.Attributes;
    using UniNodes.Nodes.Runtime.Common;
    

    [CreateNodeMenu("GameSystem/Data Source")]
    public class ContextDataSourceNode : InOutPortNode
    {
        public AssetReferenceDataSource contextDataSource;

        protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
        {
            base.UpdateCommands(nodeCommands);

            //create sync result for task
            var outputContextTarget = UniTask.FromResult<IContext>(PortPair.OutputPort);
            //create node commands
            var sourceOutputPortCommand = new 
                RegisterDataSourceCommand(outputContextTarget,contextDataSource);
            
            nodeCommands.Add(sourceOutputPortCommand);
        }

    }
}
