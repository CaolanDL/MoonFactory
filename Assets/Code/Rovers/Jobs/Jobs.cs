using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using RoverTasks;
using ExtensionMethods;
using static UnityEngine.GraphicsBuffer;

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
            if (path.length < 2) { rover.TaskFailed(); return; }

            rover.DisplayObject.PlayParticleEffect("MovingParticles");

            Structure.StructureConstructed += EvaluatePathInterruption; // Subscribe to structure build completion for evaluating path interuption.

            AdvanceNode();
        }

        public override void OnTick()
        {
            foreach (var node in path.nodes)
            {
                GameManager.Instance.AddLifespanGizmo(new Vector3(node.x, 0.25f, node.y), 100);
            }

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

            if (Mathf.Approximately(_currentRotation, _targetRotation) || lifeSpan > 100)
            {
                PopJob();
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
            // Play collection animation on rover
            // Then run this ->

            foreach (var rq in resourcesToCollect)
            {
                if (rover.Inventory.TryAddResource(rq) == false) { throw new Exception("Rover attempted to exceed inventory capacity"); }
                targetPort.CollectResource(rq); 
            }

            PopJob();
        }
    }

    public class DeliverResources : Job
    {
        private StructureGhost targetGhost;
        private RequestPort targetPort;
        private List<ResourceQuantity> resourcesToDeliver;

        public DeliverResources(RequestPort target, List<ResourceQuantity> resourcesToDeliver)
        {
            targetPort = target;
            this.resourcesToDeliver = resourcesToDeliver;
        }
        public DeliverResources(StructureGhost target, List<ResourceQuantity> resourcesToDeliver)
        {
            targetGhost = target;
            this.resourcesToDeliver = resourcesToDeliver;
        }

        public override void OnStart()
        {
            int2 target = new();

            if (targetPort != null) { target = targetPort.parent.position; }
            if (targetGhost != null) { target = targetGhost.position; }

            StackJob(new TurnTowards(target));
        }

        public override void OnTick()
        {
            // Play delivery animation on rover
            // Then run this ->

            if (targetPort != null)
            {
                foreach (var rq in resourcesToDeliver)
                {
                    targetPort.
                }
            }
            if (targetGhost != null)
            {

            }

            foreach (var rq in resourcesToDeliver)
            { 
            }

            PopJob();
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
            if (lifeSpan > structure.StructureData.timeToBuild && isComplete == false)
            {
                structure.Demolish();

                isComplete = true;

                PopJob();
            }
        }
    }



}