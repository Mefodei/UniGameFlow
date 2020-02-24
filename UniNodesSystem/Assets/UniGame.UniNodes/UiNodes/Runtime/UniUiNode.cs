﻿namespace UniGame.UniNodes.UiNodes.Runtime
{
    using System.Collections;
    using System.Collections.Generic;
    using Interfaces;
    using NodeSystem.Runtime.Core;
    using NodeSystem.Runtime.Extensions;
    using NodeSystem.Runtime.NodeData;
    using UiData;
    using UniGreenModules.UniCore.Runtime.AsyncOperations;
    using UniGreenModules.UniCore.Runtime.DataFlow.Interfaces;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniGreenModules.UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniGreenModules.UniUiSystem.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using Debug = UnityEngine.Debug;

    [CreateNodeMenu("UI/UiView", "UiView")]
    public class UniUiNode : UniNode
    {
        #region inspector

        [HideInInspector] [SerializeField] private string viewName = "UiNode";

        public ObjectInstanceData options;

        public AssetReferenceGameObject resource;

        #endregion

        private List<PortValue> uiTriggersOutputs = new List<PortValue>();
        private List<PortValue> uiModulesOutputs  = new List<PortValue>();

        private List<PortValue> slotPorts;
        private List<PortValue> triggersPorts;


        private AsyncOperationHandle<GameObject> UiViewHandle => resource.LoadAssetAsync();


        public override string GetName() => viewName;

        public bool Validate(IContext context)
        {
            if (UiViewHandle.Status == AsyncOperationStatus.None || UiViewHandle.Status == AsyncOperationStatus.Failed) {
                Debug.LogErrorFormat("NULL UI VIEW {0} {1}", UiViewHandle, this);
                return false;
            }

            return true;
        }

        protected IEnumerator OnExecuteState(IContext context)
        {
            var lifetime = LifeTime;

            //load view
            //TODO take shared object
            yield return UiViewHandle.Task.AwaitTask();

            if (UiViewHandle.Status == AsyncOperationStatus.None || UiViewHandle.Status == AsyncOperationStatus.Failed) {
                Debug.LogError(UiViewHandle);
                yield break;
            }

            var viewPrefab = UiViewHandle.Result.GetComponent<UiModule>();
            var view       = CreateView(viewPrefab);

            BindModulesPorts(view, context, lifetime);

            BindTriggers(view, context, lifetime);

            //initialize view with input data
            //view.Initialize(Input);

            ApplyViewSettings(view, lifetime);

            //yield return base.OnExecute(context);
            yield break;
        }

        /// <summary>
        /// add ui output context
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        protected void OnUiTriggerAction(IInteractionTrigger trigger, IContext context)
        {
            var port = GetPort(trigger.ItemName);
            var portValue = port.Value;
            
            if (trigger.IsActive) {
                portValue.Publish(context);
            }    
        }

        protected void OnRegisterPorts()
        {
            //base.OnRegisterPorts();

            uiModulesOutputs.Clear();
            uiTriggersOutputs.Clear();

#if UNITY_EDITOR

            var viewObject = resource.editorAsset as GameObject;

            if (!viewObject)
                return;

            var view = viewObject.GetComponent<UiModule>();
            if (!view)
                return;

            viewName = view.name;

            UpdateUiPorts(view);
#endif
        }

        private void UpdateUiPorts(UiModule uiModule)
        {
            if (!uiModule) {
                return;
            }

            UpdateTriggers(uiModule);
            UpdateModulesSlots(uiModule);
        }

        private UiModule CreateView(UiModule source)
        {
            //todo add cached version from global manager
            var uiView = ObjectPool.Spawn<UiModule>(source, options.Position,
                Quaternion.identity, options.Parent, options.StayAtWorld);

            //bind main view to input data
            //uiView.Initialize(Input);

            return uiView;
        }

        private void ApplyViewSettings(UiModule uiView, ILifeTime lifetime)
        {
            lifetime.AddCleanUpAction(() => {
                if (uiView != null) {
                    uiView?.Despawn();
                }
            });

            uiView.gameObject.SetActive(true);
            uiView.UpdateView();
        }

        /// <summary>
        /// Bind output ports to ui moduls
        /// </summary>
        private void BindModulesPorts(IUiModule view, IContext context, ILifeTime lifetime)
        {
            var slotContainer = view.Slots;

            var slots = slotContainer.Items;

            for (var i = 0; i < slots.Count; i++) {
                //get associated port value by slot
                var slot      = slots[i];
                var port = GetPort(slot.SlotName);
                var portValue = port.Value;
                
                //connect to ui module data
                slot.Value.Bind(portValue).
                    AddTo(lifetime);

                //add new placement value
                portValue.Publish<IUiPlacement>(slot);
                //set node context
                portValue.Publish(context);
            }
        }

        private void BindTriggers(IUiModule view, IContext context, ILifeTime lifetime)
        {
            var triggers               = view.Triggers;
            var interactionsDisposable = triggers.TriggersObservable.Subscribe(x => OnUiTriggerAction(x, context));

            lifetime.AddDispose(interactionsDisposable);
        }

        private void UpdateTriggers(IUiModule view)
        {
            var triggers = view.Triggers;

            foreach (var handler in triggers.Items) {
                var values = this.CreatePortPair(handler.ItemName, true);
                //uiTriggersOutputs.Add(values.outputValue);
            }
        }

        private void UpdateModulesSlots(IUiModule view)
        {
            var slots = view.Slots.Items;

            for (var i = 0; i < slots.Count; i++) {
                var slot       = slots[i];
                var outputPort = this.UpdatePortValue(slot.SlotName, PortIO.Output);
                //uiModulesOutputs.Add(outputPort.value);
            }
        }
    }
}