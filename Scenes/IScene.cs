using OpenTK;

namespace OpenCGE.Scenes
{
    interface IScene
    {
        void Render(FrameEventArgs e);
        void Update(FrameEventArgs e);
        void Close();
    }
}
