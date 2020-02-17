﻿namespace UniGreenModules.UniGameSystems.Runtime.Nodes
{
    using UniCore.Runtime.Attributes;
    using UniCore.Runtime.Rx.Extensions;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.Nodes.Runtime.Nodes;
    using UniGameFlow.UniNodesSystem.Assets.UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGameSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// Base game service binder between Unity world and regular classes
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TServiceApi"></typeparam>
    [HideNode]
    public abstract class ServiceNode<TService,TServiceApi> : 
        ContextNode
        where TServiceApi : IGameService
        where TService : TServiceApi
    {
        [SerializeField]
        protected TService service;

        #region inspector

        [Header("Service Status")]
        [ReadOnlyValue]
        [SerializeField]
        private bool isReady;
        
        #endregion
        
        public bool waitForServiceReady = true;

        protected abstract TService CreateService();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            service = CreateService();
        }

        protected override void OnExecute()
        {
            Source.Where(x => x != null).
                Do(x => service.Bind(x,LifeTime)).
                CombineLatest(service.IsReady, (ctx, ready) => (ctx,ready)).
                Where(x => x.ready || !waitForServiceReady).
                Do(x => x.ctx.Publish<TServiceApi>(service)).
                Do(x => Finish()).
                Subscribe().
                AddTo(LifeTime);
        }

    }
}