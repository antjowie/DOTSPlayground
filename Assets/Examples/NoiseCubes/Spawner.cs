using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Will spawn the entity, spawning from component allows us to make a job out of it
public struct Spawner : IComponentData
{
    public Entity entity;
    public int sideCount;
}
