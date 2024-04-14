using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace ModularInterfaces
{
    public class ModularInterface : MonoBehaviour
    {
        [SerializeField] GameObject moduleContainer;

        public List<Module> modules = new();

        public void AddModule(Module module)
        {
            modules.Add(module);
        }
    }
}
