using OpenTK;
using OpenTK.Audio.OpenAL;

namespace OpenCGE.Objects
{
    public class Audio
    {
        private Vector3 mEmitterPosition;
        private int mBuffer, mSource;
        private bool AutoPlay;
        private bool LoopPlay;
        private float VolumePlay;

        public Audio(int pBuffer)
        {
            mBuffer = pBuffer;
            AutoPlay = false;
            LoopPlay = false;
            VolumePlay = 0.8f;
            if (AutoPlay)
            {
                PlayAudio();
            }
        }
        /// <summary>
        /// Call to play this audio object
        /// </summary>
        public void PlayAudio()
        {
            // Create a sounds source using the audio clip
            mSource = AL.GenSource(); // gen a Source Handle
            AL.Source(mSource, ALSourcei.Buffer, mBuffer); // attach the buffer to a source
            AL.Source(mSource, ALSourceb.Looping, LoopPlay); // source loops infinitely if true
            AL.Source(mSource, ALSourcef.Gain, VolumePlay);
            AL.Source(mSource, ALSource3f.Position, ref mEmitterPosition);
            AL.SourcePlay(mSource);
        }

        /// <summary>
        /// Updates the position of the entity's audio
        /// </summary>
        /// <param name="pPosition">new position</param>
        public void UpdateEmitterPosition(Vector3 pPosition)
        {
            mEmitterPosition = pPosition;

            AL.Source(mSource, ALSource3f.Position, ref mEmitterPosition);
        }

        /// <summary>
        /// When scene is re-initalised this uses stored data to re-play audio without using original file
        /// </summary>
        public void ReloadAudio()
        {
            if (AutoPlay)
            {
                PlayAudio();
            }
        }
        /// <summary>
        /// Call to stop this audio object
        /// </summary>
        public void Close()
        {
            AL.SourceStop(mSource);
            AL.DeleteSource(mSource);
        }
        /// <summary>
        /// Set if this audio object should auto play on scene start
        /// </summary>
        public bool AutoPlayAudio
        {
            set { AutoPlay = value; }
        }
        /// <summary>
        /// Set if this audio obeject should auto loop on scene start
        /// </summary>
        public bool LoopPlayAudio
        {
            set { LoopPlay = value; }
        }
        /// <summary>
        /// Set the volume of this audio object
        /// </summary>
        public float VolumeOfAudio
        {
            set { VolumePlay = value; }
        }
    }
}
