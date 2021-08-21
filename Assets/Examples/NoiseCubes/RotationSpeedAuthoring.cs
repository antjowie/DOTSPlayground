using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class RotationSpeedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float DegreesPerSecond = 360f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RotationSpeed { RadiansPerSecond = math.radians(DegreesPerSecond) });
    }
}
