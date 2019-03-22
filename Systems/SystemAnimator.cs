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
using OpenCGE.Managers;
using System.Threading;

namespace OpenCGE.Systems
{
    public class SystemAnimator : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_ANIMATION);
        private float deltaTime;

        public SystemAnimator()
        {
            deltaTime = 0.01f;
        }

        public string Name
        {
            get { return "SystemAnimator"; }
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
                Vector3 position = ((ComponentPosition)positionComponent).Position;

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });

                IComponent animationComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_ANIMATION;
                });
               
                OnAnimate((ComponentAnimation)animationComponent, position, (ComponentTransform)transformComponent);
                
            }
        }

        public void OnAnimate(ComponentAnimation animation, Vector3 position, ComponentTransform transform)
        {
            Matrix4 mat = Matrix4.CreateRotationX(animation.AnimateRotation.X * animation.Speed);
            mat *= Matrix4.CreateRotationY(animation.AnimateRotation.Y * animation.Speed);
            mat *= Matrix4.CreateRotationZ(animation.AnimateRotation.Z * animation.Speed);
            transform.Transform *= mat;
        }
       
        public void UpdateDeltaTime(float time)
        {
            deltaTime = time;
        }
    }
}
