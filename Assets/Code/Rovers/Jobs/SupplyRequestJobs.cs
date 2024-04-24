using Logistics;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace RoverJobs
{
    public class CollectAndDeliverAsManyAsPossible : Job
    {
        ResourceQuantity resourceToFind;
        int2 destination;

        public CollectAndDeliverAsManyAsPossible(ResourceQuantity resourceToFind, int2 destination)
        {
            this.resourceToFind = resourceToFind;
            this.destination = destination;
        }

        public override void OnStart()
        {
            var manifest = SupplyFinder.GenerateManifestLoose(resourceToFind, rover.GridPosition);
            if (manifest.Orders.Count == 0) { FailTask(); return; }
            PopJob();
            StackJob(new ExcecuteCollectAndDeliver(manifest, destination));
        }
    }


    public class CollectDeliverExactly : Job
    {
        IEnumerable<ResourceQuantity> resourcesToFind;
        int2 destination;

        public CollectDeliverExactly(IEnumerable<ResourceQuantity> resourcesToFind, int2 destination)
        {
            this.resourcesToFind = resourcesToFind;
            this.destination = destination;
        }

        public override void OnStart()
        {
            var manifest = SupplyFinder.GenerateManifestExact(resourcesToFind, rover.GridPosition);
            Debug.Log("Started Collect & Deliver Exactly");
            if (manifest.Orders.Count == 0) { FailTask(); return; }
            PopJob();
            StackJob(new ExcecuteCollectAndDeliver(manifest, destination));
        }
    }


    public class ExcecuteCollectAndDeliver : Job
    {
        private LinkedList<Path> superPath;
        private CollectionManifest manifest;
        private List<SupplyPort> remainingPorts;
        private int2 destination;

        public ExcecuteCollectAndDeliver(CollectionManifest manifest, int2 destination)
        {
            this.manifest = manifest;
            this.remainingPorts = manifest.SupplyPorts;
            this.destination = destination;
        }

        public override void OnStart()
        {
            base.OnStart();

            superPath = PathFinder.FindSuperPath(rover.GridPosition, manifest.SupplyPorts.Select(p => p.parent.position).ToArray());
            if (superPath == null || superPath.Count == 0) { FailTask(); return; }
            superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(superPath.Last().nodes.Last(), destination));
            manifest.ReserveResources();
        }

        public override void OnTick()
        {
            if (superPath.Count > 0)
            {
                if (superPath.Count > 1)
                {
                    var resourcesToCollect = manifest.Orders[remainingPorts[0]];
                    StackJob(new CollectResources(remainingPorts[0], resourcesToCollect));
                    remainingPorts.RemoveAt(0);
                }

                StackJob(new TraversePath(superPath.First()));
                superPath.RemoveFirst();
            }
            else
            {
                PopJob();
                StackJob(new DeliverResources(destination, manifest.totalQuantities));
                return;
            }
        }
    }


    public class CollectionManifest
    {
        public List<SupplyPort> SupplyPorts = new();
        public List<ResourceQuantity> totalQuantities = new();
        public Dictionary<SupplyPort, List<ResourceQuantity>> Orders = new();

        public void Add(SupplyPort supplyPort, ResourceQuantity resourceQuantity)
        {
            if (SupplyPorts.Contains(supplyPort))
            {
                // I hate this doubling of code but I cant seem to find a better way with Unity c# :(
                if (Orders[supplyPort].Exists(rq => rq.resource == resourceQuantity.resource))
                {
                    var existingEntry = Orders[supplyPort].Find(rq => rq.resource == resourceQuantity.resource);
                    existingEntry.quantity += resourceQuantity.quantity;
                }
                else Orders[supplyPort].Add(resourceQuantity);
            }
            else
            {
                SupplyPorts.Add(supplyPort);
                Orders.Add(supplyPort, new() { resourceQuantity });
            }

            if (totalQuantities.Exists(rq => rq.resource == resourceQuantity.resource))
            {
                var existingEntry = totalQuantities.Find(rq => rq.resource == resourceQuantity.resource);
                existingEntry.quantity += resourceQuantity.quantity;
            }
            else
            {
                totalQuantities.Add(resourceQuantity);
            }
        }

        public void SortPortsByDistance(int2 origin)
        {
            SupplyPorts = SupplyFinder.SortPortsByDistance(SupplyPorts, origin);
        }

        public void ReserveResources()
        {
            foreach (var order in Orders)
            {
                SupplyPort supplyPort = order.Key;
                List<ResourceQuantity> resources = order.Value;

                foreach (ResourceQuantity rq in resources)
                {
                    supplyPort.ReserveResource(rq);
                }
            }
        } 
    }


    public static class SupplyFinder
    { 
        public static CollectionManifest GenerateManifestExact(IEnumerable<ResourceQuantity> resourcesToFind, int2 origin)
        {
            List<SupplyPort> supplyPorts = FindAllPortsContaining(resourcesToFind);
            supplyPorts = SortPortsByDistance(supplyPorts, origin);

            CollectionManifest manifest = new CollectionManifest(); 
            foreach (ResourceQuantity rq in resourcesToFind)
            {
                SearchPortsAndFillManifest(supplyPorts, manifest, rq); 
            } 
            manifest.SortPortsByDistance(origin);
            return manifest;
        }  

        public static CollectionManifest GenerateManifestLoose(ResourceQuantity resourceToFind, int2 origin)
        { 
            List<SupplyPort> supplyPorts = FindAllPortsContaining(resourceToFind);
            supplyPorts = SortPortsByDistance(supplyPorts, origin);

            CollectionManifest manifest = new CollectionManifest();
            SearchPortsAndFillManifest(supplyPorts, manifest, resourceToFind);
            manifest.SortPortsByDistance(origin);
            return manifest;
        }

        static void SearchPortsAndFillManifest(List<SupplyPort> supplyPortsToSearch, CollectionManifest manifestToFill, ResourceQuantity resourceQuantity)
        {
            int remaining = resourceQuantity.quantity;
            foreach (SupplyPort supplyPort in supplyPortsToSearch)
            {
                int quantityAvailable = supplyPort.GetUnreservedQuantity(resourceQuantity.resource);
                if (quantityAvailable <= 0) continue;

                var newRQ = new ResourceQuantity(resourceQuantity.resource, Mathf.Clamp(quantityAvailable, 0, remaining));
                manifestToFill.Add(supplyPort, newRQ);

                remaining -= quantityAvailable;
                if (remaining <= 0) { break; }
            }
        } 

        static List<SupplyPort> FindAllPortsContaining(IEnumerable<ResourceQuantity> resourcesToFind)
        {
            List<SupplyPort> foundPorts = new();

            foreach (SupplyPort supplyPort in SupplyPort.Pool)
                foreach (ResourceQuantity rq in resourcesToFind)
                {
                    if (supplyPort.GetUnreservedQuantity(rq.resource) <= 0) continue;
                    if (!foundPorts.Contains(supplyPort)) foundPorts.Add(supplyPort);
                }
            return foundPorts;
        }

        static List<SupplyPort> FindAllPortsContaining(ResourceQuantity resourceToFind)
        {
            List<SupplyPort> foundPorts = new();

            foreach (SupplyPort supplyPort in SupplyPort.Pool)
            {
                if (supplyPort.GetUnreservedQuantity(resourceToFind.resource) <= 0) continue;
                foundPorts.Add(supplyPort);
            }
            return foundPorts;
        }  

/*        public static void ReserveResources(CollectionManifest manifest)
        {
            foreach (var order in manifest.Orders)
            {
                SupplyPort supplyPort = order.Key;
                List<ResourceQuantity> resources = order.Value;

                foreach(ResourceQuantity rq in resources)
                {
                    supplyPort.ReserveResource(rq);
                } 
            }
        }*/

        public static List<SupplyPort> SortPortsByDistance(List<SupplyPort> supplyPorts, int2 origin)
        {
            var ports = new List<SupplyPort>();
            ports.AddRange(supplyPorts);
            ports.Sort(SortDistanceToOrigin);

            return ports;

            int SortDistanceToOrigin(SupplyPort a, SupplyPort b)
            {
                return FloatSort(DistanceBetween(origin, a.parent.position), DistanceBetween(origin, b.parent.position));
            }

            static int FloatSort(float a, float b)
            {
                if (a == b) return 0;
                else if (a < b) return -1;
                else if (a > b) return 1;
                else return 0;
            }

            static float DistanceBetween(float2 a, float2 b)
            {
                float2 offset = a - b;
                return (offset.x * offset.x) + (offset.y * offset.y);
            }
        }
    } 


    public class FoundHopper
    {
        public Hopper Hopper;
        public List<ResourceQuantity> reservedResources = new();
        public float DistanceToRover;
        public float DistanceToDestination;

        public FoundHopper(Hopper hopper, float distanceToRover, float distanceToDestination)
        {
            Hopper = hopper;
            DistanceToRover = distanceToRover;
            DistanceToDestination = distanceToDestination;
        }
    }

    public class OLD_CollectAndDeliverOnlyIfAvailable : Job
    {
        private List<ResourceQuantity> _resourcesToCollect;
        private int2 destination;

        private List<FoundHopper> foundHoppers = new List<FoundHopper>();

        private LinkedList<Path> _superPath = new();

        bool finished = false;

        public OLD_CollectAndDeliverOnlyIfAvailable(IEnumerable<ResourceQuantity> resourceQuantities, int2 destination)
        {
            this._resourcesToCollect = resourceQuantities.ToList();
            this.destination = destination;
        }

        public override void OnStart()
        {
            var success = TryFindHoppers();

            if (!success) { FailTask(); return; }

            _superPath = PathFinder.FindSuperPath((int2)rover.VisualPosition, foundHoppers.Select(foundHopper => foundHopper.Hopper.position).ToArray());
            if (_superPath == null || _superPath.Count == 0) { FailTask(); return; }
            _superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(_superPath.Last().nodes.Last(), destination));

            PopJob();
            //StackJob(new ExcecuteCollectAndDeliver(_superPath, destination));
        }

        private bool TryFindHoppers()
        {
            Dictionary<ResourceData, int> remaingResourcesToFind = new();
            foundHoppers = new();

            foreach (var rq in _resourcesToCollect)
                remaingResourcesToFind.Add(rq.resource, rq.quantity);

            List<Hopper> hoppers = Hopper.pool;

            hoppers.Sort(HopperSortDistToDest);

            foreach (ResourceQuantity rq in _resourcesToCollect)
                foreach (Hopper hopper in Hopper.pool)
                {
                    var quantityInHopper = hopper.storageInventory.GetQuantityOf(rq.resource);

                    if (quantityInHopper > 0)
                    {
                        if (PathFinder.FindPathToAnyFreeNeighbor((int2)rover.VisualPosition, hopper.position) == null) { continue; } // Hopper not reachable

                        var foundHopper = foundHoppers.Find(foundHoppers => foundHoppers.Hopper == hopper);

                        if (foundHopper == null)
                        {
                            foundHopper = new FoundHopper(hopper, DistanceBetween(rover.VisualPosition, hopper.position), DistanceBetween(destination, hopper.position));
                            foundHoppers.Add(foundHopper);
                        }

                        var maxReservableQuantity = Mathf.Clamp(quantityInHopper, 0, remaingResourcesToFind[rq.resource]);
                        foundHopper.reservedResources.Add(new ResourceQuantity(rq.resource, maxReservableQuantity));

                        remaingResourcesToFind[rq.resource] -= quantityInHopper;
                        if (remaingResourcesToFind[rq.resource] <= 0) { remaingResourcesToFind.Remove(rq.resource); break; }
                    }
                }

            if (remaingResourcesToFind.Count > 0) { return false; }

            foundHoppers.Sort(FHopperSortDistToDestination);

            foreach (var foundHopper in foundHoppers)
                foreach (var reservedResource in foundHopper.reservedResources)
                    foundHopper.Hopper.storageInventory.ReserveResource(reservedResource);

            return true;

            int HopperSortDistToDest(Hopper a, Hopper b)
            {
                return FloatSort(DistanceBetween(destination, a.position), DistanceBetween(destination, b.position));
            }

            int FHopperSortDistToDestination(FoundHopper a, FoundHopper b)
            {
                return FloatSort(a.DistanceToDestination, b.DistanceToDestination);
            }

            int FloatSort(float a, float b)
            {
                if (a == b) return 0;
                else if (a < b) return -1;
                else if (a > b) return 1;
                else return 0;
            }

            float DistanceBetween(float2 a, float2 b)
            {
                float2 offset = a - b;
                return (offset.x * offset.x) + (offset.y * offset.y);
            }
        }
    }

    public class OLD_CollectAndDeliverAnyAvailable
    {
        ResourceQuantity resourceQuantity;
        private int2 destination;
        private List<FoundHopper> foundHoppers = new List<FoundHopper>();
        private LinkedList<Path> _superPath = new();

        bool finished = false;

        public OLD_CollectAndDeliverAnyAvailable(ResourceQuantity resourceQuantity, int2 destination)
        {
            this.resourceQuantity = resourceQuantity;
            this.destination = destination;
        }
    }

}