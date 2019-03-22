using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenCGE.Components
{
    public class ComponentTransform : IComponent
    {
        Matrix4 transform;

        public ComponentTransform(Vector3 pScale, Vector3 pRotation)
        {
            transform = Matrix4.CreateScale(pScale);
            transform *= Matrix4.CreateRotationX(pRotation.X);
            transform *= Matrix4.CreateRotationY(pRotation.Y);
            transform *= Matrix4.CreateRotationZ(pRotation.Z);
        }

        public ComponentTransform(Matrix4 trans)
        {
            transform = trans;
        }

        public Matrix4 Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TRANSFORM; }
        }
    }
}
