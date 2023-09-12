using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSocket : MonoBehaviour
{
    public MeshSockets.SocketId socketId;
    public HumanBodyBones bone;

    public Vector3 offset;
    public Vector3 rotation;

    Transform _attachPoint;
    
    void Start()
    {
        Animator animator = GetComponentInParent<Animator>();
        _attachPoint = new GameObject("socket" + socketId).transform;
        _attachPoint.SetParent(animator.GetBoneTransform(bone));
        _attachPoint.localPosition = offset;
        _attachPoint.localRotation = Quaternion.Euler(rotation);
    }

    public void Attach(Transform objectTransform) {
        objectTransform.SetParent(_attachPoint, false);
    }
}
