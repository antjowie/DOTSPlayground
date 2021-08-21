using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

// Entities with this component will rotate
public struct RotationSpeed : IComponentData
{
    public float RadiansPerSecond;
}
