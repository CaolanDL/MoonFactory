using RoverTasks;
using System.Collections;
using UnityEngine;

namespace MoonFactory
{
    namespace Interfaces
    {
        interface IDemolishable
        {
            void Demolish();
            void ToggleDemolition();
            void FlagForDemolition();
            void CancelDemolition();
            public int DemolishTime { get; } 
        }
    }
} 