using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SpawnerAuthoring_Boids : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Prefab;
    public int Count = 100;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,new Spawner_Boids
        {
            entity = conversionSystem.GetPrimaryEntity(Prefab),
            count = Count,
        });
    }
}
