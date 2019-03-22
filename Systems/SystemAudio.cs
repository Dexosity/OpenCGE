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
using OpenTK.Audio;

namespace OpenCGE.Systems
{
    public class SystemAudio : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_AUDIO);

        public SystemAudio()
        {
            
        }

        public string Name
        {
            get { return "SystemAudio"; }
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

                IComponent audioComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
                });
                Audio audio = ((ComponentAudio)audioComponent).AudioObject;
                

                Play(position, audio);
            }
        }

        public void Play(Vector3 position, Audio audio)
        {
            audio.UpdateEmitterPosition(position);
        }
    }
}
