﻿namespace UniGame.UniNodes.GameFlow.Runtime.Commands
{
    using System;
    using Interfaces;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;

    public class GameServiceCommand : ILifeTimeCommand
    {
        public GameServiceCommand(Func<IGameService> service)
        {
        
        }
    
        public void Execute(ILifeTime lifeTime)
        {
            throw new System.NotImplementedException();
        }
    }
}