using OpenCGE;
using OpenCGE.Objects;

namespace OpenCGE.Systems
{
    public interface ISystem
    {
        void OnAction(Entity entity);

        // Property signatures: 
        string Name
        {
            get;
        }
    }
}
