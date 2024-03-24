using ExtensionMethods;
using Logistics;
using System;
using System.Collections.Generic; 
using System.Linq;
using Unity.Mathematics;
using UnityEngine; 

namespace RoverJobs
{
    public class Job
    {
        public Rover rover;

        public ushort lifeSpan = 0;

        public bool isComplete = false;

        public Job() { }

        public void Tick()
        { 
            lifeSpan++;
            OnTick();
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


    public class FetchTask : Job
    {
        public FetchTask() { }

        public override void OnStart()
        {
            if (rover == null) { throw new System.Exception("Feath task tried to tick with null rover"); } 

/*            if (DevFlags.RoverTaskOverrideToPathfind)
            {
                rover.ActiveTask = new AutoPathfind()
                {
                    rover = rover,
                };
                rover.ActiveTask?.BuildJobs();
                Debug.Log($"{rover.ActiveTask}");
                return;
            }*/

            switch (rover.Module)
            {
                case RoverModule.None: break;

                case RoverModule.Construction:
                    rover.ActiveTask = ConstructionTasks.PopTask();
                    //Debug.Log($"{rover.ActiveTask}");
                    break;

                case RoverModule.Logistics:
                    //rover.ActiveTask = TaskManager.LogisticsTasks.Last();
                    break;

                case RoverModule.Mining:
                    //rover.ActiveTask = TaskManager.MiningTasks.Last();
                    break;
            }

            if(rover.ActiveTask != null)
            {
                rover.ActiveTask.rover = rover;
                rover.JobStack.Pop();
                rover.ActiveTask?.BuildJobs();
            }
        }
    }


    public class TraversePath : Job
    {
        private Path _path;

        private float2 _currentNodeWorldPosition;
        private float2 _nextNodeWorldPosition;

        private int _currentNodeIndex = 0;

        private float _interpAlpha = 0;
        private float _distanceToNextNode = 0;

        private bool _mustFindNewPath = false;

        public TraversePath(Path path)
        {
            this._path = path;
        }

        public override void OnStart()
        {
            UpdateRotation();

            if (_path.length < 2) { rover.TaskFailed(); return; }

            _currentNodeIndex = -1;
            AdvanceNode();

            Structure.StructureConstructed += EvaluatePathInterruption; // Subscribe to structure build completion for evaluating path interuption.
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

        public override void OnTick()
        {
            UpdatePosition();

            var lastNode = _path.nodes.Last();
            Graphics.DrawMesh(GlobalData.Instance.Gizmo, new Vector3(lastNode.x, 0.25f, lastNode.y), quaternion.identity, GlobalData.Instance.mat_GhostBlocked, 0);

            if (_interpAlpha >= 1)
            {
                AdvanceNode();

                if (_mustFindNewPath)
                {
                    FindNewPath();
                }
            }
        }

        private void UpdatePosition()
        {
            _interpAlpha += Rover.MoveSpeed * 0.02f / _distanceToNextNode;

            rover.position = (_nextNodeWorldPosition * _interpAlpha) + (_currentNodeWorldPosition * (1 - _interpAlpha)); // Lerp between current node and next node; 
        }

        private void UpdateRotation()
        {
            int2 difference = _path.nodes[_currentNodeIndex] - _path.nodes[_currentNodeIndex + 1];

            var rotation = Vector2.Angle(difference.ToVector2(), Vector2.down);

            rover.rotation = (ushort)rotation;

            rover.UpdateDoRotation();
        }

        private void AdvanceNode()
        {
            _currentNodeIndex++;

            _interpAlpha = 0;

            if (_currentNodeIndex + 1 == _path.length) { Finished(); return; }

            _currentNodeWorldPosition = _path.nodes[_currentNodeIndex];
            _nextNodeWorldPosition = _path.nodes[_currentNodeIndex + 1];

            _distanceToNextNode = Float2Extensions.DistanceBetween(_currentNodeWorldPosition, _nextNodeWorldPosition);

            UpdateRotation();
        }

        private void Finished()
        {
            rover.JobStack.Pop();

            // Unsubscribe events
            Structure.StructureConstructed -= EvaluatePathInterruption;
        }
    }


    public class CollectAndDeliverResources : Job
    {
        private List<ResourceQuantity> _resourcesToCollect;
        private int2 destination;

        private SortedList<float, Hopper> _foundHoppers = new SortedList<float, Hopper>(); 
        private LinkedList<Path> _superPath = new();

        bool finished = false;

        public CollectAndDeliverResources(IEnumerable<ResourceQuantity> resourceQuantities, int2 destination)
        {
            this._resourcesToCollect = resourceQuantities.ToList();
            this.destination = destination;
        }

        public override void OnStart()
        {
            if (!TryFindHoppers()) { FailTask(); return; }

            _superPath = PathFinder.FindSuperPath((int2)rover.position, _foundHoppers.Values.Select(hopper => hopper.position).ToArray());

            if(_superPath == null || _superPath.Count == 0) { FailTask(); return; }

            _superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(_superPath.Last().nodes.Last(), destination));

            if (_superPath.Count < 2) { FailTask(); return; }  
        }

        public override void OnTick()
        {
            Job job;

            if (_superPath.Count > 0)
            {
                job = new CollectResource(_superPath.First().nodes.Last());
                StackJob(job);

                var path = _superPath.First();
                _superPath.RemoveFirst();
                job = new TraversePath(path);
                StackJob(job);
            }
            else if (!finished)
            {
                job = new DeliverResource(destination);
                finished = true;
            }
            else
            {
                PopJob(); return; 
            }
        }

/*        private List<Path> FindSuperPath(int2 origin, int2[] destinations)
        {
            List<Path> superPath = new();

            int2 lastDestination = origin;

            for (int i = 0; i < destinations.Length; i++)
            {
                Path subPath = PathFinder.FindPathToAnyFreeNeighbor(lastDestination, destinations[i]);

                if (subPath == null) { return null; } // No path found;

                if (subPath.length > 8) { subPath.Compress(); } // Should add a static variable. Needs perf test to determine path length where compression becomes beneficial. 

                lastDestination = destinations[i];

                superPath.Add(subPath);
            } 

            return superPath;
        }*/ // Exported to Pathfinder

        private bool TryFindHoppers()
        {
            bool foundResourceInHopper = false;
            Dictionary<ResourceData, int> remaingResourcesToFind = new();
            foreach(var rq in _resourcesToCollect)
            {
                remaingResourcesToFind.Add(rq.resource, rq.quantity);
            }

            foreach (ResourceQuantity rq in _resourcesToCollect)
            {
                foreach (Hopper hopper in Hopper.pool)
                {
                    var quantityInHopper = hopper.storageInventory.GetQuantityOf(rq.resource);

                    if (quantityInHopper > 0)
                    {
                        /*                        if (_foundHoppers.ContainsValue(hopper)) // Break if Hopper has already been found
                                                {
                                                    foundResourceInHopper = true; 
                                                    break; 
                                                } */  

                        if (PathFinder.FindPathToAnyFreeNeighbor((int2)rover.position, hopper.position) == null) { continue; } // Hopper not reachable

                        float2 hopperRoverOffset = rover.position - hopper.position; 
                        float hopperDistanceMagnitude = (hopperRoverOffset.x * hopperRoverOffset.x) + (hopperRoverOffset.y * hopperRoverOffset.y);

                        if (!_foundHoppers.ContainsValue(hopper)) _foundHoppers.Add(hopperDistanceMagnitude, hopper); 
                        foundResourceInHopper = true;

                        remaingResourcesToFind[rq.resource] -= quantityInHopper;
                        if (remaingResourcesToFind[rq.resource] <= 0) { remaingResourcesToFind.Remove(rq.resource); break; } 
                    }
                }

                if (foundResourceInHopper == false)
                {
                    return false;
                }
            } 

            if(remaingResourcesToFind.Count > 0) { return false; }

            return true;
        }
    }

    public class TurnTowards : Job
    {
        private int2 _target;

        public TurnTowards(int2 target)
        {
            _target = target;
        }

        public override void OnStart()
        {
            UpdateRotation();
            PopJob();
        } 

        private void UpdateRotation()
        {
            float2 difference = _target - rover.position;

            var rotation = Vector2.Angle(Vector2.right, new Vector2(difference.x, difference.y));

            rover.rotation = (ushort)rotation;

            rover.UpdateDoRotation();
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
            if(lifeSpan > 50 )
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
}