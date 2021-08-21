using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotationSpeedSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities.ForEach((ref Rotation rotation, in RotationSpeed speed) =>
        {
            rotation.Value = math.mul(
                rotation.Value,
                quaternion.RotateY(speed.RadiansPerSecond * dt)
                );
        }).ScheduleParallel();
    }
}
