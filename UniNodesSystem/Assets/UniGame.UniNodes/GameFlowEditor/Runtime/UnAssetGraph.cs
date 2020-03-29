﻿using GraphProcessor;

namespace UniGame.GameFlowEditor.Runtime
{
    using System;
    using System.Collections.Generic;
    using UniNodes.NodeSystem.Runtime.Core;
    using UniNodes.NodeSystem.Runtime.Interfaces;
    using UnityEditor;
    using Vector2 = UnityEngine.Vector2;

    public class UniAssetGraph : BaseGraph
    {
        private UniGraph sourceGraph;
        private SerializedObject serializableObject;
        
        public Dictionary<int,UniBaseNode> uniNodes = new Dictionary<int,UniBaseNode>(16);

        public UniGraph UniGraph => sourceGraph;

        public void Activate(UniGraph graph)
        {
            sourceGraph = graph;
            sourceGraph.Initialize();
            serializableObject = new SerializedObject(sourceGraph);
            UpdateGraph();
        }

        public void RemoveUniNode(BaseNode node)
        {
            if (node is UniBaseNode targetNode) {
                sourceGraph.RemoveNode(targetNode.SourceNode);
            }
            RemoveNode(node);
        }

        public UniBaseNode CreateNode(Type type, Vector2 nodePosition)
        {
            var newNode = sourceGraph.AddNode(
                type,
                ObjectNames.NicifyVariableName(type.Name),
                nodePosition);
            return CreateNode(newNode);
        }

        public UniBaseNode CreateNode(INode node)
        {
            
            var graphNode = BaseNode.CreateFromType<UniBaseNode>(node.Position);
            graphNode.Initialize(node);
            
            //register node into all nodes list
            AddNode(graphNode);
            
            //register only uni nodes
            uniNodes[node.Id] = graphNode;
            
            //sourceGraph.Save();
            return graphNode;
        }

        public void UpdateGraph()
        {
            CreateNodes();
            ConnectNodePorts();
        }

        private void CreateNodes()
        {
            foreach (var node in sourceGraph.Nodes) {
                CreateNode(node);
            }
        }

        private void ConnectNodePorts()
        {
            foreach (var nodeItem in uniNodes) {
                var nodeView = nodeItem.Value;
                var node     = nodeView.SourceNode;
                foreach (var outputPortView in nodeView.outputPorts) {
                    
                    var portData = outputPortView.portData;
                    var sourcePort = node.GetPort(portData.displayName);
                    
                    foreach (var connection in sourcePort.Connections) {
                        var targetNodeView = uniNodes[connection.NodeId];
                        var targetNode = targetNodeView.SourceNode;
                        var port = targetNode.GetPort(connection.PortName);
                        
                        if(port.Direction != PortIO.Input)
                            continue;
                        
                        var inputPortView = targetNodeView.
                            GetPort(nameof(targetNodeView.inputs),connection.PortName);
                        
                        Connect(inputPortView,outputPortView);
                    }
                }
            }
        }
    }
}