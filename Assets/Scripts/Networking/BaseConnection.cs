using Unity.Netcode;
using LazyEvents;

/*
 * Connection classes contains some of the used events when connecting / disconnecting
 * Host and client has different ways to handle it aswell.
 * I would use this only for client based events, and treat host as a client aswell
 * then use another class for server specific things, like when client connects.
 */

public abstract class BaseConnection
{
    public virtual void OnConnected(ulong clientId) 
        => EventPlanner.Invoke("OnConnected");
    public virtual void OnDisconnected(ulong clientId) 
        => EventPlanner.Invoke("OnDisconnected");
    public virtual void OnBeginSynchronize(ulong clientId)
        => EventPlanner.Invoke("OnBeginSynchronize");
    public virtual void OnSynchronizeComplete(ulong clientId)
        => EventPlanner.Invoke("OnSynchronized");
}

public class ClientConnection : BaseConnection
{
    public bool IsSynchronized { get; set; }

    public override void OnConnected(ulong _)
    {
        //this should only be called once on client
        base.OnConnected(_);
    }
    public override void OnDisconnected(ulong _)
    {
        //this should only be called once on client
        base.OnDisconnected(_);
    }
    public override void OnBeginSynchronize(ulong _)
    {
        IsSynchronized = false;
        base.OnBeginSynchronize(_);
    }
    public override void OnSynchronizeComplete(ulong _)
    {
        IsSynchronized = true;
        base.OnSynchronizeComplete(_);
    }

}

public class HostConnection : BaseConnection
{
    public override void OnConnected(ulong clientId)
    {
        // this is so the host gets the event for itself as a client
        if (clientId == NetworkManager.ServerClientId)
        {
            base.OnConnected(clientId);
        }
    }

    public override void OnDisconnected(ulong clientId)
    {
        // this is so the host gets the event for itself as a client
        if (clientId == NetworkManager.ServerClientId)
        {
            base.OnDisconnected(clientId);
        }
    }

    // Events we don't care about for host, in this example, overriding to stop the events.
    // server will get this event aswell when the clients has finished their sync.
    public override void OnBeginSynchronize(ulong _){}
    public override void OnSynchronizeComplete(ulong _){ }
}