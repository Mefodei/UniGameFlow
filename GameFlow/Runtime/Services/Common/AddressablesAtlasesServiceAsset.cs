﻿using UnityEngine;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using Systems;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniRx.Async;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Services/AddressablesAtlasesService",fileName = nameof(AddressablesAtlasesService))]
    public class AddressablesAtlasesServiceAsset : ContextService<AddressablesAtlasesService>
    {
        [SerializeField]
        private AssetReferenceAtlasConfiguration _configuration;
        
        protected override async UniTask<AddressablesAtlasesService> CreateServiceInternalAsync(IContext context)
        {
            var handler = await _configuration.LoadAssetTaskAsync(context.LifeTime);
            var service = new AddressablesAtlasesService(handler);
            return service;
        }
    }
}