using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Structure
{
    public BridgeLink Link;

    public Bridge Other;

    ResourceData InputItem;
    Platform ElevatorPlatform = new();

    static int ElevatorMaxHeight = 1000;
    public int ElevatorHeight;

    public bool IsOutput = false;
    bool isCycling = false;
    bool isWaiting = false;

    public override void OnConstructed()
    {
        base.OnConstructed();

        SearchForLinkage();
    }

    void SearchForLinkage()
    {
        var worldGrid = GameManager.Instance.GameWorld.worldGrid;
        for (int i = 1; i < 7; i++)
        {
            var pos = rotation.ToInt2() * i + position;
            var entityAtPos = worldGrid.GetEntityAt(pos);

            if (entityAtPos != null && entityAtPos.GetType() == typeof(Bridge))
            {
                var bridge = (Bridge)entityAtPos;
                if(bridge.Link == null && bridge.rotation == rotation.Rotate(2))
                {
                    LinkWith(bridge, i);
                }
            }
        }
    }

    void LinkWith(Bridge bridge, int distance)
    {
        Link = new BridgeLink(this, bridge, distance);
    }

    public override void OnOutputFound()
    {
        IsOutput = true;
    }

    public override void OnOutputLost()
    {
        IsOutput = false;
    }

    public override bool TryInputItem(ResourceData resource, TinyTransform inputTransform)
    {
        if (InputItem != null) { return false; }

        InputItem = resource;

        return true; 
    } 

    public bool TryOutput(ResourceData resource)
    {
        bool success = TryOutputItem(resource, StructureData.ports[0]);

        isCycling = false;

        return success;
    }

    public override void OnTick()
    {
        base.OnTick();

        if (isCycling)
        {
            if(IsOutput)
            {
                DoOutputCycle();
            }
            else
            {
                DoInputCycle();
            }
        } 

        Link?.Tick();
    }

    void TryStartCycle()
    {
        var LinkPlatform = Link.GetPlatformAt(this);
        if (LinkPlatform == null) return;
        if (IsOutput && LinkPlatform.resource != null)
        {
            isCycling = true;
            ElevatorPlatform.resource = LinkPlatform.resource;
        }
        else if (LinkPlatform.resource == null)
        {
            isCycling = true;
        }
    }

    void DoInputCycle()
    {
        if (ElevatorPlatform.resource != null && ElevatorHeight < ElevatorMaxHeight)
        {
            ElevatorHeight++;
        }
        if (ElevatorPlatform.resource == null && ElevatorHeight > 0)
        {
            ElevatorHeight--;
        }
        if (ElevatorHeight >= ElevatorMaxHeight)
        {
            if (Link.TryAddItem(ElevatorPlatform.resource, this)) isCycling = false;
        } 
        if (ElevatorPlatform.resource == null && ElevatorHeight <= 0)
        {
            ElevatorPlatform.resource = InputItem;
            InputItem = null;
        }
    }

    void DoOutputCycle()
    {
        if (ElevatorPlatform.resource != null && ElevatorHeight < ElevatorMaxHeight)
        {
            ElevatorHeight--;
        }
        if (ElevatorPlatform.resource == null && ElevatorHeight > 0)
        {
            ElevatorHeight++;
        }
        if (ElevatorPlatform.resource == null && ElevatorHeight >= ElevatorMaxHeight)
        {  
             isCycling = false;
        } 
        if (ElevatorHeight <= 0)
        {
            isWaiting = true;
            if (TryOutput(ElevatorPlatform.resource)) isWaiting = false;
        }
    }

    public class Platform
    {
        public ResourceData resource;
        public bool isHidden;
    }

    public class BridgeLink
    {
        public static List<BridgeLink> Pool = new();

        public Bridge BaseA;
        Bridge BaseB;
        public Dictionary<Platform, int> Loop = new();
        static int alphaMax = 100;
        int _alpha;
        int Alpha { get { return _alpha; } set { _alpha = value % alphaMax; } }
        int itemCount = 0;
        public int length; 
        bool isHalted = false;

        public BridgeLink(Bridge baseA, Bridge baseB, int tileSpan)
        {
            BaseA = baseA;
            BaseB = baseB; 
            BaseA.Link = this;
            BaseB.Link = this;

            this.length = tileSpan * alphaMax * 2 -1;

            for(int i = 0; i < length; i += alphaMax) 
                Loop.Add(new(), i); 

            Pool.Add(this); 
        }

        public void Tick()
        {
            isHalted = BaseA.isCycling || BaseB.isCycling;

            if ((itemCount > 0 || Alpha != 0) && !isHalted)
            {
                ProgressPlatforms();
            } 
            if(Alpha == 0 && !isHalted)
            {
                BaseA.TryStartCycle();
                BaseB.TryStartCycle();
            } 
        } 

        Platform GetPlatformAt(int t)
        {
            foreach (var item in Loop) 
                if (item.Value == t)
                    return item.Key; 
            return null;
        }

        public Platform GetPlatformAt(Bridge origin)
        {
            var t = origin == BaseA ? 0 : length / 2;

            foreach (var item in Loop) 
                if (item.Value == t) 
                    return item.Key;  
            return null;
        }

        void ProgressPlatforms()
        {
            Alpha++;
            foreach(KeyValuePair<Platform, int> kvp in Loop)
            {
                Loop[kvp.Key] += Loop[kvp.Key] + 1 % length; 
            }
        }

        public bool TryAddItem(ResourceData resourceData, Bridge origin)
        {
            if(Alpha > 1) return false; 
            var platform = GetPlatformAt(origin);
            if (platform != null) { platform.resource = resourceData; return true; } 
            return false;
        } 
    }

}


public class PlatformRenderer
{
    ChunkedMatrixArray chunkedMatrixArray = new();
    public int RenderCount = 0;

    public Mesh mesh;
    public Material material;

    public void Init()
    {
        mesh = GameManager.Instance.GlobalData.gizmo_Axis;
        material = GameManager.Instance.RenderData.devgizmo_material;
    }

    public void RenderAll()
    {
        foreach (var link in Bridge.BridgeLink.Pool)
        {
            foreach(var kvp in link.Loop)
            {
                var pos = link.BaseA.position.ToVector3() + (link.BaseA.rotation.ToInt2() * kvp.Value).ToVector3();
                var matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);

                chunkedMatrixArray.QueueMatrix(matrix);
            } 
        }

        BatchRenderer.RenderChunkedMatrixArray(chunkedMatrixArray, mesh, material);
    }

    public void RenderVisible()
    {

    }
}