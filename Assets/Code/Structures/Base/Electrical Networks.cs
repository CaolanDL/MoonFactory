using Logistics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Electrical
{
    public class SystemManager
    {
        public static List<Network> networks = new();
        public static List<Relay> relays = new();


        public SystemManager()
        {
            GameManager.OnStartNewGame += Reset;
            GameManager.OnGameExit += Reset;
        }

        void Reset() { networks.Clear(); }

        public static void MergeNetworks(Network a, Network b)
        {
            var LargestNetwork = a.nodes.Count > b.nodes.Count ? a : b;
            var SmallerNetwork = a == LargestNetwork ? b : a;
             
            LargestNetwork.nodes.AddRange(SmallerNetwork.nodes);
            LargestNetwork.relays.AddRange(SmallerNetwork.relays);
            LargestNetwork.inputs.AddRange(SmallerNetwork.inputs);
            LargestNetwork.sinks.AddRange(SmallerNetwork.sinks);

            foreach (var node in LargestNetwork.nodes)
            {
                node.Network = LargestNetwork;
            }

            SmallerNetwork.Destroy();

            LargestNetwork.Update();
        }

        public static void Tick()
        {
            foreach (var network in networks)
            {
                network.Update();
            }
        }
    }

    public class Network
    {
        public List<Node> nodes = new();
        public List<Relay> relays = new();
        public List<Input> inputs = new();
        public List<Sink> sinks = new();

        public float MaxInput = 0f;
        public float TotalInput = 0f;
        public float MaxConsumption = 0f;
        public float TotalConsumption = 0f;

        public float PowerRatio;
        /// <summary> Power ratio clamped 01 </summary>
        public float ClampedPowerRatio;

        public Network()
        {
            SystemManager.networks.Add(this);
        }

        public void Update()
        {
            MaxInput = 0;
            TotalInput = 0;
            MaxConsumption = 0;
            TotalConsumption = 0;

            foreach(var input in inputs)
            {
                MaxInput += input.MaxProduction;
                TotalInput += input.Production;
            }
            foreach (var sink in sinks)
            {
                MaxConsumption += sink.MaxConsumption;
                TotalConsumption += sink.Consumption;
            } 

            PowerRatio = TotalInput / TotalConsumption;
            ClampedPowerRatio = Mathf.Clamp01(PowerRatio);

            TotalConsumption = Mathf.Clamp(TotalConsumption, 0, TotalInput);
        }

        public void Add(Node node)
        {
            nodes.Add(node);
            if (node.GetType() == typeof(Relay)) relays.Add((Relay)node);
            if (node.GetType() == typeof(Input)) inputs.Add((Input)node);
            if (node.GetType() == typeof(Sink)) sinks.Add((Sink)node);
            node.Network = this;
            Update();
        }

        public void Remove(Node node)
        {
            nodes.Remove(node);
            if (node.GetType() == typeof(Relay)) relays.Remove((Relay)node);
            if (node.GetType() == typeof(Input)) inputs.Remove((Input)node);
            if (node.GetType() == typeof(Sink)) sinks.Remove((Sink)node);
            node.Network = null;
            if (nodes.Count == 0)
            {
                Destroy();
                return;
            }
            Update();
        }

        public void Destroy()
        {
            SystemManager.networks.Remove(this);
        }

    }

    public class Node
    {
        public Structure Parent;
        public Network Network;
        public List<Connection> Connections = new();
        public bool CanConnect = true;

        public void Constructed()
        {
            Parent.OnDemolishedEvent += Demolished; 
            OnConstructed();
        }

        public void Demolished()
        {
            CanConnect = false;
            DestroyAllConnections(); 
            Parent.OnDemolishedEvent -= Demolished;
            OnDemolished();
        }

        public virtual void OnConstructed() { }
        public virtual void OnDemolished() { }

        public bool TryConnectCleanly(Node node, int MaxConnections)
        {
            if (Connections.Count > MaxConnections) return false;
            if (node.Connections.Count > MaxConnections) return false;

            if (IsConnectedTo(node)) { return false; }
            if (node.CanConnect == false) { return false; }

            //! Big Smart: Only connect to node, if none of its connected nodes are already connected to this one 
            foreach (var connection in node.Connections)
            {
                var otherNode = connection.GetOther(node);
                if (otherNode.IsConnectedTo(this)) return false;
            }  

            CreateConnectionTo(node);
            return true;
        }

        public void CreateConnectionTo(Node other)
        {
            var newConnection = new Connection(this, other);
            Connections.Add(newConnection);
            other.Connections.Add(newConnection); 

            // If the other node is the only node in its network absorb it into this one.
            if (Network != null && other.Network != null && other.Network.nodes.Count == 1)
            {
                other.Network.Destroy();
                Network.Add(other);
            }
            // If this node is the only in its network, do the opposite.
            else if (Network != null && other.Network != null && Network.nodes.Count == 1)
            {
                Network.Destroy();
                other.Network.Add(this);
            }
            // If both nodes have networks join both the networks together with a merge call.
            else if (Network != null && other.Network != null && Network != other.Network)
            {
                SystemManager.MergeNetworks(Network, other.Network);
            }
            // If this node has no network but the other node does, join its network.
            else if (Network == null && other.Network != null)
            {
                other.Network.Add(this);
            }
            // If this node has a network and the other does not, other joins this nodes network.
            else if (Network != null && other.Network == null)
            {
                Network.Add(other);
            }
            else if (Network == null && other.Network == null)
            {
                Network = new Network();
                Network.Add(this);
                Network.Add(other);
            }
        }

        public void DestroyAllConnections()
        {
            var ConnectionsToDestroy = new List<Connection>();
            ConnectionsToDestroy.AddRange(Connections);

            foreach (var connection in ConnectionsToDestroy) connection.Destroy();
        } 

        public virtual void OnConnectionDestroyed() { }

        public void CreateNetwork()
        {
            Network = new Network();
            Network.Add(this);
        }

        public void Disconnect()
        {
            if (Connections.Count > 0) DestroyAllConnections();
            Network.Remove(this);
        }

        public bool IsConnectedTo(Node other)
        {
            if (Connections.Exists(x => x.GetOther(this) == other)) return true;
            return false;
        } 

        /// <summary>
        /// Finds nodes that inherit from or are the type specified
        /// </summary> 
        public List<Node> FindNearbyNodesByType(Type type, int range)
        {
            List<Location> nearbyLocations = GameManager.Instance.GameWorld.worldGrid.GetSquareRadius(Parent.position, range);

            List<Structure> nearbyStructures = new();
            nearbyStructures.AddRange(from location in nearbyLocations
                                      where location.entity != null && location.entity.GetType().IsSubclassOf(typeof(Structure))
                                      select (Structure)location.entity);

            List<Node> nearbyNodes = new();
            nearbyNodes.AddRange(from structure in nearbyStructures
                                 where structure.ElectricalNode != null && (structure.ElectricalNode.GetType() == type || structure.ElectricalNode.GetType().IsSubclassOf(type))
                                 select structure.ElectricalNode);

            if (nearbyNodes.Count == 0) return null;
            else return nearbyNodes;
        }

        public int SortNodeByDistanceToSelf(Node a, Node b)
        {
            var aDistance = GetRoughDistance(a.Parent.position, this.Parent.position);
            var bDistance = GetRoughDistance(b.Parent.position, this.Parent.position);

            if (aDistance == bDistance) return 0;
            else if (aDistance < bDistance) return -1;
            else if (aDistance > bDistance) return 1;
            else return 0;
        }

        float GetRoughDistance(float2 a, float2 b)
        {
            float2 offset = a - b;
            return (offset.x * offset.x) + (offset.y * offset.y);
        }

        public int SortNodesByNetworkCapacity(Node a, Node b)
        {
            if (a.Network == null) { return -1; }
            else if (b.Network == null) { return +1; }
            else if (a.Network.MaxInput == b.Network.MaxInput) return 0;
            else if (a.Network.MaxInput < b.Network.MaxInput) return -1;
            else if (a.Network.MaxInput > b.Network.MaxInput) return 1;
            else return 0;
        }
    }

    public class Connection
    {
        public static readonly List<Connection> pool = new();

        public Node a;
        public Node b;

        public Vector3 origin;
        public Quaternion rotation;
        public Vector3 scale;

        // public object LineBetweenA&B

        public Connection(Node a, Node b)
        {
            this.a = a;
            this.b = b;
            pool.Add(this);

            var aConnectionPoint = a.Parent.DisplayObject.GetWireConnectionPoint();
            var bConnectionPoint = b.Parent.DisplayObject.GetWireConnectionPoint();

            origin = aConnectionPoint;

            var difference = aConnectionPoint - bConnectionPoint;

            rotation = Quaternion.LookRotation(difference, Vector3.up);

            scale = new Vector3(1, 1, 1 * difference.magnitude);
        }

        public void Init()
        {
            // Create new LineBetweenA&B (a.entity.location)
        }

        public void Destroy()
        {
            // Destory LineBetweenA&B
            a.Connections.Remove(this);
            b.Connections.Remove(this);

            pool.Remove(this);
        }

        public Node GetOther(Node self)
        {
            if (self == null) return null;
            if (self == a) return b;
            if (self == b) return a;
            else return null;
        }
    }

    public class Relay : Node
    {
        public static int connectionRange = 4;
        public static int MaxRelayConnections = 4;

        public override void OnConstructed()
        {
            base.OnConstructed();
            SystemManager.relays.Add(this); 
            TryConnect();
        }

        public override void OnDemolished()
        {
            base.OnDemolished();
            SystemManager.relays.Remove(this);
        }

        public override void OnConnectionDestroyed()
        {
            TryConnect();
        } 

        public void TryConnect()
        {
            ConnectToNearbyRelays();
            TryConnectToComponents();
        }

        public void ConnectToNearbyRelays()
        {
            List<Node> nearbyRelays = FindNearbyNodesByType(typeof(Relay), connectionRange);
            if (nearbyRelays == null || nearbyRelays.Count == 0) return;

            nearbyRelays.Sort(SortNodeByDistanceToSelf);
            nearbyRelays.Remove(this);  

            for (int i = 0; i < nearbyRelays.Count; i++)
            { 
                TryConnectCleanly(nearbyRelays[i], MaxRelayConnections);
            } 
        }

        public void TryConnectToComponents()
        {
            List<Node> nearbyComponents = FindNearbyNodesByType(typeof(Component), connectionRange);
            if (nearbyComponents == null || nearbyComponents.Count == 0) return;

            List<Component> invalidComponents = new();

            foreach (Component component in nearbyComponents) 
                if(component.Connections.Count > 0) 
                    invalidComponents.Add(component);

            foreach (Component component in invalidComponents)
                nearbyComponents.Remove(component);

            foreach (Component component in nearbyComponents)
                CreateConnectionTo(component);
        }
    }

    public class Component : Node
    {
        public int connectionRange = 6;

        public override void OnConstructed()
        {
            ConnectToNearbyRelays();
        }

        public override void OnDemolished()
        {
            DestroyAllConnections();
        }

        public override void OnConnectionDestroyed()
        {
            Disconnect();
        }

        public void ConnectToNearbyRelays()
        {
            if (Connections.Count > 0) return;

            List<Node> nearbyRelays = FindNearbyNodesByType(typeof(Relay), connectionRange);   
            if (nearbyRelays == null || nearbyRelays.Count == 0) return;

            nearbyRelays.Sort(SortNodesByNetworkCapacity);  
            var bestNetwork = nearbyRelays[0].Network;
            List<Node> bestRelays = new();
            // Remove all relays that are not part of the best network.
            foreach(var relay in nearbyRelays)
            {
                if(relay.Network == null) continue;
                if(relay.Network == bestNetwork) bestRelays.Add(relay); 
            } 
            bestRelays.Sort(SortNodeByDistanceToSelf);

            if (bestRelays.Count == 0) return; //? Unsure why this is needed? Bestrelays returns as empty when only one relay is nearby

            CreateConnectionTo(bestRelays[0]);
        }
    }

    public class Input : Component
    {
        public float MaxProduction = 10f;
        public float Production = 1f;
    }

    public class Sink : Component
    {
        public float MaxConsumption = 10f;
        public float Consumption = 10f;
    }
     
}