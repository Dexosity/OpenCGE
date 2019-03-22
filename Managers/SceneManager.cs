using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenCGE.Scenes;
using OpenCGE.Objects;
using System.Reflection;
using OpenTK.Audio.OpenAL;

namespace OpenCGE.Managers
{
    public class SceneManager : GameWindow
    {
        WindowConfig config;
        private SystemManager systemManager;
        private EntityManager entityManager;
        private Scene scene;

        public static int width = 1600, height = 900;
        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;
        public Camera camera;
        public bool lockMouse;
        private int SceneIndex;
        private string[] Namespace;

        private Vector3 ListenerPos, ListenerDir, ListenerUp;

        public SceneManager(string[] pNamespace) : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
        {
            config = new WindowConfig();
            Namespace = pNamespace;
            //mainAssembly = pAssembly;
            base.Width = config.Width;
            base.Height = config.Height;
            CursorVisible = false;
            lockMouse = true;
            // Iniitlise managers
            systemManager = new SystemManager();
            entityManager = new EntityManager();
            // Set up main camera
            camera = new Camera(config.Width, config.Height);
            // Setup audio listener
            ListenerPos = camera.Position;
            ListenerDir = camera.View.ExtractTranslation();
            ListenerUp = Vector3.UnitY;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);

            //Load the GUI
            GUI.SetUpGUI(width, height);

            // Loads and intialises first scene
            SceneIndex = 0;
            Object[] array = new Object[] { this, systemManager, entityManager };
            scene = (Scene)Activator.CreateInstance(Type.GetType(Namespace[SceneIndex]), array);

            
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            // Camera settings
            camera.UpdateCameraView();
            // Update AL 
            ListenerPos = camera.Position;
            ListenerDir = camera.Direction;

            AL.Listener(ALListener3f.Position, ref ListenerPos);
            AL.Listener(ALListenerfv.Orientation, ref ListenerDir, ref ListenerUp);
            
            if (lockMouse && Focused)
            {
                camera.MouseLinkedCamera((float)e.Time);
            }
            updater(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, config.Width, config.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            renderer(e);

            GL.Flush();
            SwapBuffers();
        }
        
        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);

            if (Focused)
            {
                camera.ReloadMouse();
            }
        }
        /// <summary>
        /// Changes the current scene to the scene matching provided index
        /// </summary>
        /// <param name="index">Index of new scene (Index = Order in config 0 = 1st)</param>
        public void ChangeScene(int index)
        {
            // Scene clean up
            CleanUpScene();
            // New scene intialisation
            camera.ReloadCamera();
            SceneIndex = index;
            scene = (Scene)Activator.CreateInstance(Type.GetType(Namespace[SceneIndex]), this, systemManager, entityManager);
        }
        /// <summary>
        /// Changes the current scene to the next scene in the sequence
        /// </summary>
        public void NextScene()
        {
            // Scene clean up
            CleanUpScene();
            // New scene intialisation
            camera.ReloadCamera();
            SceneIndex++;
            // If current scene is last scene then loop round to first scene
            if(SceneIndex >= Namespace.Length) { SceneIndex = 0; }
            camera.UpdateCameraProjection();
            scene = (Scene)Activator.CreateInstance(Type.GetType(Namespace[SceneIndex]), this, systemManager, entityManager);
        }
        /// <summary>
        /// Changes the current scene to the previous scene in the sequence
        /// </summary>
        public void PreviousScene()
        {
            // Scene clean up
            CleanUpScene();
            // New scene intialisation
            camera.ReloadCamera();
            SceneIndex--;
            // If current scene is first scene in sequence then loop to last scene
            if(SceneIndex <= 0) { SceneIndex = Namespace.Length - 1; }
            scene = (Scene)Activator.CreateInstance(Type.GetType(Namespace[SceneIndex]), this, systemManager, entityManager);
        }
        /// <summary>
        /// Cleans up all stored data on previous current scene ready for intialisation of new one
        /// </summary>
        private void CleanUpScene()
        {
            scene.Close();
            entityManager.DeleteEntities();
            entityManager.ClearEntities();
            systemManager.ClearSystems();
        }
        /// <summary>
        /// Returns current scene index
        /// </summary>
        public int GetSceneIndex
        {
            get { return SceneIndex; }
        }
        /// <summary>
        /// Get/Set if mouse should control the camera
        /// </summary>
        public bool MouseControlledCamera
        {
            get { return lockMouse; }
            set { lockMouse = value; }
        }
        /// <summary>
        /// Get/Set if the cursor should be visiable ingame
        /// </summary>
        public bool ShowCursor
        {
            get { return CursorVisible; }
            set { CursorVisible = value; }
        }
        /// <summary>
        /// Returns the game window width
        /// </summary>
        public int WindowWidth
        {
            get { return config.Width; }
        }
        /// <summary>
        /// Returns the game window height
        /// </summary>
        public int WindowHeight
        {
            get { return config.Height; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            config.Width = Width;
            config.Height = Height;
            //Load the GUI
            GUI.SetUpGUI(Width, Height);
        }
    }

}

