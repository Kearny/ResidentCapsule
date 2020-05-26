using Unity.Entities;
using Unity.NetCode;

namespace Com.Kearny.Shooter.Networking
{
    [GenerateAuthoringComponent]
    public struct MovableCubeComponent : IComponentData
    {
        [GhostDefaultField] public int PlayerId;
    }
}
