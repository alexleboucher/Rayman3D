using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] Transform origin;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundDistanceSafeValue = 0.1f;

    RaycastHit? hitInfo;

    void Update()
    {
        if (Physics.Raycast(origin.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
            this.hitInfo = hitInfo;
        else
            this.hitInfo = null;
    }

    public bool IsGrounded(out RaycastHit? hitInfo)
    {
        hitInfo = this.hitInfo;
        return hitInfo.HasValue && hitInfo.Value.distance <= groundDistanceSafeValue;
    }
}
