using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

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
            for(int x = 0; x < spawner.countX; x++)
            {
                for (int z = 0; z < spawner.countX; z++)
                {
                    var instance = ecb.Instantiate(entityInQueryIndex,spawner.entity);
                    var pos = translate.Value;
                    pos.x += x * 1.5f;
                    pos.z += z * 1.5f;

                    ecb.SetComponent(entityInQueryIndex, instance, new Translation { Value = pos });
                }
            }

            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();

        Barrier.AddJobHandleForProducer(Dependency);
    }
}
