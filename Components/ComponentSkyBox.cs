using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenCGE.Managers;

namespace OpenCGE.Components
{
    public class ComponentSkyBox : IComponent
    {
        int skybox;

        public ComponentSkyBox(string[] skyboxTextures)
        {
            skybox = ResourceManager.LoadCubeMapTexture(skyboxTextures);
        }

        public int Skybox
        {
            get { return skybox; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_SKYBOX; }
        }
    }
}
