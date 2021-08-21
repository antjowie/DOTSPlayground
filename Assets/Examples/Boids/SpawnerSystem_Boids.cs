using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnerSystem_Boids : SystemBase
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

        Entities.ForEach((Entity entity, int entityInQueryIndex, in Spawner_Boids spawner, in Translation translation) =>
        {
            var random = new Random(2001);

            for(int i = 0; i < spawner.count; i++)
            {
                var instance = ecb.Instantiate(entityInQueryIndex, spawner.entity);

                ecb.SetComponent(entityInQueryIndex, instance, new Translation { Value = random.NextFloat3(new float3(10)) + translation.Value });
            }

            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();

        Barrier.AddJobHandleForProducer(Dependency);
    }
}
