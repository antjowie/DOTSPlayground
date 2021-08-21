using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

// Entities with this component are part of the collection spawned by spawnersystem
public struct NoiseElement : IComponentData
{
    public float RadiansPerSecond;

    public int2 index;
    public int bounds;
    public float3 facing;
}
