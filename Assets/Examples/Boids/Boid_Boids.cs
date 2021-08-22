using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

// Should be called Velocity instead of Boid
[GenerateAuthoringComponent]
public struct Boid_Boids : IComponentData
{
    //public float coherence; // Factor to head towards the group centre
    //public float seperation; // Factor to avoid running into others
    //public float alignment; // Factor to match surrounding speed and direction
    //public float speed;

    public float3 velocity;
}
