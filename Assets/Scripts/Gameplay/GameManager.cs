using System.Collections.Generic;
using System.Linq;
using AdvancedSceneManager.Callbacks;
using AdvancedSceneManager.Models;
using Unity.Netcode;
using UnityEngine;

public class GameManager : Singleton<GameManager>, ICollectionOpen
{
    // For debugging purposes.
    [SerializeField] private bool SpawnPlayers = true;
    private bool wasInitialized = false;
    public IGameMode GameMode { get => GameSettings.Instance.GameMode; }

    [SerializeField] private GameObject playerPrefab;
    private readonly List<NetworkObject> playersObjects = new();

    protected override void Awake()
    {
        // Our singleton needs this to register
        base.Awake();
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollectionOpen(SceneCollection collection)
    {

        if (!SpawnPlayers || wasInitialized) return;

        SpawnPlayerObjects();

        playersObjects[0].gameObject.SetActive(true);
        playersObjects[0].SpawnAsPlayerObject(NetworkManager.ServerClientId);

        wasInitialized = true;
    }

    // We spawn max amount of player objects on server side then hide them
    // This in theory, would make it easier to handle reconnect or player swap.
    private void SpawnPlayerObjects()
    {
        for (int i = 0; i < GameMode.MaxPlayers; i++)
        {
            var go = Instantiate(playerPrefab);
            playersObjects.Add(go.GetComponent<NetworkObject>());
            go.gameObject.transform.position = new Vector3(i, 0, 0);
            go.SetActive(false);
        }
    }

    public void PlayerObjectRequest(ulong uid)
    {
        if (!SpawnPlayers) return;

        // grabbing the index, we know we will have max amount of player and spawned x amount
        int index = NetworkManager.Singleton.ConnectedClientsList.ToList().FindIndex(x => x.ClientId == uid);

        if (index < 0)
        {
            Debug.Log("Error spawning player, id incorrect");
            return;
        }

        NetworkObject playerObject = playersObjects[index];

        if (playerObject.OwnerClientId == uid)
            return;

        if (playerObject.IsSpawned)
        {
            playerObject.ChangeOwnership(uid);
            return;
        }

        playerObject.gameObject.SetActive(true);
        playerObject.SpawnAsPlayerObject(uid);
    }

}
