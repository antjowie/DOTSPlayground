using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// Will spawn the entity, spawning from component allows us to make a job out of it
public struct Spawner_Boids : IComponentData
{
    public Entity entity;
    public int count;
}
