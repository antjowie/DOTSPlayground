using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// System looks for spawner entities and executes them.
// Deletes them afterwards
public class SpawnerSystem : SystemBase
{
    EntityCommandBufferSystem Barrier;

    protected override void OnCreate()
    {
        base.OnCreate();
        Barrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = Barrier.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((Entity entity, int entityInQueryIndex, in Spawner spawner, in Translation translate) =>
        {
            for (int i = 0; i < 6; i++)
            {
                var facing = float3.zero;
                switch(i)
                {
                    case 0: facing.x = 1; break;
                    case 1: facing.x = -1; break;
                    case 2: facing.y = 1; break;
                    case 3: facing.y = -1; break;
                    case 4: facing.z = 1; break;
                    case 5: facing.z = -1; break;
                }

                for (int x = 0; x < spawner.sideCount; x++)
                {
                    for (int z = 0; z < spawner.sideCount; z++)
                    {
                        var instance = ecb.Instantiate(entityInQueryIndex, spawner.entity);

                        // Setup NoiseElement component
                        ecb.SetComponent(entityInQueryIndex, instance, new NoiseElement
                        {
                            index = new int2(x, z),
                            bounds = spawner.sideCount,
                            facing = facing,
                        });

                        ecb.AddComponent<LocalToWorld>(entityInQueryIndex, instance);
                        ecb.RemoveComponent<Translation>(entityInQueryIndex, instance);
                        ecb.RemoveComponent<Rotation>(entityInQueryIndex, instance);
                    }
                }
            }

            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();

        Barrier.AddJobHandleForProducer(Dependency);
    }
}
