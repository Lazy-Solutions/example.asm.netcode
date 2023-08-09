using System;
using UnityEngine;

public class CrosshairTarget : Singleton<CrosshairTarget>
{
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private LayerMask layerMask;

    private Vector3 cachedPoint;
    private GameObject cachedGameObject;
    private int lastFrame;

    public Vector3 GetTargetPoint()
    {
        if (lastFrame != Time.frameCount)
            RayTarget();

        return cachedPoint;
    }

    public GameObject GetTargetGameObject()
    {
        if (lastFrame != Time.frameCount)
            RayTarget();

        return cachedGameObject;
    }

    private void RayTarget()
    {
        Vector2 point = crosshair.position;
        Ray ray = Camera.main.ScreenPointToRay(point);

        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, 100f, layerMask);

        cachedPoint = (didHit) ? hit.point : ray.GetPoint(100f);
        cachedGameObject = (didHit) ? hit.collider.gameObject : null;
        lastFrame = Time.frameCount;
    }

}
