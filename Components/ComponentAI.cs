using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenCGE.Components
{
    public class ComponentAI : IComponent
    {
        Vector3 OgTarget;
        Vector3 target;
        List<Vector3> path = new List<Vector3>();

        public ComponentAI(string vector)
        {
            if(vector != ""){
                string[] split = vector.Split(',');
                float[] values = Array.ConvertAll<string, float>(split, float.Parse);
                target = new Vector3(values[0], values[1], values[2]);
                OgTarget = target;
            }
            else
            {
                target = new Vector3(0.0f, 0.0f, 0.0f);
                ogTarget = target;
            }
            
        }
        public ComponentAI(Vector3 pos)
        {
            target = pos;
        }

        /// <summary>
        /// Get/Set for the target the AI aims to follow
        /// </summary>
        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }
        /// <summary>
        /// Get/Set for position of the original target the AI aimed to follow
        /// </summary>
        public Vector3 ogTarget
        {
            get { return OgTarget; }
            set { OgTarget = value; }
        }
        /// <summary>
        /// Get/Set the array of positions the AI must follow to reach it's target
        /// </summary>
        public Vector3[] Path
        {
            get { return path.ToArray(); }
            set { if (value != null) { path = value.ToList(); } }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AI; }
        }
    }
}
