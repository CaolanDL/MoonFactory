public class SpawnWidgetNode : TreeNode
{
    public override void OnUnlocked()
    {
        RoverManager.Instance.SpawnWidgetDropship();
    }
}
