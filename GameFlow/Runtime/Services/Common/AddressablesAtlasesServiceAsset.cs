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
        private AddressableSpriteAtlasConfiguration _configuration;
        
        protected override async UniTask<AddressablesAtlasesService> CreateServiceInternalAsync(IContext context)
        {
            var service = new AddressablesAtlasesService(_configuration);
            return service;
        }
    }
}
