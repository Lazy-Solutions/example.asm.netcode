using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ClientPlayerEnabler : NetworkBehaviour
{
    public UnityEvent OnOwnerControl;
    public UnityEvent OnOwnerLostControl;

    private void Start()
    {
        if(IsOwner)
            OnOwnerControl.Invoke();
        else
            OnOwnerLostControl.Invoke();
    }

    public override void OnGainedOwnership()
    {
        OnOwnerControl.Invoke();
        base.OnGainedOwnership();
    }

    public override void OnLostOwnership()
    {
        OnOwnerLostControl.Invoke();
        base.OnLostOwnership();
    }   
}
