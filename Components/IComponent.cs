using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenCGE.Components
{
    [FlagsAttribute]
    public enum ComponentTypes {
        COMPONENT_NONE      = 0,
	    COMPONENT_POSITION  = 1 << 0,
        COMPONENT_GEOMETRY  = 1 << 1,
        COMPONENT_TEXTURE   = 1 << 2,
        COMPONENT_VELOCITY  = 1 << 3,
        COMPONENT_AUDIO     = 1 << 4,
        COMPONENT_TRANSFORM = 1 << 5,
        COMPONENT_SKYBOX    = 1 << 6,
        COMPONENT_COLLIDER  = 1 << 7,
        COMPONENT_AI        = 1 << 8,
        COMPONENT_ANIMATION = 1 << 9
    }

    public interface IComponent
    {
        ComponentTypes ComponentType
        {
            get;
        }
    }
}
