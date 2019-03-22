using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenCGE.Managers;

namespace OpenCGE.Components
{
    public class ComponentCollider : IComponent
    {
        bool isRigid;

        public ComponentCollider(bool pIsRigid)
        {
            isRigid = pIsRigid;
        }
        /// <summary>
        /// Get/Set to determine if entity is a trigger or physical collider
        /// </summary>
        public bool isCollidable
        {
            get { return isRigid; }
            set { isRigid = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_COLLIDER; }
        }
    }
}
