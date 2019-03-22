using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCGE.Managers;
using OpenCGE.Objects;


namespace OpenCGE.Components
{
    public class ComponentGeometry : IComponent
    {
        Geometry geometry;

        public ComponentGeometry(string geometryName)
        {
            this.geometry = ResourceManager.LoadGeometry(geometryName);
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_GEOMETRY; }
        }

        public Geometry Geometry()
        {
            return geometry;
        }
    }
}
