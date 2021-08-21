using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Source used
// http://www.kfish.org/boids/pseudocode.html
[BurstCompile]
public class BoidSystem_Boids : SystemBase
{
    private EntityQuery Query;

    // We need a job because we have to access other entity component values while iterating over entities
    public struct UpdateBoidRulesJob : IJobEntityBatch
    {
        public ComponentTypeHandle<Boid_Boids> boidTypeHandle;
        public ComponentTypeHandle<Translation> translationTypeHandle;
        public ComponentTypeHandle<Rotation> rotationTypeHandle;
        public float dt;

        public float speed;
        public float bounds;
        public float coherence; // Factor to head towards the group centre
        public float seperation; // Factor to avoid running into others
        public float alignment; // Factor to match surrounding speed and direction

        // Calculate optimal vector to head towards the center of the group
        static private float3 CalculateCoherence(int i, NativeArray<Translation> translations)
        {
            var centreMass = float3.zero;
            for (int j = 0; j < translations.Length; j++)
            {
                if (i == j) continue;

                centreMass += translations[j].Value;
            }
            centreMass /= (translations.Length - 1);
            return centreMass - translations[i].Value;
        }

        // Calculate optimal vector to seperate other boids
        static private float3 CalculateSeperation(int i, NativeArray<Translation> translations)
        {
            var seperateVector = float3.zero;
            var seperateDistance = 10f;
            seperateDistance = seperateDistance * seperateDistance;

            for (int j = 0; j < translations.Length; j++)
            {
                if (i == j) continue;

                // If we are to close, we subtract the delta to the other boid from the desired seperateance
                if (math.lengthsq(translations[j].Value - translations[i].Value) < seperateDistance)
                {
                    seperateVector = seperateVector - (translations[j].Value - translations[i].Value);
                }
            }
            return seperateVector;
        }

        static private float3 CalculateAlignment(int i, NativeArray<Boid_Boids> boids)
        {
            var desiredVec = float3.zero;

            for (int j = 0; j < boids.Length; j++)
            {
                if (i == j) continue;

                desiredVec += boids[j].velocity;
            }

            desiredVec /= (boids.Length - 1);
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

        [BurstDiscard]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            NativeArray<Boid_Boids> boids = batchInChunk.GetNativeArray(boidTypeHandle);
            NativeArray<Translation> translations = batchInChunk.GetNativeArray(translationTypeHandle);
            NativeArray<Rotation> rotations = batchInChunk.GetNativeArray(rotationTypeHandle);
            //Debug.Log(batchInChunk.Count);
            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var boid = boids[i];

                var coherenceVec = CalculateCoherence(i, translations) * coherence;
                var seperationVec = CalculateSeperation(i, translations) * seperation;
                var alignmentVec = CalculateAlignment(i, boids) * alignment;
                var toBoundsVec = CalculateBounds(translations[i].Value, bounds);

                // Update velocity and position
                boid.velocity += coherenceVec + seperationVec + alignmentVec + toBoundsVec;

                // Normalize if speed is to high
                if (math.lengthsq(boid.velocity) > speed * speed)
                {
                    boid.velocity = math.normalize(boid.velocity) * speed;
                }

                translations[i] = new Translation { Value = translations[i].Value + boid.velocity * dt };
                rotations[i] = new Rotation { Value = quaternion.LookRotation(boid.velocity, new float3(0, 1, 0)) };
                boids[i] = boid;
            }
        }
    }
    protected override void OnCreate()
    {
        base.OnCreate();

        var desc = new EntityQueryDesc()
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<Boid_Boids>(),
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Rotation>(),
            }
        };

        Query = GetEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        // Setup job
        var job = new UpdateBoidRulesJob();

        job.boidTypeHandle = GetComponentTypeHandle<Boid_Boids>();
        job.translationTypeHandle = GetComponentTypeHandle<Translation>();
        job.rotationTypeHandle = GetComponentTypeHandle<Rotation>();

        job.speed = BoidGUISettings_Boids.speed;
        job.bounds = BoidGUISettings_Boids.boxSize;
        job.coherence = BoidGUISettings_Boids.coherence;
        job.seperation = BoidGUISettings_Boids.seperation;
        job.alignment = BoidGUISettings_Boids.alignment;

        job.dt = Time.DeltaTime;

        // Schedule job
        //job.Run(Query);

        Dependency = job.ScheduleParallel(Query, 1, Dependency);

        //var dt = Time.DeltaTime;
        //var speed = BoidGUISettings_Boids.speed;
        //Entities.ForEach((ref Translation translation, ref Rotation rotation, ref Boid_Boids boid) =>
        //{
        //    // Update velocity and position
        //    boid.velocity += boid.coherenceVec + boid.seperationVec + boid.alignmentVec + boid.toBoundsVec;

        //    // Normalize if speed is to high
        //    if (math.lengthsq(boid.velocity) > speed * speed)
        //    {
        //        boid.velocity = math.normalize(boid.velocity) * speed;
        //    }

        //    translation = new Translation { Value = translation.Value + boid.velocity * dt };
        //    rotation = new Rotation { Value = quaternion.LookRotation(boid.velocity, new float3(0, 1, 0)) };
        //}).Run();
        
        //Dependency = job.Schedule(Query, Dependency);
    }
}
