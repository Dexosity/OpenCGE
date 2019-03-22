using System;
using System.Collections.Generic;
using OpenCGE.Systems;
using OpenCGE.Objects;

namespace OpenCGE.Managers
{
    public class SystemManager
    {
        List<ISystem> systemList = new List<ISystem>();

        public SystemManager()
        {
        }
        /// <summary>
        /// Loops through all systems and for each entity passes it into the action method of the current system
        /// </summary>
        /// <param name="entityManager"></param>
        public void ActionSystems(EntityManager entityManager)
        {
            List<Entity> entityList = entityManager.Entities();
            foreach(ISystem system in systemList)
            {
                foreach(Entity entity in entityList)
                {
                    system.OnAction(entity);
                }
            }
        }
        /// <summary>
        /// Add a new system to the game scenes list of systems
        /// </summary>
        /// <param name="system">System to be added</param>
        public void AddSystem(ISystem system)
        {
            ISystem result = FindSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            systemList.Add(system);
        }
        /// <summary>
        /// Returns the system matching provided name if it exists
        /// </summary>
        /// <param name="name">Name of system to find</param>
        /// <returns></returns>
        public ISystem FindSystem(string name)
        {
            return systemList.Find(delegate(ISystem system)
            {
                return system.Name == name;
            }
            );
        }
        /// <summary>
        /// Clears the entire list of systems
        /// </summary>
        public void ClearSystems()
        {
            systemList = new List<ISystem>();
        }
    }
}
