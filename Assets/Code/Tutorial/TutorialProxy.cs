using System; 

public class TutorialProxy
{
    public enum Events
    {
        None,
        BeginTutorial,
        BuildButtonPressed,
        StaticDrillSelected,
        StaticDrillInterfaceOpened,
        LanderInterfaceOpened,
        TechTreeOpened
    }

    public static Action<Events> Call { get; set; }

    public static Action BeginTutorial; 
    public static Action BuildButtonPressed;
    public static Action StaticDrillSelected;
    public static Action StaticDrillInterfaceOpened;
    public static Action LanderInterfaceOpened;
    public static Action TechTreeOpened;
}
