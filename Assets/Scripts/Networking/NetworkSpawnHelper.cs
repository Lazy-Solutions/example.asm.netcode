using UnityEngine;
using Unity.Netcode;

public static class NetworkSpawnHelper
{
    public static GameObject SpawnObject(GameObject prefab, bool destroywithscene = true)
        => SpawnObjectBase(prefab, Vector3.zero, Quaternion.identity, null, null, destroywithscene);

    public static GameObject SpawnObject(GameObject prefab, Transform parent, bool destroywithscene = true)
        => SpawnObjectBase(prefab, Vector3.zero, Quaternion.identity, null, parent, destroywithscene);

    public static GameObject SpawnObject(GameObject prefab, ulong? clientId, Transform parent, bool destroywithscene = true) 
        => SpawnObjectBase(prefab, Vector3.zero, Quaternion.identity, clientId, parent, destroywithscene);

    public static GameObject SpawnObjectBase(GameObject prefab, Vector3 position, Quaternion rotation, ulong? clientId, Transform parent, bool destroywithscene = true) {
        
        if (!NetworkManager.Singleton.IsServer)
        {
            #if UNITY_EDITOR
            Debug.LogError("Spawn must not be called on client!");
            #endif
            return null;
        }

        GameObject go = Object.Instantiate(prefab, position, rotation, parent);

        if(clientId == null)
            go.GetComponent<NetworkObject>().Spawn(destroywithscene);
        else
            go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId.Value, destroywithscene);

        return go;
    }

    
}
