﻿namespace UniGame.UniNodes.NodeSystem.Runtime.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Attributes;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniGame.Context.Runtime.Context;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class PortValue : IPortValue, ISerializationCallbackReceiver
    {
        #region serialized data

        /// <summary>
        /// port value Name
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// allowed port value types
        /// </summary>
        [SerializeField]
        protected List<string> serializedValueTypes;

        [SerializeField] [ReadOnlyValue] protected int broadcastersCount;

        #endregion

        #region private property

        private EntityContext data;

        private ReactiveCommand portValueChanged = new ReactiveCommand();

        private ILifeTime lifeTime;

        private LifeTimeDefinition lifeTimeDefeDefinition = new LifeTimeDefinition();

        private List<Type> valueTypeFilter;

        #endregion

        #region constructor

        public PortValue()
        {
            Initialize();
        }

        #endregion

        #region public properties

        public IReadOnlyList<Type> ValueTypes => valueTypeFilter = valueTypeFilter ?? new List<Type>();

        public ILifeTime LifeTime => lifeTimeDefeDefinition.LifeTime;

        public string ItemName => name;

        public bool HasValue => data.HasValue;

        public IObservable<Unit> PortValueChanged => portValueChanged;

        public bool IsValidPortValueType(Type type)
        {
            if (valueTypeFilter == null || valueTypeFilter.Count == 0)
                return true;
            return valueTypeFilter.Contains(type);
        }

        #endregion
        
        #region connection api

        public int ConnectionsCount => data.ConnectionsCount;

        public void Disconnect(IMessagePublisher connection) {
            data.Disconnect(connection);
        }

        public IDisposable Bind(IMessagePublisher contextData)
        {
            var disposable = data.Bind(contextData);
            broadcastersCount = data.ConnectionsCount;
            return disposable;
        }

        #endregion
        
        public void Initialize(string portName)
        {
            name = portName;
            Initialize();
        }


        public void SetValueTypeFilter(IReadOnlyList<Type> types)
        {
            valueTypeFilter = valueTypeFilter ?? new List<Type>();
            valueTypeFilter.Clear();
            valueTypeFilter.AddRange(types);

            UpdateSerializedFilter(valueTypeFilter);
        }

        public void Dispose() => Release();

        public void Release() => lifeTimeDefeDefinition.Terminate();

        #region type data container

        public bool Remove<TData>()
        {
            var result = data.Remove<TData>();
            if (result) {
                portValueChanged.Execute(Unit.Default);
            }

            return result;
        }

        public void Publish<TData>(TData value)
        {
            if (valueTypeFilter != null &&
                valueTypeFilter.Count != 0 &&
                !valueTypeFilter.Contains(typeof(TData))) {
                return;
            }

            data.Publish(value);
            portValueChanged.Execute(Unit.Default);
        }

        public void RemoveAllConnections() => data.Release();

        public TData Get<TData>() => data.Get<TData>();

        public bool Contains<TData>() => data.Contains<TData>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObservable<TValue> Receive<TValue>() => data.Receive<TValue>();

        #endregion

        private bool DefaultFilter(Type type) => true;

        private void Initialize()
        {
            lifeTimeDefeDefinition = lifeTimeDefeDefinition ?? new LifeTimeDefinition();
            lifeTime               = lifeTimeDefeDefinition.LifeTime;
            lifeTimeDefeDefinition.Release();

            data        = data ?? new EntityContext();

            lifeTime.AddCleanUpAction(data.Release);
            lifeTime.AddCleanUpAction(RemoveAllConnections);
        }

        #region serialization rules

        public void OnBeforeSerialize()
        {
            UpdateSerializedFilter(valueTypeFilter);
        }

        public void OnAfterDeserialize()
        {
            valueTypeFilter = valueTypeFilter ?? new List<Type>();
            valueTypeFilter.Clear();

            for (var i = 0; i < serializedValueTypes.Count; i++) {
                var typeFilter = serializedValueTypes[i];
                var type       = Type.GetType(typeFilter, false, true);
                if (type != null)
                    valueTypeFilter.Add(type);
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void UpdateSerializedFilter(IReadOnlyList<Type> filter)
        {
            serializedValueTypes = filter == null ? new List<string>() : filter.Select(x => x.AssemblyQualifiedName).ToList();
        }

        #endregion


        #region Unity Editor Api

#if UNITY_EDITOR

        public IReadOnlyDictionary<Type, IValueContainerStatus> Values => data.EditorValues;

#endif

        #endregion

    }
}