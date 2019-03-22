using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using OpenTK;

namespace OpenGL_Game.Components
{
    class ComponentLightEmitter : IComponent
    {
        Vector3 position;

        public ComponentLightEmitter(Vector3 pPosition)
        {
            position = pPosition;
        }

        public Vector3 LightPoint
        {
            get { return position; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_LIGHTEMITTER; }
        }
    }
}
