using System;
using System.Collections;
using System.Linq;
using AdvancedSceneManager;
using AdvancedSceneManager.Models;
using LazyEvents;
using Unity.Netcode;

public class ConnectionManager : Singleton<ConnectionManager>
{
    public BaseConnection Connection { get; private set; }

    private readonly ClientConnection clientConnection = new();
    private readonly HostConnection hostConnection = new();


    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    protected override void OnDestroy()
    {
        //For singleton
        base.OnDestroy();

        if (!NetworkManager.Singleton) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void OnClientConnect(ulong clientId)
    {
        NetworkManager.Singleton.SceneManager.OnSynchronize += OnSynchronize;
        NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;

        Connection?.OnConnected(clientId);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        NetworkManager.Singleton.SceneManager.OnSynchronize -= OnSynchronize;
        NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;

        Connection?.OnDisconnected(clientId);
    }

    private void OnSynchronize(ulong clientId)
    => Connection?.OnBeginSynchronize(clientId);
    private void OnSynchronizeComplete(ulong clientId)
        => Connection?.OnSynchronizeComplete(clientId);



    public void StartClient()
    {
        Connection = clientConnection;
        SceneTransitionHandler.Instance.Transition(ClientSequence(v =>
        {
            if (!v)
                throw new Exception("Failed to connect");
        }), Profile.current.loadingScreen);
    }

    public void StartHost()
    {
        Connection = hostConnection;
        SceneTransitionHandler.Instance.Transition(HostSequence(v =>
        {
            if (!v)
                throw new Exception("Failed to host");
        }), Profile.current.loadingScreen);
    }

    IEnumerator ClientSequence(Action<bool> connectionCallback)
    {
        Scene[] openScenes = SceneManager.openScenes.ToArray();

        yield return SceneManager.runtime.Close(openScenes.Where(x => !x.isLoadingScreen && !x.isPersistent));

        bool connected = NetworkManager.Singleton.StartClient();

        if (!connected)
        {
            connectionCallback.Invoke(false);
            yield break;
        }

        NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += SceneManager_OnSynchronizeComplete;

        bool canContinue = false;

        while (!canContinue)
            yield return null;

        NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= SceneManager_OnSynchronizeComplete;

        connectionCallback.Invoke(true);

        EventPlanner.Invoke("SynchronizeComplete");

        void SceneManager_OnSynchronizeComplete(ulong cliendId)
        {
            canContinue = true;
        }
    }

    IEnumerator HostSequence(Action<bool> connectionCallback)
    {
        if (!NetworkManager.Singleton.StartHost())
        {
            connectionCallback.Invoke(false);
            yield break;
        }

        yield return GameSettings.Instance.LoadLevel.Open();

        connectionCallback.Invoke(true);
    }


}
