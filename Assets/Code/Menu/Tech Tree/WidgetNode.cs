public class SpawnWidgetNode : TreeNode
{
    private void OnEnable()
    {
        techTree = GetComponentInParent<TechTreeController>();
    }

    public override void OnUnlocked()
    {
        RoverManager.Instance.SpawnWidgetDropship();
    }
}
