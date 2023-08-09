#if ADVANCED_SCENE_MANAGER

using LazyEvents;
using Unity.Netcode;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    /*
     * Instead of using RPC and Networkbehaviour and NetworkObject etc,
     * We make use of custom message.
     * I try to avoid NetworkObjects as much as I can.
     * 
     * This specific one is a oneshot, we just need to tell server we want a player object. Check SendMessageToServer.
     * Server then listens (see RemoteMessage method) and calls gamemanager of the request.
     * 
     * Before this however we need to make sure the active scene is correct. Since ASM is not present in Client.
     * 
     * Remember to dispose of RemoteMessage class, if you use mine.
     */

    private RemoteMessage<bool> message;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
            EventPlanner.AddListner("SynchronizeComplete", OnSynchronizeComplete); 
        else
            message = new RemoteMessage<bool>("ClientManager", RemoteMessage); // client dont need this one
    }

    private void RemoteMessage(bool data, ulong sender)
    {
        // should not reach clients, but just in case
        if (!NetworkManager.Singleton.IsServer) 
            return;
           
        GameManager.Instance.PlayerObjectRequest(sender);
        message.Dispose();
    }

    private void OnSynchronizeComplete(object data)
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsHost)
        {
            RemoteMessage<bool>.SendMessageToServer("ClientManager", true);

            EventPlanner.RemoveListner("SynchronizeComplete", OnSynchronizeComplete);
        }
    }
}

#endif