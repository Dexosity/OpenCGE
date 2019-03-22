using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenCGE.Managers;

namespace OpenCGE.Components
{
    public class ComponentTexture : IComponent
    {
        int texture;
        float scaler = 1;

        public ComponentTexture(string textureName)
        {
            texture = ResourceManager.LoadTexture(textureName);
        }

        public ComponentTexture(string textureName, float texScaler)
        {
            texture = ResourceManager.LoadTexture(textureName);
            scaler = texScaler;
        }

        public int Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public float Scaler
        {
            get { return scaler; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }
    }
}
