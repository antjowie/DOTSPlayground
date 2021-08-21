using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

// Class makes sure to convert gameobjects to entities, we use this class
// so that we can define our rotation in degrees, and then convert it to radians 
public class NoiseElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float DegreesPerSecond = 360f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new NoiseElement { RadiansPerSecond = math.radians(DegreesPerSecond) });
    }
}
