using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Bridge : Structure
{
    public static int Interval = 16;
    public static float visualHeight = 1.54f;

    public BridgeLink Link;
    public Bridge Other;
    ResourceData InputItem;
    public Platform ElevatorPlatform = new();

    static int ElevatorMaxHeight = Interval;
    public int ElevatorHeight;

    public bool IsOutput = false;
    bool isCycling = false;
    bool isWaiting = false;

    public override void OnConstructed()
    {
        base.OnConstructed();

        SearchForLinkage();

        ElevatorHeight = ElevatorMaxHeight;
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
                if (bridge.Link == null && bridge.rotation == rotation.Rotate(2))
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
        Debug.Log("Became Output");
    }

    public override void OnOutputLost()
    {
        IsOutput = false;

        ElevatorPlatform.resource = null;
        isWaiting = false;
        ElevatorHeight = ElevatorMaxHeight;
        isCycling = false;

        Debug.Log("Became Input");
    }

    public override bool TryInputItem(ResourceData resource, TinyTransform inputTransform)
    {
        if (InputItem != null) { return false; }

        InputItem = resource;

        return true;
    }

    public bool TryOutput(ResourceData resource)
    {
        return TryOutputItem(resource, StructureData.ports[0]);
    }

    public override void OnTick()
    {
        if (isCycling)
        {
            if (IsOutput) DoOutputCycle();
            else DoInputCycle();
        }

        if (Link != null && Link.BaseA == this) Link.Tick();
    }

    void TryStartCycle()
    {
        var LinkPlatform = Link.GetPlatformAt(this);
        if (LinkPlatform == null) { return; }
        if (IsOutput)
        {
            if (LinkPlatform.resource == null) { return; }
            Debug.Log($"{LinkPlatform.resource}");
            Debug.Log("Starting Output Cycle");
            isCycling = true;
            ElevatorPlatform.resource = LinkPlatform.resource;
            LinkPlatform.resource = null;
            Link.itemCount--;
        }
        else if (LinkPlatform.resource == null && InputItem != null)
        {
            Debug.Log("Starting Input Cycle");
            isCycling = true;
        }
    }

    void DoInputCycle()
    {
        if (ElevatorPlatform.resource != null && ElevatorHeight <= ElevatorMaxHeight)
        {
            ElevatorHeight++;
        }
        if (ElevatorPlatform.resource == null && ElevatorHeight >= 0)
        {
            ElevatorHeight--;
        }
        if (ElevatorHeight >= ElevatorMaxHeight)
        {
            if (Link.TryAddItem(ElevatorPlatform.resource, this))
            {
                Debug.Log("Item Trasferred to Loop");
                ElevatorPlatform.resource = null;
                isCycling = false;
            }
        }
        if (ElevatorPlatform.resource == null && ElevatorHeight <= 0)
        {
            ElevatorPlatform.resource = InputItem;
            InputItem = null;
        }
    }

    void DoOutputCycle()
    {
        if (ElevatorPlatform.resource == null && ElevatorHeight >= ElevatorMaxHeight)
        {
            isCycling = false;
            Debug.Log("Finished Output Cycle");
        }
        if (ElevatorHeight == 0)
        {
            isWaiting = true;

            if (TryOutput(ElevatorPlatform.resource))
            {
                ElevatorPlatform.resource = null;
                isWaiting = false;
            }
        }
        if (ElevatorPlatform.resource != null && ElevatorHeight > 0)
        {
            ElevatorHeight--;
        }
        if (ElevatorPlatform.resource == null && ElevatorHeight < ElevatorMaxHeight && isWaiting == false)
        {
            ElevatorHeight++;
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
        public Bridge BaseB;
        public List<Platform> platforms = new();
        public Dictionary<Platform, int> Loop = new();
        public static int AlphaMax = Interval;
        int _alpha;
        int Alpha { get { return _alpha; } set { _alpha = value % AlphaMax; } }
        public int itemCount = 0;
        public int length;
        public int tileSpan;
        bool isHalted = false;

        public BridgeLink(Bridge baseA, Bridge baseB, int tileSpan)
        {
            BaseA = baseA;
            BaseB = baseB;
            BaseA.Link = this;
            BaseB.Link = this;

            this.tileSpan = tileSpan;
            this.length = tileSpan * AlphaMax * 2;

            for (int i = 0; i < length; i += AlphaMax)
            {
                var newplatform = new Platform();
                platforms.Add(newplatform);
                Loop.Add(newplatform, i);
            }

            Pool.Add(this);
        }

        public void Tick()
        {
            isHalted = BaseA.isCycling | BaseB.isCycling;
            //Debug.Log($"isHalted? {isHalted}");

            if ((itemCount > 0 || Alpha != 0) && !isHalted)
            {
                ProgressPlatforms();
            }
            if (!isHalted)
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

            foreach (var platform in platforms)
                if (Loop[platform] == t)
                    return platform;
            return null;
        }

        void ProgressPlatforms()
        {
            Alpha++;
            foreach (var platform in platforms)
            {
                Loop[platform] = (Loop[platform] + 1) % length;
            }
        }

        public bool TryAddItem(ResourceData resourceData, Bridge origin)
        {
            var platform = GetPlatformAt(origin);
            if (platform != null)
            {
                platform.resource = resourceData;
                itemCount++;
                Debug.Log($"Added resource {platform.resource} to platform");
                return true;
            }
            return false;
        }
    }

}

// Right now this just renders debug gizmos along straight paths. Should replace with spline system, and add platform swapping.
public class BridgeRenderer
{
    ChunkedMatrixArray platformMatrices = new();
    ChunkedMatrixArray bridgeSegments = new();
    public int RenderCount = 0;

    public Mesh mesh;
    public Material material;

    public void Init()  { }

    public void RenderAll()
    {
        foreach (var link in Bridge.BridgeLink.Pool)
        {
            Matrix4x4 matrix;
            Vector3 pos;
            foreach (var kvp in link.Loop)
            {
                Vector3 lrOffset = kvp.Value < link.length / 2 ? Vector3.left * 0.2f : Vector3.right * 0.2f;
                lrOffset = link.BaseA.rotation.ToQuaternion() * lrOffset;
                pos = link.BaseA.position.ToVector3() + (link.BaseA.rotation.ToInt2().ToVector3() * (Mathf.PingPong(kvp.Value, link.length / 2) / Bridge.Interval) + (Vector3.up * Bridge.visualHeight) + lrOffset);
                matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);

                platformMatrices.QueueMatrix(matrix);
                if(kvp.Key.resource != null)
                {
                    GameManager.Instance.ItemRenderer.QueueItem(kvp.Key.resource, matrix);
                } 
            }
            RenderElevator(link.BaseA);
            RenderElevator(link.BaseB);

            void RenderElevator(Bridge bridge)
            {
                pos = bridge.position.ToVector3() + (Vector3.up / Bridge.Interval * bridge.ElevatorHeight * Bridge.visualHeight);
                matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                platformMatrices.QueueMatrix(matrix);

                if (bridge.ElevatorPlatform.resource != null) GameManager.Instance.ItemRenderer.QueueItem(bridge.ElevatorPlatform.resource, matrix); 
            }

            for (int i = 1; i < link.tileSpan; i++)
            {
                var rotation = link.BaseA.rotation.ToQuaternion();
                pos = (link.BaseA.position + ( link.BaseA.rotation.ToInt2() * i)).ToVector3();

                matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
                bridgeSegments.QueueMatrix(matrix);
            }
        }

        var renderdata = GameManager.Instance.RenderData;

        BatchRenderer.RenderChunkedMatrixArray(platformMatrices, renderdata.BridgePlatform, renderdata.UniversalMaterial);
        BatchRenderer.RenderChunkedMatrixArray(bridgeSegments, renderdata.BridgeSegment, renderdata.UniversalMaterial);
    }

    public void RenderVisible()
    {

    }
}