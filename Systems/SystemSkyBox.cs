using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenCGE.Components;
using OpenCGE.Objects;
using OpenCGE.Scenes;

namespace OpenCGE.Systems
{
    public class SystemSkyBox : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_TRANSFORM | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_SKYBOX);

        private Camera mCamera;

        protected int pgmID;
        protected int vsID;
        protected int fsID;
        protected int attribute_vpos;
        protected int uniform_mview;
        protected int uniform_mtransform;
        protected int uniform_cubemap;

        public SystemSkyBox(ref Camera pCamera)
        {
            mCamera = pCamera;
            pgmID = GL.CreateProgram();
            LoadShader("Shaders/skyboxVert.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("Shaders/skyboxFrag.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            attribute_vpos = GL.GetAttribLocation(pgmID, "a_Position");
            uniform_mview = GL.GetUniformLocation(pgmID, "WorldViewProj");
            uniform_mtransform = GL.GetUniformLocation(pgmID, "Transform");
            uniform_cubemap = GL.GetUniformLocation(pgmID, "cubeMap");

            if (attribute_vpos == -1 || uniform_mview == -1 || uniform_mtransform == -1 || uniform_cubemap == -1)
            {
                Console.WriteLine("Error binding attributes");
            }
        }

        void LoadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        public string Name
        {
            get { return "SystemSkyBox"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent geometryComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
                });
                Geometry geometry = ((ComponentGeometry)geometryComponent).Geometry();

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                ((ComponentPosition)positionComponent).Position = mCamera.Position;
                Vector3 position = ((ComponentPosition)positionComponent).Position;
               

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                Matrix4 transform = ((ComponentTransform)transformComponent).Transform;

                IComponent skyboxComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_SKYBOX;
                });
                int texture = ((ComponentSkyBox)skyboxComponent).Skybox;
                
                Draw(position, geometry, transform, texture);

                
            }
        }

        public void Draw(Vector3 position, Geometry geometry, Matrix4 transform, int texture)
        {
            GL.UseProgram(pgmID);
            GL.Uniform1(uniform_cubemap, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            // Enables all openGL features for this current render
            GL.BindTexture(TextureTarget.TextureCubeMap, texture);
            GL.Enable(EnableCap.TextureCubeMap);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            // Updates the shader with current world details
            Matrix4 world = Matrix4.CreateTranslation(position);
            Matrix4 worldViewProjection = world * mCamera.View * mCamera.Projection;
            GL.UniformMatrix4(uniform_mview, false, ref worldViewProjection);
            GL.UniformMatrix4(uniform_mtransform, true, ref transform);
            // Calls the geometry to be rendered 
            geometry.Render();
            // Rendering clean up
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.TextureCubeMap);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
