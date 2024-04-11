    using Logistics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics; 
using UnityEngine;
using RoverTasks;
using static UnityEngine.GraphicsBuffer;
using static RoverJobs.CollectAndDeliverShoppingList;

namespace RoverJobs
{
    public class Job
    {
        public Rover rover;

        public int lifeSpan = -1;

        public bool isComplete = false;

        public Job() { }

        public void Tick()
        {
            lifeSpan++;
            OnTick();
        }

        public void Start()
        {
            OnStart();
        }

        public virtual void OnStart() { }

        public virtual void OnTick() { }

        public void StackJob(Job job) => rover.StackJob(job);
        public void PopJob() => rover.PopJob();

        public void EnqueueJob(Job job) => rover.EnqueueJob(job);

        public void FailTask()
        {
            rover.TaskFailed();
        }
    }


    public class FinishTask : Job
    {
        public override void OnStart()
        {
            rover.TaskFinished();
        }
    } 

    public class FetchTask : Job
    {
        public FetchTask() { }

        public override void OnStart()
        {
            if (rover == null) { throw new System.Exception("Feath task tried to tick with null rover"); }

            //Debug.Log("Tried to fetch a task");

            switch (rover.Module)
            {
                case RoverModule.None: break;

                case RoverModule.Construction: rover.ActiveTask = TaskManager.PopTask(TaskCategory.Construction); 
                    break;

                case RoverModule.Logistics: rover.ActiveTask = TaskManager.PopTask(TaskCategory.Logistics);
                    break;

                case RoverModule.Mining: rover.ActiveTask = TaskManager.PopTask(TaskCategory.Mining);
                    break;

                case RoverModule.Widget: rover.ActiveTask = TaskManager.PopTask(TaskCategory.All);
                    break;
            }

            if (rover.ActiveTask != null)
            {
                rover.ActiveTask.rover = rover; 
                rover.ActiveTask.BuildJobs();
                rover.ActiveTask.OnFetched?.Invoke();
            }

            PopJob();
        }
    }


    public class TraversePath : Job
    {
        private Path _path;

        private float2 _currentNodePosition;
        private float2 _nextNodePosition;

        private int _currentNodeIndex = -1; 

        private bool _mustFindNewPath = false;
        private bool _mustTurn = false;

        private bool _finished;

        public TraversePath(Path path)
        {
            this._path = path;
        }

        public override void OnStart()
        { 
            if (_path.length < 2) { rover.TaskFailed(); return; }

            rover.DisplayObject.PlayParticleEffect("MovingParticles");

            Structure.StructureConstructed += EvaluatePathInterruption; // Subscribe to structure build completion for evaluating path interuption.
        }


        public override void OnTick()
        {
            _mustTurn = false; 

            var lastNode = _path.nodes.Last();

            GameManager.Instance.AddLifespanGizmo(new Vector3(lastNode.x, 0.25f, lastNode.y), 100); 

            if (rover.VisualPosition.Equals(_nextNodePosition) || lifeSpan == 0)
            {
                AdvanceNode();

                if (_mustFindNewPath)
                {
                    FindNewPath();
                }
            }

            if (_finished || _mustTurn) return;

            rover.VisualPosition = Vector2.MoveTowards(rover.VisualPosition, _nextNodePosition, rover.MoveSpeed * Time.fixedDeltaTime); 
        } 

        private void AdvanceNode()
        {
            _currentNodeIndex++; 

            if (_currentNodeIndex + 1 == _path.nodes.Length) { Finished(); _finished = true; return; }

            rover.GridPosition = _path.nodes[_currentNodeIndex];

            _currentNodePosition = _path.nodes[_currentNodeIndex];
            _nextNodePosition = _path.nodes[_currentNodeIndex + 1]; 

            // Stack a TurnTowards job if the next node would cause the rover to turn
            Vector2 _difference = (Vector2)(rover.VisualPosition - _nextNodePosition);
            float _angle = Vector2.SignedAngle(_difference, Vector2.down); 

            if (! (Mathf.Approximately(_angle, rover.VisualRotation) || (Mathf.Approximately(Mathf.Abs(_angle), 180) && Mathf.Approximately(Mathf.Abs(rover.VisualRotation), 180))) )
            {
                StackJob(new TurnTowards(_nextNodePosition));
                _mustTurn = true;
            }
        }


        private void EvaluatePathInterruption(Structure structure)
        {
            if (_path.nodes.Contains(structure.position)) _mustFindNewPath = true;
            else _mustFindNewPath = false;
        }

        private void FindNewPath()
        {
            _path = PathFinder.FindPath(_path.nodes[_currentNodeIndex], _path.nodes.Last());
            _currentNodeIndex = -1;
            AdvanceNode();
            _mustFindNewPath = false;
        }

        private void Finished()
        {
            rover.GridPosition = _path.destination;

            PopJob();

            rover.DisplayObject.StopParticleEffect("MovingParticles");

            // Unsubscribe events
            Structure.StructureConstructed -= EvaluatePathInterruption;
        }
    } 

    public class GotoEntity : Job
    { 
        Entity Entity;

        Path path;  

        public GotoEntity(Entity entity)
        {
            this.Entity = entity;  
        }

        public override void OnStart()
        {
            if (Entity != null) path = PathFinder.FindPathToAnyFreeNeighbor(rover.GridPosition, Entity.position); 

            if (path == null) { FailTask(); return; }

            PopJob();

            StackJob(new TraversePath(path)); 
        } 
    } 

    public class TurnTowards : Job
    {
        private float2 target;

        //private float _baseRotation;
        private float _targetRotation;
        private float _currentRotation;

        public TurnTowards(int2 target)
        {
            this.target = (float2)target;
        }

        public TurnTowards(float2 target)
        {
            this.target = target;
        }

        public override void OnStart()
        {
            //_baseRotation = rover.rotation;
            _currentRotation = rover.VisualRotation;

            Vector2 difference = (Vector2)(rover.VisualPosition - target);
            _targetRotation = Vector2.SignedAngle(difference.normalized, Vector2.down);

            //Debug.Log($"Rotate Towards {target} at {_targetRotation} from {_currentRotation}");
        }

        public override void OnTick()
        {
            _currentRotation = Mathf.MoveTowardsAngle(_currentRotation, _targetRotation, rover.TurnSpeed);

            rover.VisualRotation = _currentRotation;

            rover.UpdateDoRotation();

            if (Mathf.Approximately(_currentRotation, _targetRotation))
            {
                PopJob();
            } 
        }
    }


    public class CollectResource : Job
    {
        private int2 _target;

        public CollectResource(int2 target)
        {
            _target = target;
        }

        public override void OnStart()
        {
            StackJob(new TurnTowards(_target));
        }

        public override void OnTick()
        {
            if (lifeSpan > 50)
            {
                PopJob();
            }
        }
    } 

    public class DeliverResource : Job
    {
        private int2 _target;

        public DeliverResource(int2 target)
        {
            _target = target;
        }

        public override void OnStart()
        {
            StackJob(new TurnTowards(_target));
        }

        public override void OnTick()
        {
            if (lifeSpan > 50)
            {
                PopJob();
            }
        }
    }


    public class BuildStructure : Job
    {
        StructureGhost structureGhost;

        public BuildStructure(StructureGhost structureGhost)
        {
            this.structureGhost = structureGhost;
        }

        public override void OnTick()
        {
            if (lifeSpan >= structureGhost.structureData.timeToBuild && isComplete != true)
            {
                structureGhost.FinishConstruction();
                isComplete = true;
                PopJob();
            }
        }
    } 

    public class DemolishStructure : Job
    {
        Structure structure; 

        public DemolishStructure(Structure structure)
        {
            this.structure = structure;
        }

        public override void OnStart()
        {
            StackJob(new TurnTowards(structure.position));
        }

        public override void OnTick()
        {
            if(lifeSpan > structure.StructureData.timeToBuild && isComplete == false)
            {
                structure.Demolish();

                isComplete = true;

                PopJob();
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

    public static class CollectionPathBuilder
    {


        public static LinkedList<Path> ShoppingList(List<ResourceQuantity> resourcesToCollect, int2 destination)
        {
            List<ResourceQuantity> _resourcesToCollect;
            int2 _destination;

            List<FoundHopper> foundHoppers = new List<FoundHopper>(); 
            LinkedList<Path> _superPath = new();



            return null;
        }

        public static LinkedList<Path> UpToQuantity(ResourceQuantity resourceQuantity, int2 destination)
        {


            return null;
        }
    }

    public class ExcecuteCollectAndDeliver : Job
    {
        private LinkedList<Path> superPath;
        private int2 destination;
        bool finished = false;

        public ExcecuteCollectAndDeliver(LinkedList<Path> superPath, int2 destination)
        {
            this.superPath = superPath;
            this.destination = destination;
        }

        public override void OnTick()
        {
            Job job;

            if (superPath.Count > 0)
            {
                if (superPath.Count > 1)
                {
                    job = new CollectResource(superPath.First().target);
                    StackJob(job);
                }

                var path = superPath.First();
                superPath.RemoveFirst();
                job = new TraversePath(path);
                StackJob(job);
            }
            else if (!finished)
            {
                job = new DeliverResource(destination);
                StackJob(job);
                finished = true;
            }
            else
            {
                PopJob(); return;
            }
        }
    }

    public class CollectAndDeliverShoppingList : Job
    {
        private List<ResourceQuantity> _resourcesToCollect;
        private int2 destination;

        private List<FoundHopper> foundHoppers = new List<FoundHopper>();

        private LinkedList<Path> _superPath = new();

        bool finished = false; 

        public CollectAndDeliverShoppingList(IEnumerable<ResourceQuantity> resourceQuantities, int2 destination)
        {
            this._resourcesToCollect = resourceQuantities.ToList();
            this.destination = destination;
        }

        public override void OnStart()
        {
            if (!TryFindHoppers()) { FailTask(); return; }

            _superPath = PathFinder.FindSuperPath((int2)rover.VisualPosition, foundHoppers.Select(foundHopper => foundHopper.Hopper.position).ToArray());

            if (_superPath == null || _superPath.Count == 0) { FailTask(); return; }

            _superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(_superPath.Last().nodes.Last(), destination));

            if (_superPath.Count < 2) { FailTask(); return; }

            PopJob();

            StackJob(new ExcecuteCollectAndDeliver(_superPath, destination));
        } 

        private bool TryFindHoppers()
        {
            bool foundResourceInHopper = false;
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

                        float2 hopperRoverOffset = rover.VisualPosition - hopper.position;
                        float hopperDistanceMagnitude = (hopperRoverOffset.x * hopperRoverOffset.x) + (hopperRoverOffset.y * hopperRoverOffset.y);

                        var foundHopper = foundHoppers.Find(foundHoppers => foundHoppers.Hopper == hopper);

                        if (foundHopper == null)
                        {
                            foundHopper = new FoundHopper(hopper, GetRoughDistanceToRover(hopper), GetRoughDistanceToDestination(hopper));
                            foundHoppers.Add(foundHopper);
                        }

                        var maxReservableQuantity = Mathf.Clamp(quantityInHopper, 0, remaingResourcesToFind[rq.resource]);
                        foundHopper.reservedResources.Add(new ResourceQuantity(rq.resource, maxReservableQuantity));
                        foundResourceInHopper = true;

                        remaingResourcesToFind[rq.resource] -= quantityInHopper;
                        if (remaingResourcesToFind[rq.resource] <= 0) { remaingResourcesToFind.Remove(rq.resource); break; }
                    }

                    if (foundResourceInHopper == false) return false;
                }

            if (remaingResourcesToFind.Count > 0) { return false; }

            foundHoppers.Sort(FHopperSortDistToDestination);

            foreach (var foundHopper in foundHoppers)
                foreach (var reservedResource in foundHopper.reservedResources)
                    foundHopper.Hopper.storageInventory.ReserveResource(reservedResource);

            return true;

            int HopperSortDistToDest(Hopper a, Hopper b)
            {
                float aDistance = GetRoughDistanceToDestination(a);
                float bDistance = GetRoughDistanceToDestination(b);

                if (aDistance == bDistance) return 0;
                else if (aDistance < bDistance) return -1;
                else if (aDistance > bDistance) return 1;
                else return 0;
            }

            int FHopperSortDistToDestination(FoundHopper a, FoundHopper b)
            {
                float aDistance = a.DistanceToDestination;
                float bDistance = b.DistanceToDestination;

                if (aDistance == bDistance) return 0;
                else if (aDistance < bDistance) return -1;
                else if (aDistance > bDistance) return 1;
                else return 0;
            }

            float GetRoughDistanceToRover(Hopper hopper)
            {
                float2 offset = rover.VisualPosition - hopper.position;
                return (offset.x * offset.x) + (offset.y * offset.y);
            }

            float GetRoughDistanceToDestination(Hopper hopper)
            {
                float2 offset = destination - hopper.position;
                return (offset.x * offset.x) + (offset.y * offset.y);
            }
        }
    } 

    public class CollectAndDeliverUpToQuantity
    {
        ResourceQuantity resourceQuantity;
        private int2 destination; 
        private List<FoundHopper> foundHoppers = new List<FoundHopper>(); 
        private LinkedList<Path> _superPath = new();

        bool finished = false;

        public CollectAndDeliverUpToQuantity(ResourceQuantity resourceQuantity, int2 destination)
        {
            this.resourceQuantity = resourceQuantity;
            this.destination = destination;
        }
    }
}