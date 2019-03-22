using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenCGE.Components
{
    public class ComponentPosition : IComponent
    {
        Vector3 position;

        public ComponentPosition(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
        }

        public ComponentPosition(Vector3 pos)
        {
            position = pos;
        }
        /// <summary>
        /// Get/Set the vector3 position of the entity
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_POSITION; }
        }
    }
}
