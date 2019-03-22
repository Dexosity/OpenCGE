using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenCGE.Components;

namespace OpenCGE.Objects
{
    public class Entity
    {
        string name;
        List<IComponent> componentList = new List<IComponent>();
        ComponentTypes mask;
 
        public Entity(string name)
        {
            this.name = name;
        }

        /// <summary>Adds a single component</summary>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component cannot be null");

            componentList.Add(component);
            mask |= component.ComponentType;
        }
        /// <summary>
        /// Returns the name of the this entity
        /// </summary>
        public String Name
        {
            get { return name; }
        }
        /// <summary>
        /// Return the component mask of the this entity
        /// </summary>
        public ComponentTypes Mask
        {
            get { return mask; }
        }
        /// <summary>
        /// Returns a list of all components contained in this entity
        /// </summary>
        public List<IComponent> Components
        {
            get { return componentList; }
        }
        /// <summary>
        /// Returns the component provided if it exists in this entity
        /// </summary>
        /// <param name="type">Type of component to find</param>
        /// <returns></returns>
        public IComponent FindComponent(ComponentTypes type)
        {
            return componentList.Find(delegate (IComponent c)
            {
                return c.ComponentType == type;
            }
            );
        }
    }
}
