using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class NoiseElementSystem : SystemBase
{
    float2 offset = float2.zero;

    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        offset.x += dt * NoiseElementGUISettings.speed;
        var localOffset = offset;
        var localYScale = NoiseElementGUISettings.yScale;
        var localZoom = NoiseElementGUISettings.zoom;

        Entities.ForEach((ref LocalToWorld localToWorld, in NoiseElement noiseElement) =>
        {
            var height = (noise.cnoise(new float2(noiseElement.index.x, noiseElement.index.y) / localZoom + localOffset)) * localYScale;

            var transform = float4x4.identity;

            // Rotate
            var up = noiseElement.facing.y == 0 ? new float3(0, 1, 0) : new float3(1, 0, 0);
            transform = math.mul(transform, float4x4.LookAt(float3.zero, noiseElement.facing, up));

            // Translate
            // We simply make the squared distance always be 1, since we rotate the whole plane
            // we can write the coordinate the same always
            // It wasn't that simple unfortunately. I think, the whole approach at the moment
            // is flawed (and unrealistic, you'd normally generate a mesh)
            // It was a nice attempt though and the result is pretty cool (even though it throws thousands of 
            // exceptions every frame)
            // Note: if index is bounds is 100 and we subtract 50, we go from -50 to 49
            // which I think is incorrect. Problem for later
            var pos = noiseElement.index - noiseElement.bounds / 2;
            var hypotenuse = math.sqrt(pos.x * pos.x + pos.y * pos.y);
            var ratio = hypotenuse / (noiseElement.bounds * 0.5f);

            transform = math.mul(transform,
                float4x4.Translate(new float3(
                    pos.x,
                    pos.y,
                    //(noiseElement.bounds * 0.5f + height) * ratio
                    (noiseElement.bounds * 0.5f + height) / ratio

                    ) * 1.5f));

            localToWorld.Value = transform;
        }).ScheduleParallel();
    }
}
