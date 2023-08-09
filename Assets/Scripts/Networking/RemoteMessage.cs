#nullable enable

using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Make sure to dispose it after!
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class RemoteMessage<T> : IDisposable where T : unmanaged, IEquatable<T>
{
    private bool disposed;
    private readonly string key;
    public delegate void ReceiveCallbackDelegate(T data, ulong sender);
    private event ReceiveCallbackDelegate ReceiveCallbackEvent;
    private static CustomMessagingManager? CustomMessagingManager => 
        NetworkManager.Singleton?.CustomMessagingManager;

    public RemoteMessage(string key, ReceiveCallbackDelegate receiveCallback)
    {
        this.key = key;
        ReceiveCallbackEvent = receiveCallback;

        CustomMessagingManager?.RegisterNamedMessageHandler(key, ReceiveMessage);
    }

    private void ReceiveMessage(ulong senderClientId, FastBufferReader messagePayload)
    {
        messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<T> receivedMessageContent);

        ReceiveCallbackEvent.Invoke(receivedMessageContent.Value, senderClientId);
    }

    #region Sending

    public void SendMessageToClient(T value, ulong target) 
        => SendMessageToClient(key, value, target);
    public void SendMessageToServer(T value) 
        => SendMessageToServer(key, value);
    public void SendMessageToAllClients(T value)
        => SendMessageToAllClients(key, value);




    public static void SendMessageToServer(string key, T value) => 
        SendMessageTo(key, value, NetworkManager.ServerClientId);
    public static void SendMessageToClient(string key, T value, ulong ClientId) => 
        SendMessageTo(key, value, ClientId);
    private static void SendMessageTo(string key, T value, ulong target) => 
        Message(value, (writer) => CustomMessagingManager?.SendNamedMessage(key, target, writer, NetworkDelivery.Reliable));

    public static void SendMessageToAll(string key, T value)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            #if UNITY_EDITOR
            Debug.LogError("SendMessageToAll can only be sent from server");
            #endif

            return;
        }

        Message(value, (writer) => CustomMessagingManager?.SendNamedMessageToAll(key, writer));
    }
    public static void SendMessageToAllClients(string key, T value)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
#if UNITY_EDITOR
            Debug.LogError("SendMessageToAll can only be sent from server");
#endif
            return;
        }

        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SendMessageToClient(key, value, id);
        }
    }

    private static void Message(T value, Action<FastBufferWriter> action)
    {
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(value), Unity.Collections.Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        action(writer);
    }
    #endregion

    #region Disposing
    private void Dispose(bool disposing)
    {
        if (disposed) return;
        
        if (disposing)
            CustomMessagingManager?.UnregisterNamedMessageHandler(key);

        disposed = true;     
    }

    ~RemoteMessage()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}