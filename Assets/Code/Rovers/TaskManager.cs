using System;
using System.Collections.Generic;
using RoverTasks;

public class TaskManager
{
    public static LinkedList<Task> AllTasks = new();

    //public static LinkedList<ConstructionTasks> ConstructionTasks = new();

    public static LinkedList<LogisticsTasks> LogisticsTasks = new();

    public static LinkedList<MiningTasks> MiningTasks = new();

    public TaskManager()
    {
        GameManager.OnGameExit += OnGameExit;
    }

    void OnGameExit()
    {
        AllTasks.Clear();
        ConstructionTasks.Clear();
        LogisticsTasks.Clear();
        MiningTasks.Clear();
        GameManager.OnGameExit -= OnGameExit;
    }
     
    private static readonly Type ConstructionTaskType = typeof(ConstructionTasks);
    private static readonly Type LogisticsTaskType = typeof(LogisticsTasks);
    private static readonly Type MiningTaskType = typeof(MiningTasks);

    public static void QueueTask(Task task)
    {
        AllTasks.AddLast(task);

        var taskType = task.GetType();

        if (taskType.IsSubclassOf(ConstructionTaskType))
        {
            ConstructionTasks.QueueTask((ConstructionTasks)task);
        }
        else if (taskType == LogisticsTaskType)
        {
            //LogisticsTasks.Add((LogisticsTask)task);
        }
        else if (taskType == MiningTaskType)
        {
            //MiningTasks.Add((MiningTask)task);
        }
    } 

    public static void CancelTask(Task task)
    {
        Task.Pool.Remove(task);

        var taskType = task.GetType();

        if (taskType == ConstructionTaskType)
        {
            ConstructionTasks.CancelTask((ConstructionTasks)task);
        }
    }
}
