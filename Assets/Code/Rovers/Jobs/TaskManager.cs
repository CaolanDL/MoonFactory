using System;
using System.Collections.Generic;
using RoverTasks;
 
public class TaskManager
{
    public static LinkedList<Task> Tasks = new(); 
    public static Dictionary<TaskCategory, TaskCategoryManager> CategoryManagers = new();

    public TaskManager()
    {
        GameManager.OnGameExit += OnGameExit; 

        CategoryManagers.Add(TaskCategory.Construction, new TaskCategoryManager());
        CategoryManagers.Add(TaskCategory.Logistics, new TaskCategoryManager());
        CategoryManagers.Add(TaskCategory.Mining, new TaskCategoryManager());
    }

    void OnGameExit()
    {
        ClearAllTasks();
        CategoryManagers.Clear();
        GameManager.OnGameExit -= OnGameExit;
    }
    
    // Sub task manager types
    //private static readonly Type ConstructionTaskType = typeof(ConstructionTask);
    //private static readonly Type LogisticsTaskType = typeof(LogisticsTasks);
    //private static readonly Type MiningTaskType = typeof(MiningTasks);

    public static void QueueTask(Task task)
    {
        Tasks.AddLast(task); 
        var taskCategory = task.Category;

        foreach(var category in CategoryManagers) 
            if(task.Category == category.Key)
                category.Value.QueueTask(task);    
    } 

    public static void CancelTask(Task task)
    { 
        Task.Pool.Remove(task); 
        var taskCategory = task.Category;

        foreach (var category in CategoryManagers)
            if (task.Category == category.Key)
                category.Value.CancelTask(task); 
    }

    public static void ClearAllTasks()
    {
        Tasks.Clear(); 
        foreach(var category in CategoryManagers) 
            category.Value.Clear();  
    }

    public static Task PopTask(TaskCategory category)
    {
        Task task;

        if (Tasks.Count == 0) { return null; }

        if (category == TaskCategory.All)
        {
            task = Tasks.First.Value;
            Tasks.RemoveFirst();

            foreach (var _category in CategoryManagers)
                if (task.Category == _category.Key)
                    _category.Value.PopTask();
            return task;
        }
        else
            foreach (var _category in CategoryManagers)
                if (category == _category.Key)
                {
                    var _task = _category.Value.PopTask();
                    Tasks.Remove(_task);
                    return _task;
                }
        return null;
    }

    public static Task PeekTask(TaskCategory category)
    {
        Task task;

        if (Tasks.Count == 0) { return null; }

        if (category == TaskCategory.All)
        {
            task = Tasks.First.Value;  
            return task;
        }
        else
            foreach (var _category in CategoryManagers)
                if (category == _category.Key)
                {
                    var _task = _category.Value.PeekTask(); 
                    return _task;
                }
        return null;
    }
}

public class TaskCategoryManager
{
    LinkedList<Task> tasks = new LinkedList<Task>(); 

    public Task PopTask()
    {
        if (tasks.Count == 0) return null;
        var task = tasks.First;
        tasks.RemoveFirst();
        if(TaskManager.Tasks.First== task)
            TaskManager.Tasks.RemoveFirst(); 
        return task.Value;
    }

    public Task PeekTask()
    {
        if (tasks.Count == 0) return null;
        var task = tasks.First;
        return task.Value;
    }

    public void CancelTask(Task task)
    {
        tasks.Remove(task); 
    }

    public void QueueTask(Task task)
    {
        tasks.AddLast(task);   
    }

    public void Clear()
    {
        tasks.Clear();
    }
}