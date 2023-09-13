using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : Health
{
    public float dieForce = 15.0f;
    Ragdoll _ragdoll;
    ActiveWeapon _weapons;
    CharacterAiming _aiming;
    VolumeProfile _postProcessing;
    CameraManager _cameraManager;

    protected override void OnStart() {
        _ragdoll = GetComponent<Ragdoll>();
        _weapons = GetComponent<ActiveWeapon>();
        _aiming = GetComponent<CharacterAiming>();
        _postProcessing = FindObjectOfType<Volume>().profile;
        _cameraManager = FindObjectOfType<CameraManager>();
        UpdateVignette();
    }

    protected override void OnDeath(Vector3 direction) {
        _ragdoll.ActivateRagdoll();
        direction.Normalize();
        direction.y = 1.0f;
        _ragdoll.ApplyForce(direction * dieForce);
        _weapons.DropWeapon();
        _aiming.enabled = false;
        _cameraManager.EnableKillCam();
        GameEventsManager.Instance.PlayerDeath();
        MouseEject mouseEject = FindObjectOfType<MouseEject>(); //a little work-around
        mouseEject.EjectMouse();
        mouseEject.canLockIn = false;
    }

    protected override void OnDamage(Vector3 direction) {
        UpdateVignette();
    }

    protected override void OnHeal(float amount) {
        UpdateVignette();
    }

    private void UpdateVignette() {
        if (_postProcessing.TryGet(out Vignette vignette)) {
            float percent = 1.0f - (currentHealth / maxHealth);
            vignette.intensity.value = percent * 0.6f;
        }
    }
}
