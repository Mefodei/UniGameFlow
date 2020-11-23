﻿namespace UniGame.UniNodes.NodeSystem.Runtime.Core.Nodes
{
    using System;
    using System.Collections.Generic;
    using UniGame.UniNodes.NodeSystem.Runtime.Attributes;
    using UniGame.UniNodes.NodeSystem.Runtime.Core;
    using UniGame.UniNodes.NodeSystem.Runtime.Core.Interfaces;
    using UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;

    [HideNode]
    [Serializable]
    public class DummyNode : INode
    {
        public int    Id       { get; private set; }
        public string ItemName { get; private set;  }

        public IContext Context { get; }
        public IGraphData              GraphData { get; }
        public IEnumerable<INodePort> Ports     { get; }
        public IEnumerable<INodePort>   Outputs   { get; }
    
        public IEnumerable<INodePort> Inputs { get; }

        public INodePort GetOutputPort(string fieldName) => null;

        public INodePort GetInputPort(string fieldName) => null;

        public INodePort GetPort(string fieldName) => null;

        public bool HasPort(string fieldName) => false;

        public bool AddPortValue(INodePort portValue)
        {
            return false;
        }

        public int SetId(int id) => Id = id;

        public Vector2 Position { get; set; } = Vector2.zero;
        
        public int Width { get; set; } = 220;
        

        public void OnIdUpdate(int oldId, int newId, IGraphItem updatedItem) { }
        
        public void SetUpData(IGraphData data) { }

        public void SetName(string nodeName) { }

        public void RemovePort(string fieldName) { }

        public void RemovePort(INodePort port) { }

        public void ClearConnections() { }
        public void Initialize(IGraphData data) {}

        public void Validate(){}

        public void SetPosition(Vector2 position) { }
        
        public void SetWidth(int nodeWidth) {}

        public NodePort AddPort(string fieldName,
            IEnumerable<Type> types, 
            PortIO direction,
            ConnectionType connectionType = ConnectionType.Multiple,
            ShowBackingValue showBackingValue = ShowBackingValue.Always) => null;

    }
}
