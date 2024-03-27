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

            if (rover.ActiveTask != null)
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

        private float2 _currentNodePosition;
        private float2 _nextNodePosition;

        private int _currentNodeIndex = -1;

        //private float _interpAlpha = 0;
        //private float _distanceToNextNode = 0;

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

            //_interpAlpha += Rover.MoveSpeed * 0.02f / _distanceToNextNode;

            var lastNode = _path.nodes.Last();
            Graphics.DrawMesh(GlobalData.Instance.gizmo_Axis, new Vector3(lastNode.x, 0.25f, lastNode.y), quaternion.identity, GlobalData.Instance.mat_GhostBlocked, 0);

            if (rover.position.Equals(_nextNodePosition) || lifeSpan == 0)
            {
                AdvanceNode();

                if (_mustFindNewPath)
                {
                    FindNewPath();
                }
            }

            if (_finished || _mustTurn) return;

            rover.position = Vector2.MoveTowards(rover.position, _nextNodePosition, rover.MoveSpeed * Time.fixedDeltaTime);

            //rover.position = (_nextNodeWorldPosition * _interpAlpha) + (_currentNodeWorldPosition * (1 - _interpAlpha)); // Lerp between current node and next node;  
        }

        //x private void UpdateRotation()
        //x {
        //x     int2 difference = _path.nodes[_currentNodeIndex] - _path.nodes[_currentNodeIndex + 1];

        //x     var rotation = Vector2.Angle(difference.ToVector2(), Vector2.down);

        //x     rover.rotation = (ushort)rotation;

        //x     rover.UpdateDoRotation();
        //x }

        private void AdvanceNode()
        {
            _currentNodeIndex++;

            //_interpAlpha = 0;

            if (_currentNodeIndex + 1 == _path.nodes.Length) { Finished(); _finished = true; return; }

            _currentNodePosition = _path.nodes[_currentNodeIndex];
            _nextNodePosition = _path.nodes[_currentNodeIndex + 1];

            //_distanceToNextNode = Float2Extensions.DistanceBetween(_currentNodeWorldPosition, _nextNodeWorldPosition);

            // Stack a TurnTowards job if the next node would cause the rover to turn
            Vector2 _difference = (Vector2)(rover.position - _nextNodePosition);
            float _angle = Vector2.SignedAngle(_difference, Vector2.down);
            //Debug.Log($"{_angle} : {rover.rotation}");
            if (! (Mathf.Approximately(_angle, rover.rotation) || (Mathf.Approximately(Mathf.Abs(_angle), 180) && Mathf.Approximately(Mathf.Abs(rover.rotation), 180))) )
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
            PopJob();

            rover.DisplayObject.StopParticleEffect("MovingParticles");

            // Unsubscribe events
            Structure.StructureConstructed -= EvaluatePathInterruption;
        }
    }


    public class CollectAndDeliverResources : Job
    {
        private List<ResourceQuantity> _resourcesToCollect;
        private int2 destination;

        private List<Hopper> foundHoppers = new List<Hopper>();
        public class FoundHopper
        {
            Hopper Hopper; float DistanceToRover; 
            public FoundHopper(Hopper hopper, float distanceToRover)
            {
                Hopper = hopper;
                DistanceToRover = distanceToRover;
            }
        }

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

            _superPath = PathFinder.FindSuperPath((int2)rover.position, foundHoppers.Select(hopper => hopper.position).ToArray());

            if (_superPath == null || _superPath.Count == 0) { FailTask(); return; }

            _superPath.AddLast(PathFinder.FindPathToAnyFreeNeighbor(_superPath.Last().nodes.Last(), destination));

            if (_superPath.Count < 2) { FailTask(); return; }
        }

        public override void OnTick()
        {
            Job job;

            if (_superPath.Count > 0)
            {
                job = new CollectResource(_superPath.First().trueDestination);
                StackJob(job);

                var path = _superPath.First();
                _superPath.RemoveFirst();
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

        private bool TryFindHoppers()
        {
            bool foundResourceInHopper = false;
            Dictionary<ResourceData, int> remaingResourcesToFind = new();
            foundHoppers = new();

            foreach (var rq in _resourcesToCollect)
            {
                remaingResourcesToFind.Add(rq.resource, rq.quantity);
            }

            List<Hopper> hoppers = Hopper.pool;

            hoppers.Sort(HopperSort);

            foreach (ResourceQuantity rq in _resourcesToCollect)
            {
                foreach (Hopper hopper in Hopper.pool)
                {
                    var quantityInHopper = hopper.storageInventory.GetQuantityOf(rq.resource);

                    if (quantityInHopper > 0)
                    {
                        if (PathFinder.FindPathToAnyFreeNeighbor((int2)rover.position, hopper.position) == null) { continue; } // Hopper not reachable

                        float2 hopperRoverOffset = rover.position - hopper.position;
                        float hopperDistanceMagnitude = (hopperRoverOffset.x * hopperRoverOffset.x) + (hopperRoverOffset.y * hopperRoverOffset.y);

                        if (!foundHoppers.Contains(hopper)) foundHoppers.Add(hopper);
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

            if (remaingResourcesToFind.Count > 0) { return false; }

            // Sort Hoppers Here 

            foundHoppers.Sort(HopperSort);

            int HopperSort(Hopper a, Hopper b)
            {
                float aDistance = GetRoughHopperDistance(a);
                float bDistance = GetRoughHopperDistance(b);

                if (aDistance == bDistance) return 0;
                else if (aDistance < bDistance) return -1;
                else if (aDistance > bDistance) return 1;
                else return 0;
            }

            float GetRoughHopperDistance(Hopper hopper)
            {
                float2 hopperRoverOffset = rover.position - hopper.position;
                return (hopperRoverOffset.x * hopperRoverOffset.x) + (hopperRoverOffset.y * hopperRoverOffset.y);
            }

            //

            return true;
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
            _currentRotation = rover.rotation;

            Vector2 difference = (Vector2)(rover.position - target);
            _targetRotation = Vector2.SignedAngle(difference.normalized, Vector2.down);

            //Debug.Log($"Rotate Towards {target} at {_targetRotation} from {_currentRotation}");
        }

        public override void OnTick()
        {
            _currentRotation = Mathf.MoveTowardsAngle(_currentRotation, _targetRotation, rover.TurnSpeed);

            rover.rotation = _currentRotation;

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
            if (lifeSpan >= structureGhost.structureData.timeToBuild)
            {
                structureGhost.FinishConstruction();
                PopJob();
            }
        }
    }
}