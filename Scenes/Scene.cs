using OpenTK;
using OpenCGE.Managers;

namespace OpenCGE.Scenes
{
    public abstract class Scene : IScene
    {
        protected SceneManager sceneManager;
        protected SystemManager systemManager;
        protected EntityManager entityManager;

        public Scene(SceneManager sceneManager, SystemManager systemManager, EntityManager entityManager)
        {
            this.sceneManager = sceneManager;
            this.systemManager = systemManager;
            this.entityManager = entityManager;
        }

        public abstract void Render(FrameEventArgs e);

        public abstract void Update(FrameEventArgs e);

        public abstract void Close();
    }
}
