using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimation : MonoBehaviour {
    public float bounceSpeed = 8;
    public float bounceAmplitude = 0.05f;
    public float rotationSpeed = 90;

    private float _startingHeight;
    private float _timeOffset;
    private void Start() {
        _startingHeight = transform.localPosition.y;
        _timeOffset = Random.value * Mathf.PI * 2;
    }

    void Update() {
        // Bounce animation
        float finalHeight = _startingHeight + Mathf.Sin(Time.time * bounceSpeed + _timeOffset) * bounceAmplitude;
        var position = transform.localPosition;
        position.y = finalHeight;
        transform.localPosition = position;

        // Spin
        Vector3 rotation = transform.localRotation.eulerAngles;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
}