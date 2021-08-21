using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// Since we convert from GameObject to entity, we have to create a system for it instead of 
// generating an authoring component
public class SpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Prefab;
    public int CountX = 100;
    public int CountZ = 100;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            entity = conversionSystem.GetPrimaryEntity(Prefab),
            countX = CountX,
            countZ = CountZ,
        });
    }
}
