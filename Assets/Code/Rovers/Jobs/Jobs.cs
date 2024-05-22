using System; 
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using RoverTasks;
using ExtensionMethods;
using MoonFactory.Interfaces;

namespace RoverJobs
{
    public class Job
    {
        public Rover rover;

        public int lifeSpan = 0;

        public bool wasStarted = false;
        public bool isComplete = false;

        public Job() { }

        public void Tick()
        {
            lifeSpan++;
            OnTick();
        }

        public void Start()
        {
            wasStarted = true;
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


        //------------------------------------------------------
        public int SortDistanceToRover(Structure a, Structure b)
        {
            return FloatSort(Float2Extensions.DistanceBetween(rover.GridPosition, a.position), Float2Extensions.DistanceBetween(rover.GridPosition, b.position));
        }

        public static int FloatSort(float a, float b)
        {
            if (a == b) return 0;
            else if (a < b) return -1;
            else if (a > b) return 1;
            else return 0;
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

                case RoverModule.Construction:
                    rover.ActiveTask = TaskManager.PopTask(TaskCategory.Construction);
                    break;

                case RoverModule.Logistics:
                    rover.ActiveTask = TaskManager.PopTask(TaskCategory.Logistics);
                    break;

                case RoverModule.Mining:
                    rover.ActiveTask = TaskManager.PopTask(TaskCategory.Mining);
                    break;

                case RoverModule.Widget:
                    rover.ActiveTask = TaskManager.PopTask(TaskCategory.All);
                    break;
            }

            if (rover.ActiveTask != null && rover.recentTaskIDs.Contains(rover.ActiveTask.ID))
            {
                rover.TaskFailed();
            }

            if (rover.ActiveTask != null)
            {
                rover.ActiveTask.rover = rover;
                rover.recentTaskIDs.Add(rover.ActiveTask.ID);
                rover.ActiveTask.BuildJobs();
                rover.ActiveTask.OnFetched?.Invoke();
            }

            PopJob();
        }
    }


    public class GoCharge : Job
    {
        ChargingPad targetPad;

        public override void OnStart()
        {
            ChargingPad.Pool.Sort(SortDistanceToRover);

            foreach(var pad in ChargingPad.Pool)
            {
                if (pad.inUse == true) continue;
                else
                {
                    targetPad = pad;
                    targetPad.inUse = true;
                    break;
                }
            }

            if(targetPad == null)
            {
                FailTask(); return;
            }

            var path = PathFinder.FindPath(rover.GridPosition, targetPad.position);
            if(path != null)
            {
                targetPad.inUse = true;
                StackJob(new TurnTowards(targetPad.position + targetPad.rotation.ToInt2()));
                StackJob(new TraversePath(path));
            }
            else
            {
                FailTask(); return;
            }
        }

        public override void OnTick()
        { 
            if(rover.GridPosition.Equals(targetPad.position))
            { 
                rover.powerLevel += targetPad.powerSupply;
            }
            if(rover.powerLevel >= Rover.maxPowerLevel)
            {
                rover.powerLevel = Rover.maxPowerLevel;
                targetPad.inUse = false;
                PopJob();
            }
        }
    }


    public class TraversePath : Job
    {
        private Path path;

        private int2 currentNodePosition;
        private int2 nextNodePosition;

        private int currentNodeIndex = -1;

        private bool mustFindNewPath = false;
        private bool mustTurn = false;

        private bool finished;

        public TraversePath(Path path)
        {
            this.path = path;
        }

        public override void OnStart()
        {
            //if (path.length < 2) { rover.TaskFailed(); return; }

            rover.DisplayObject.PlayParticleEffect("MovingParticles");
            rover.DisplayObject.CrossfadeAnimation("Moving", 0.5f); 

            Structure.StructureConstructed += EvaluatePathInterruption; // Subscribe to structure build completion for evaluating path interuption.

            AdvanceNode();
        }

        public override void OnTick()
        {
/*            foreach (var node in path.nodes)
            {
                GameManager.Instance.AddLifespanGizmo(new Vector3(node.x, 0.25f, node.y), 100);
            }*/

            if (rover.VisualPosition.Equals(nextNodePosition))
            {
                AdvanceNode();

                if (mustFindNewPath)
                {
                    FindNewPath();
                }

                if (mustTurn) { return; }
            }

            if (finished) return;

            rover.VisualPosition = Vector2.MoveTowards(rover.VisualPosition, nextNodePosition.ToVector2(), rover.MoveSpeed * Time.fixedDeltaTime);

            mustTurn = false;
        }

        private void AdvanceNode()
        {
            currentNodeIndex++;

            if (currentNodeIndex + 1 == path.nodes.Length) { Finished(); finished = true; return; }

            currentNodePosition = path.nodes[currentNodeIndex];
            nextNodePosition = path.nodes[currentNodeIndex + 1];

            rover.GridPosition = (int2)currentNodePosition;

            // Stack a TurnTowards job if the next node would cause the rover to turn
            Vector2 _difference = (Vector2)(rover.VisualPosition - nextNodePosition);
            float _angle = Vector2.SignedAngle(_difference, Vector2.down);
            if (!(Mathf.Approximately(_angle, rover.VisualRotation) || (Mathf.Approximately(Mathf.Abs(_angle), 180) && Mathf.Approximately(Mathf.Abs(rover.VisualRotation), 180))))
            {
                StackJob(new TurnTowards(nextNodePosition));
                mustTurn = true;
            }
        }


        private void EvaluatePathInterruption(Structure structure)
        {
            if (path.nodes.Contains(structure.position)) mustFindNewPath = true;
            else mustFindNewPath = false;
        }

        private void FindNewPath()
        {
            path = PathFinder.FindPath(path.nodes[currentNodeIndex], path.nodes.Last());
            currentNodeIndex = -1;
            AdvanceNode();
            mustFindNewPath = false;
        }

        private void Finished()
        {
            rover.GridPosition = path.destination;

            PopJob();

            rover.DisplayObject.StopParticleEffect("MovingParticles");
            rover.DisplayObject.CrossfadeAnimation("Idle", 0.5f);

            // Unsubscribe events
            Structure.StructureConstructed -= EvaluatePathInterruption;
        }
    }

    public class GotoNeighbor : Job
    {
        Entity Entity;

        Path path;

        public GotoNeighbor(Entity entity)
        {
            this.Entity = entity;
        }

        public override void OnStart()
        {
            if (Entity != null) path = PathFinder.FindPathToAnyFreeNeighbor(rover, rover.GridPosition, Entity.position);

            if (path == null) { FailTask(); return; }

            PopJob();

            StackJob(new TraversePath(path));
        }
    }

    public class GotoPosition : Job
    {
        int2 destination;

        Path path;

        public GotoPosition(int2 position)
        {
            this.destination = position;
        }

        public override void OnStart()
        {
            path = PathFinder.FindPath(rover.GridPosition, destination);

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

            if (Mathf.Approximately(_currentRotation, _targetRotation) || lifeSpan > 100)
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
            if (lifeSpan >= 10 && isComplete != true)
            {
                structureGhost.FinishConstruction();
                isComplete = true;
                PopJob();
            }
        }
    }

    public class Demolish : Job
    {
        Entity entity;
        IDemolishable demolishable;

        public Demolish(Entity entity)
        {
            this.entity = entity;
            this.demolishable = (IDemolishable)entity;
        }

        public override void OnStart()
        {
            StackJob(new TurnTowards(entity.position));
        }

        public override void OnTick()
        {
            if (lifeSpan > demolishable.DemolishTime && isComplete == false)
            {
                demolishable.Demolish();

                isComplete = true;

                PopJob();
            }
        }
    } 
}