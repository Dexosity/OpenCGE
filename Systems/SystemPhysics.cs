using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenCGE.Components;
using OpenCGE.Objects;
using OpenCGE.Scenes;

namespace OpenCGE.Systems
{
    public class SystemPhysics : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_VELOCITY);
        float deltaTime = 0.1f;

        public string Name
        {
            get { return "SystemPhysics"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                IComponent velocityComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_VELOCITY;
                });

                Motion((ComponentPosition)positionComponent, (ComponentVelocity)velocityComponent);
            }
        }

        public void Motion(ComponentPosition pPos, ComponentVelocity pVel)
        {
            pPos.Position += pVel.Veclotiy * deltaTime;
        }
        
        public void UpdateDeltaTime(float time)
        {
            deltaTime = time;
        }
    }
}
