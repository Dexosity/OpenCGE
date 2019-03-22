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
    public class SystemRender : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TEXTURE | ComponentTypes.COMPONENT_TRANSFORM);

        private Camera mCamera;

        protected int pgmID;
        protected int vsID;
        protected int fsID;
        protected int attribute_vtex;
        protected int attribute_vpos;
        protected int attribute_vnorm;

        protected int uniform_stex;
        protected int uniform_texscale;
        protected int uniform_mview;
        protected int uniform_mmodel;
        protected int uniform_mtransform;
        protected int uniform_mlightpos;
        protected int uniform_meyepos;
        protected int uniform_mnumLights;
        protected Vector3[] LightPos;

        public SystemRender(ref Camera pCamera)
        {
            mCamera = pCamera;
            LightPos = new Vector3[] { new Vector3(0.0f, 10.0f, 0.0f) };

            BindShaderValues();
        }

        public SystemRender(ref Camera pCamera, Vector3 pLightPoint)
        {
            LightPos = new Vector3[] { pLightPoint };
            mCamera = pCamera;

            BindShaderValues();
        }
        public SystemRender(ref Camera pCamera, Vector3[] pLightPoints)
        {
            mCamera = pCamera;
            LightPos = pLightPoints;

            BindShaderValues();
        }

        void BindShaderValues()
        {
            pgmID = GL.CreateProgram();
            LoadShader("Shaders/stdLtVert.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("Shaders/stdLtFrag.glsl", ShaderType.FragmentShader, pgmID, out fsID);
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            attribute_vpos = GL.GetAttribLocation(pgmID, "a_Position");
            attribute_vtex = GL.GetAttribLocation(pgmID, "a_TexCoord");
            attribute_vnorm = GL.GetAttribLocation(pgmID, "a_Normal");
            uniform_mview = GL.GetUniformLocation(pgmID, "ViewProj");
            uniform_mmodel = GL.GetUniformLocation(pgmID, "Model");
            uniform_mtransform = GL.GetUniformLocation(pgmID, "Transform");
            uniform_texscale = GL.GetUniformLocation(pgmID, "TexScaler");
            uniform_stex = GL.GetUniformLocation(pgmID, "s_texture");
            uniform_mlightpos = GL.GetUniformLocation(pgmID, "LightPosition");
            uniform_meyepos = GL.GetUniformLocation(pgmID, "EyePosition");

            if (attribute_vpos == -1 || attribute_vtex == -1 || attribute_vnorm == -1 || uniform_stex == -1 || uniform_texscale == -1 || uniform_mmodel == -1 || uniform_mview == -1 || uniform_mtransform == -1 || uniform_mlightpos == -1 || uniform_meyepos == -1)
            {
                Console.WriteLine("Error binding attributes");
                Console.WriteLine(attribute_vpos + "\n" + attribute_vtex + "\n" + attribute_vnorm + "\n" + uniform_stex + "\n" + uniform_texscale + "\n" + uniform_mmodel + "\n" + uniform_mview + "\n" + uniform_mtransform + "\n" + uniform_mlightpos + "\n" + uniform_meyepos);
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
            get { return "SystemRender"; }
        }
        /// <summary>
        /// Updates the position of the light emitter
        /// </summary>
        /// <param name="pPosition">vector3 light position</param>
        public void UpdateLightSource(Vector3[] pPosition)
        {
            LightPos = pPosition;
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent geometryComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_GEOMETRY;
                });
                Geometry geometry = ((ComponentGeometry)geometryComponent).Geometry();

                IComponent transformComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TRANSFORM;
                });
                Matrix4 transform = ((ComponentTransform)transformComponent).Transform;

                IComponent positionComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });               
                Vector3 position = ((ComponentPosition)positionComponent).Position;

                IComponent textureComponent = components.Find(delegate(IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_TEXTURE;
                });
                int texture = ((ComponentTexture)textureComponent).Texture;
                float scaler = ((ComponentTexture)textureComponent).Scaler;

                Draw(position, geometry, transform, texture, scaler);
            }
        }

        public void Draw(Vector3 position, Geometry geometry, Matrix4 transform, int texture, float scaler)
        {
            GL.UseProgram(pgmID);
            GL.Uniform1(uniform_stex, 0);
            GL.Uniform1(uniform_texscale, scaler);
            GL.ActiveTexture(TextureUnit.Texture0);
            // Enables all openGL features for this current render
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            // Updates the shader with current world details        
            Matrix4 model = Matrix4.CreateTranslation(position);
            //model *= transform;
            Matrix4 worldViewProjection = mCamera.View * mCamera.Projection;
            GL.UniformMatrix4(uniform_mview, false, ref worldViewProjection);
            GL.UniformMatrix4(uniform_mmodel, false, ref model);
            GL.UniformMatrix4(uniform_mtransform, true, ref transform);
            GL.Uniform3(uniform_meyepos, mCamera.Position);
            List<float> lightPoints = new List<float>();
            for (int i = 0; i < LightPos.Length; i++)
            {
                lightPoints.Add(LightPos[i].X);
                lightPoints.Add(LightPos[i].Y);
                lightPoints.Add(LightPos[i].Z);
            }
            GL.Uniform3(uniform_mlightpos, LightPos.Length, lightPoints.ToArray());
            // Calls the geometry to be rendered
            geometry.Render();
            // Rendering clean up
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Texture2D);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
    }
}
