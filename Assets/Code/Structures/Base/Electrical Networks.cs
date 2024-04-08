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
    }

    public class Network
    {
        public List<Node> nodes;
        public List<Relay> relays;
        public List<Input> inputs;
        public List<Sink> sinks;

        public float MaxInput = 0f;
        public float TotalInput = 0f;
        public float MaxConsumption = 0f;
        public float TotalConsumption = 0f;

        float PowerRatio
        {
            get { return TotalInput / TotalConsumption; }
        }

        public void Update()
        {

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

        }
    }

    public class Node
    {
        public Structure Parent;
        public Network Network;
        public List<Connection> Connections = new();

        public void Constructed()
        { 
            OnConstructed();
        }
        public void Demolished()
        {
            OnDemolished();

            Parent.OnDemolishedEvent -= Demolished;
        }
        public virtual void OnConstructed() { }
        public virtual void OnDemolished() { }

        public void CreateConnection(Node other)
        {
            var newConnection = new Connection(this, other);
            Connections.Add(newConnection);
            other.Connections.Add(newConnection);

            //return;

            // If the other node is the only node in its network absorb it into this one.
            if (Network != null && other.Network.nodes.Count == 1)
            {
                other.Network.Destroy();
                Network.Add(other);
            }
            // If this node is the only in its network, do the opposite.
            else if (Network != null && Network.nodes.Count == 1)
            {
                Network.Destroy();
                other.Network.Add(this);
            }
            // If both nodes have networks join both the networks together with a merge call.
            else if (Network != null && other.Network != null)
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
        }

        public void DestroyAllConnections()
        {
            foreach (var connection in Connections) connection.Destroy();
        }

        public void ConnectionDestroyed(Connection connection)
        {
            Connections.Remove(connection);
            OnConnectionDestroyed();
        }

        public virtual void OnConnectionDestroyed() { }

        public void CreateNetwork()
        {
            Network = new Network();
            Network.Add(this);
        }

        public void Disconnect()
        {
            if(Connections.Count > 0) DestroyAllConnections(); 
            Network.Remove(this);
        }

        public bool IsConnectedTo(Node other)
        {
            if(Connections.Exists(x => x.GetOther(this) == other)) return true;
            return false;
        }

        public List<Node> FindNearbyOfType(Type type, int range)
        {
            List<Location> nearbyLocations = GameManager.Instance.GameWorld.worldGrid.GetSquareRadius(Parent.position, range); 

            List<Structure> nearbyStructures = new();   
            nearbyStructures.AddRange(from location in nearbyLocations
                                      where location.entity != null && location.entity.GetType().IsSubclassOf(typeof(Structure))
                                      select (Structure)location.entity); 

            List<Node> nearbyNodes = new();  
            nearbyNodes.AddRange(from structure in nearbyStructures
                                  where structure.ElectricalNode != null && structure.ElectricalNode.GetType() == type
                                  select structure.ElectricalNode); 

            if (nearbyNodes.Count == 0) return null;
            else return nearbyNodes;
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

            scale = new Vector3(1, 1, 1*difference.magnitude);
        }

        public void Init()
        {
            // Create new LineBetweenA&B (a.entity.location)
        }

        public void Destroy()
        {
            // Destory LineBetweenA&B
            a.ConnectionDestroyed(this);
            b.ConnectionDestroyed(this);

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
        public int connectionRange = 6;

        public override void OnConstructed()
        {
            ConnectToClosestRelay();
        }

        public override void OnConnectionDestroyed()
        {
            ConnectToClosestRelay();
        }

        public void ConnectToClosestRelay()
        { 
            List<Relay> nearbyRelays = FindNearbyOfType(typeof(Relay), connectionRange).Select(x => (Relay)x).ToList();

            nearbyRelays.Sort(SortRelayByDistance); 
            nearbyRelays.Remove(this);

            int SortRelayByDistance(Relay a, Relay b)
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

            if (nearbyRelays.Count == 0) { return; }
            if(IsConnectedTo(nearbyRelays[0]) == false)
            {
                CreateConnection(nearbyRelays[0]);
            }
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
            List<Location> nearbyLocations = GameManager.Instance.GameWorld.worldGrid.GetSquareRadius(Parent.position, connectionRange);
            //Debug.Log($"nearbyLocations {nearbyLocations.Count}");

            List<Structure> nearbyStructures = new();   // = nearbyLocations.FindAll(x => x.entity != null && x.entity.GetType() == typeof(Structure)).Select(x => (Structure)x.entity).ToList();
            nearbyStructures.AddRange(from location in nearbyLocations
                                      where location.entity != null && location.entity.GetType().IsSubclassOf(typeof(Structure))
                                      select (Structure)location.entity);
            //Debug.Log($"nearbyStructures {nearbyStructures.Count}");

            List<Relay> nearbyRelays = new();    //nearbyStructures.FindAll(x => x.ElectricalNode.GetType() == typeof(Relay)).Select(x => (Relay)x.ElectricalNode).ToList();
            nearbyRelays.AddRange(from structure in nearbyStructures
                                  where structure.ElectricalNode != null && structure.ElectricalNode.GetType() == typeof(Relay)
                                  select (Relay)structure.ElectricalNode);
            //Debug.Log($"nearbyRelays {nearbyRelays.Count}");

            if (nearbyRelays.Count == 0) return;

            nearbyRelays.Sort(SortRelayByNetworkCapacity);

            int SortRelayByNetworkCapacity(Relay a, Relay b)
            {
                if (a.Network == null) { return -1; }
                else if (b.Network == null) { return +1; }
                else if (a.Network.MaxInput == b.Network.MaxInput) return 0;
                else if (a.Network.MaxInput < b.Network.MaxInput) return -1;
                else if (a.Network.MaxInput > b.Network.MaxInput) return 1;
                else return 0;
            }

            if (nearbyRelays.Count == 0) { return; }
            if (IsConnectedTo(nearbyRelays[0]) == false)
            {
                CreateConnection(nearbyRelays[0]);
            }
        }
    }

    public class Input : Component
    {
        public float production = 0f;
    }

    public class Sink : Component
    {
        public float consumption = 0f;
    }
}