using ExtensionMethods;
using Logistics;
using System;
using System.Collections;
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
        CollectionManifest manifest;

        public CollectAndDeliverAsManyAsPossible(ResourceQuantity resourceToFind, int2 destination)
        {
            this.resourceToFind = resourceToFind;
            this.destination = destination;
        }

        public override void OnStart()
        {
            resourceToFind = SupplyFinder.CullSearchByExistingInventory(resourceToFind, rover.Inventory);
            // If the rover doesnt already have all the resources go find them.
            if(resourceToFind.quantity > 0)
            {
                manifest = SupplyFinder.GenerateManifestLoose(resourceToFind, rover.GridPosition, destination);
                if (manifest.Orders.Count == 0 && rover.Inventory.GetQuantityOf(resourceToFind.resource) == 0) { FailTask(); return; }
                if(manifest.Orders.Count > 0)
                {
                    PopJob();

                    rover.collectionManifest = manifest;
                    var success = manifest.ReserveResources();
                    if (success == false) { FailTask(); return; }

                    StackJob(new ExcecuteCollectAndDeliver(manifest, destination));
                    return;
                } 
            }
            // If the rover has the resources already then go deliver them 

            StackJob(new GotoNeighbor(destination)); 
            var quantityToDeliver = Mathf.Clamp(rover.Inventory.GetQuantityOf(resourceToFind.resource), 0, resourceToFind.quantity);
            var resourcesToDeliver = new List<ResourceQuantity>() { new(resourceToFind.resource, quantityToDeliver) };
            StackJob(new DeliverResources(destination, resourcesToDeliver));
        }
    }


    public class CollectDeliverExactly : Job
    {
        List<ResourceQuantity> resourcesToFind;
        int2 destination;

        public CollectDeliverExactly(IEnumerable<ResourceQuantity> resourcesToFind, int2 destination)
        {
            this.resourcesToFind = resourcesToFind.ToList();
            this.destination = destination;
        }

        public override void OnStart()
        { 
            resourcesToFind = SupplyFinder.CullSearchByExistingInventory(resourcesToFind, rover.Inventory);

            var manifest = SupplyFinder.GenerateManifestExact(resourcesToFind, rover.GridPosition, destination);

            if (manifest.Orders.Count == 0) { FailTask(); return; }

/*            var rtfList = resourcesToFind;
            foreach (var rq in manifest.totalQuantities)
                if (rtfList.Exists(x => x.resource == rq.resource && x.quantity == rq.quantity) == false) { FailTask(); return; }
*/
            if(manifest.VerifyManifest(resourcesToFind) == false ) { FailTask(); return; }  


            rover.collectionManifest = manifest;
            var success = manifest.ReserveResources();
            if(success == false) { FailTask(); return; }    

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

            if (manifest.SupplyPorts.Count == 0) { FailTask(); return; }

            superPath = PathFinder.FindSuperPath(rover, rover.GridPosition, manifest.SupplyPorts.Select(p => p.parent.position).ToArray());
            if (superPath == null || superPath.Count == 0) { FailTask(); return; }
            superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(rover, superPath.Last().nodes.Last(), destination));

            //manifest.ReserveResources();
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


    public class CollectResources : Job
    {
        private SupplyPort targetPort;
        private List<ResourceQuantity> resourcesToCollect;

        public CollectResources(SupplyPort target, List<ResourceQuantity> resourcesToCollect)
        {
            this.targetPort = target;
            this.resourcesToCollect = resourcesToCollect;
        }

        public override void OnStart()
        { 
            StackJob(new TurnTowards(targetPort.parent.position));
        }

        public override void OnTick()
        {
            if (lifeSpan == 1)
            {
                rover.DisplayObject.CrossfadeAnimation("CollectResource", 0.5f);
                rover.PlaySound(AudioData.Instance.Rover_CollectResource, 1f);
            }

            if (rover.DisplayObject.IsAnimationPlaying("CollectResource") == false && lifeSpan > 50)
            {
                foreach (var rq in resourcesToCollect)
                {
                    if (rover.Inventory.TryAddResource(rq) == false) { throw new Exception("Rover attempted to exceed inventory capacity"); }
                    targetPort.CollectResource(rq);
                }

                PopJob();
            }
        }
    }


    public class DeliverResources : Job
    {
        private StructureGhost targetGhost;
        private RequestPort targetPort;
        private IRecieveResources targetRequestor;
        private int2 target;
        private List<ResourceQuantity> resourcesToDeliver;

        public DeliverResources(int2 target, List<ResourceQuantity> resourcesToDeliver)
        {
            this.target = target;
            this.resourcesToDeliver = resourcesToDeliver;
        }

        public override void OnStart()
        {
            // Shouldnt need to do this but rovers are ocassionally arriving without the resources they need. No clue why. Not enough time to fix it.
            foreach (var resource in resourcesToDeliver)
            {
                if (rover.Inventory.GetQuantityOf(resource.resource) < resource.quantity)
                {
                    FailTask(); return;
                }
            }

            var entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(target);

            if (entity == null) { FailTask(); return; }

            if (entity.GetType() == typeof(StructureGhost))
            {
                targetGhost = (StructureGhost)entity;
            }
            else if (entity.GetType().IsSubclassOf(typeof(Structure)) && ((Structure)entity).RequestPort != null)
            {
                targetPort = ((Structure)entity).RequestPort;
            }
            else if (entity.GetType().GetInterfaces().Contains(typeof(IRecieveResources)))
            {
                targetRequestor = (IRecieveResources)entity;
            }

            StackJob(new TurnTowards(target));
        }

        public override void OnTick()
        {
            if (lifeSpan == 1)
            {
                rover.DisplayObject.CrossfadeAnimation("DeliverResource", 0.5f);
                rover.PlaySound(AudioData.Instance.Rover_DeliverResource, 1f);
            }

            // Play delivery animation on rover
            // Then run this ->

            if (rover.DisplayObject.IsAnimationPlaying("DeliverResource") == false && lifeSpan > 50)
            {
                if (targetPort != null)
                {
                    foreach (var rq in resourcesToDeliver)
                    {
                        targetPort.SupplyResources(rq);
                    }
                }
                if (targetGhost != null)
                {
                    foreach (var rq in resourcesToDeliver)
                    {
                        targetGhost.SupplyResources(rq);
                    }
                }
                if (targetRequestor != null)
                {
                    foreach (var rq in resourcesToDeliver)
                    {
                        targetRequestor.RecieveResources(rq.resource, rq.quantity);
                    }
                }

                foreach (var rq in resourcesToDeliver)
                {
                    rover.Inventory.RemoveResource(rq);
                }

                PopJob();
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
                if (Orders[supplyPort].Exists(rq => rq.resource == resourceQuantity.resource))
                {
                    // I hate this doubling of code but Unity c# doesn't have TryFind with out :(
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
                // I hate this doubling of code but Unity c# doesn't have TryFind with out :(
                var entryIndex = totalQuantities.FindIndex(rq => rq.resource == resourceQuantity.resource);
                totalQuantities[entryIndex] = new ResourceQuantity(totalQuantities[entryIndex].resource, totalQuantities[entryIndex].quantity + resourceQuantity.quantity);
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

        public bool ReserveResources()
        {
            foreach (var order in Orders)
            {
                SupplyPort supplyPort = order.Key;
                List<ResourceQuantity> resources = order.Value;

                foreach (ResourceQuantity rq in resources)
                {
                    var remainder = supplyPort.ReserveResource(rq);
                    if (remainder > 0) return false;
                }
            }

            return true;
        }

        public void UnreserveResources()
        {
            foreach (var order in Orders)
            {
                SupplyPort supplyPort = order.Key;
                List<ResourceQuantity> resources = order.Value;

                foreach (ResourceQuantity rq in resources)
                {
                    supplyPort.FreeResource(rq);
                }
            }
        }

        public bool VerifyManifest(List<ResourceQuantity> resourceQuantities)
        {
            foreach(var r in resourceQuantities)
            { 
                if (totalQuantities.Contains(r)) continue;
                return false;
            }
            return true;
        }
    }


    public static class SupplyFinder
    {
        public static CollectionManifest GenerateManifestExact(IEnumerable<ResourceQuantity> resourcesToFind, int2 roverPosition, int2 destinationPosition)
        {
            List<SupplyPort> supplyPorts = FindAllPortsContaining(resourcesToFind);
            supplyPorts = SortPortsByDistance(supplyPorts, roverPosition);
            var self = supplyPorts.Find(x => x.parent.position.Equals(destinationPosition));
            if(self != null) { supplyPorts.Remove(self); }

            CollectionManifest manifest = new CollectionManifest();
            foreach (ResourceQuantity rq in resourcesToFind) SearchPortsAndFillManifest(supplyPorts, manifest, rq); 
            manifest.SortPortsByDistance(roverPosition);

            foreach (var val in manifest.totalQuantities)
            {
                //Debug.Log($"{val.resource} : {val.quantity}");
            }
            return manifest;
        }

        public static CollectionManifest GenerateManifestLoose(ResourceQuantity resourceToFind, int2 origin, int2 destinationPosition)
        {
            // Little bit of code duplication here. Cant be bothered to fix this rn.
            List<SupplyPort> supplyPorts = FindAllPortsContaining(resourceToFind);
            supplyPorts = SortPortsByDistance(supplyPorts, origin);
            var self = supplyPorts.Find(x => x.parent.position.Equals(destinationPosition));
            if (self != null) { supplyPorts.Remove(self); }

            CollectionManifest manifest = new CollectionManifest();
            SearchPortsAndFillManifest(supplyPorts, manifest, resourceToFind);
            manifest.SortPortsByDistance(origin);
            return manifest;
        }


        public static ResourceQuantity CullSearchByExistingInventory(ResourceQuantity resourceToFind, Inventory inventory)
        {
            ResourceQuantity newSearch = resourceToFind; 

            if(inventory.stacks.Exists(stack => stack.resource == newSearch.resource))
            {
                var stack = inventory.GetStack(newSearch.resource);
                var newQuanity = newSearch.quantity - stack.quantity;
                if (newQuanity > 0) newSearch = new(newSearch.resource, newQuanity);
                else newSearch = new(newSearch.resource, 0); 
            } 
            return newSearch;
        }
        public static List<ResourceQuantity> CullSearchByExistingInventory(List<ResourceQuantity> resourcesToFind, Inventory inventory)
        {
            List<ResourceQuantity> newList = new();
            newList.AddRange(resourcesToFind);
            foreach (var stack in inventory.stacks)
            {
                if (resourcesToFind.Exists(rq => rq.resource == stack.resource))
                {
                    var rqindex = newList.FindIndex(rq => rq.resource == stack.resource);
                    var newQuanity = newList[rqindex].quantity - stack.quantity;
                    if (newQuanity > 0) newList[rqindex] = new(newList[rqindex].resource, newQuanity);
                    else newList.RemoveAt(rqindex);
                }
            }
            return newList;
        }

        static void SearchPortsAndFillManifest(List<SupplyPort> supplyPortsToSearch, CollectionManifest manifestToFill, ResourceQuantity resourceQuantity)
        {
            int remaining = resourceQuantity.quantity;
            //Debug.Log($"Looking for {remaining} {resourceQuantity.resource}s");
            foreach (SupplyPort supplyPort in supplyPortsToSearch)
            {
                int quantityAvailable = supplyPort.GetUnreservedQuantity(resourceQuantity.resource);
                if (quantityAvailable <= 0) continue;
                var quantityTakeable = Mathf.Clamp(remaining, 0, quantityAvailable);
                if(quantityTakeable == 0) continue;

                var newRQ = new ResourceQuantity(resourceQuantity.resource, quantityTakeable);
                manifestToFill.Add(supplyPort, newRQ);

                remaining -= quantityTakeable;
                if (remaining <= 0) { break; }
            }
            //Debug.Log($"Found {resourceQuantity.quantity - remaining}");
        }

        static List<SupplyPort> FindAllPortsContaining(IEnumerable<ResourceQuantity> resourcesToFind)
        {
            List<SupplyPort> foundPorts = new();

            foreach (SupplyPort supplyPort in SupplyPort.Pool)
            {
                if (supplyPort.enabled == false) { continue; }
                foreach (ResourceQuantity rq in resourcesToFind)
                {
                    if (supplyPort.enabled == false) { continue; }
                    if (supplyPort.GetUnreservedQuantity(rq.resource) <= 0) continue;
                    if (!foundPorts.Contains(supplyPort)) foundPorts.Add(supplyPort);
                }
            }
            return foundPorts;
        }

        static List<SupplyPort> FindAllPortsContaining(ResourceQuantity resourceToFind)
        {
            List<SupplyPort> foundPorts = new();

            foreach (SupplyPort supplyPort in SupplyPort.Pool)
            {
                if (supplyPort.enabled == false) { continue; }
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
                return FloatSort(Float2Extensions.DistanceBetween(origin, a.parent.position), Float2Extensions.DistanceBetween(origin, b.parent.position));
            }

            static int FloatSort(float a, float b)
            {
                if (a == b) return 0;
                else if (a < b) return -1;
                else if (a > b) return 1;
                else return 0;
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

            _superPath = PathFinder.FindSuperPath(rover, (int2)rover.VisualPosition, foundHoppers.Select(foundHopper => foundHopper.Hopper.position).ToArray());
            if (_superPath == null || _superPath.Count == 0) { FailTask(); return; }
            _superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(rover, _superPath.Last().nodes.Last(), destination));

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
                        if (PathFinder.FindPathToAnyFreeNeighbor(rover, (int2)rover.GridPosition, hopper.position) == null) { continue; } // Hopper not reachable

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