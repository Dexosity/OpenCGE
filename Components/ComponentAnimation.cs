using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenCGE.Components
{
    public class ComponentAnimation : IComponent
    {
        Vector3 transform;
        float speed;
        public ComponentAnimation(Vector3 pRotation, float speed)
        {
            transform = pRotation;
            this.speed = speed;
        }

        public Vector3 AnimateRotation
        {
            get { return transform; }
        }
        public float Speed
        {
            get { return this.speed; }
        }
        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_ANIMATION; }
        }
    }
}
