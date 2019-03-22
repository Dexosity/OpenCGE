using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenCGE.Managers;
using OpenCGE.Objects;
using OpenTK.Audio;

namespace OpenCGE.Components
{
    public class ComponentAudio : IComponent
    {
        Audio audio;

        public ComponentAudio(string audioName, float volume)
        {
            audio = new Audio(ResourceManager.LoadAudio(audioName));
            audio.VolumeOfAudio = volume;
        }
        public ComponentAudio(string audioName, float volume, bool autoPlay)
        {
            audio = new Audio(ResourceManager.LoadAudio(audioName));
            audio.AutoPlayAudio = autoPlay;
            audio.VolumeOfAudio = volume;
        }
        public ComponentAudio(string audioName, float volume, bool autoPlay, bool loopPlay)
        {
            audio = new Audio(ResourceManager.LoadAudio(audioName));
            audio.AutoPlayAudio = autoPlay;
            audio.LoopPlayAudio = loopPlay;
            audio.VolumeOfAudio = volume;
        }

        public Audio AudioObject
        {
            get { return audio; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }

        public void CloseAudio()
        {
            audio.Close();
        }
    }
}
