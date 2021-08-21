using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Boid_Boids : IComponentData
{
    //public float coherence; // Factor to head towards the group centre
    //public float seperation; // Factor to avoid running into others
    //public float alignment; // Factor to match surrounding speed and direction
    //public float speed;
    //public float3 coherenceVec;
    //public float3 seperationVec;
    //public float3 alignmentVec;
    //public float3 toBoundsVec;

    public float3 velocity;
}
