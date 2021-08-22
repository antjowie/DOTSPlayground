using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Source used
// http://www.kfish.org/boids/pseudocode.html
public class BoidSystem_Boids : SystemBase
{
    EntityQuery BoidQuery;

    static private float3 CalculateCoherence(int i, NativeArray<float3> translations)
    {
        var centreMass = float3.zero;
        for (int j = 0; j < translations.Length; j++)
        {
            if (i == j) continue;

            centreMass += translations[j];
        }
        centreMass /= (translations.Length - 1);
        return centreMass - translations[i];
    }

    static private float3 CalculateSeperation(int i, NativeArray<float3> translations)
    {
        var seperateVector = float3.zero;
        var seperateDistance = 10f;
        seperateDistance = seperateDistance * seperateDistance;

        for (int j = 0; j < translations.Length; j++)
        {
            if (i == j) continue;

            // If we are to close, we subtract the delta to the other boid from the desired seperateance
            if (math.lengthsq(translations[j] - translations[i]) < seperateDistance)
            {
                seperateVector = seperateVector - (translations[j] - translations[i]);
            }
        }
        return seperateVector;
    }

    static private float3 CalculateAlignment(int i, NativeArray<float3> velocities)
    {
        var desiredVec = float3.zero;

        for (int j = 0; j < velocities.Length; j++)
        {
            if (i == j) continue;

            desiredVec += velocities[j];
        }

        desiredVec /= (velocities.Length - 1);
        return desiredVec;
    }

    static private float3 CalculateBounds(float3 pos, float bounds)
    {
        var toBounds = float3.zero;
        if (pos.x < -bounds) toBounds.x = 10;
        if (pos.x > bounds) toBounds.x = -10;
        if (pos.y < -bounds) toBounds.y = 10;
        if (pos.y > bounds) toBounds.y = -10;
        if (pos.z < -bounds) toBounds.z = 10;
        if (pos.z > bounds) toBounds.z = -10;
        return toBounds;
    }

    protected override void OnUpdate()
    {
        // Make a local copy of any variables we use for calculation
        var speed = BoidGUISettings_Boids.speed;
        var bounds = BoidGUISettings_Boids.boxSize;
        var coherence = BoidGUISettings_Boids.coherence;
        var seperation = BoidGUISettings_Boids.seperation;
        var alignment = BoidGUISettings_Boids.alignment;
        var dt = Time.DeltaTime;

        var copyPositions = new NativeArray<float3>(BoidQuery.CalculateEntityCount(), Allocator.TempJob);
        var copyVelocities = new NativeArray<float3>(BoidQuery.CalculateEntityCount(), Allocator.TempJob);

        // Gather component data into native arrays, which should be more efficient than randomly looking up entities
        var fillNativeArraysHandle = Entities
            .WithName("FillNativeArrays")
            .WithStoreEntityQueryInField(ref BoidQuery)
            .ForEach((int entityInQueryIndex, in Translation translation, in Boid_Boids boid) =>
            {
                copyPositions[entityInQueryIndex] = translation.Value;
                copyVelocities[entityInQueryIndex] = boid.velocity;
            }).ScheduleParallel(Dependency);

        var boidHandle = Entities
            .WithName("BoidAlgorithm")
            .WithReadOnly(copyPositions)
            .WithReadOnly(copyVelocities)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref Boid_Boids boid) =>
            {
                var coherenceVec = CalculateCoherence(entityInQueryIndex, copyPositions) * coherence;
                var seperationVec = CalculateSeperation(entityInQueryIndex, copyPositions) * seperation;
                var alignmentVec = CalculateAlignment(entityInQueryIndex, copyVelocities) * alignment;
                var toBoundsVec = CalculateBounds(translation.Value, bounds);

                // Update velocity and position
                boid.velocity += coherenceVec + seperationVec + alignmentVec + toBoundsVec;

                // Normalize if speed is to high
                if (math.lengthsq(boid.velocity) > speed * speed)
                {
                    boid.velocity = math.normalize(boid.velocity) * speed;
                }

                translation.Value += boid.velocity * dt;
                rotation.Value = quaternion.LookRotation(boid.velocity, new float3(0, 1, 0));
            })
            .WithDisposeOnCompletion(copyPositions)
            .WithDisposeOnCompletion(copyVelocities)
            .ScheduleParallel(fillNativeArraysHandle);
        
        Dependency = boidHandle;
    }
}
