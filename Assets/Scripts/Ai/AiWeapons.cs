using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with weapon-related AI methods
/// </summary>
public class AiWeapons : MonoBehaviour
{
    public enum WeaponState {
        Holstering,
        Holstered,
        Activating,
        Active,
        Reloading
    }

    public enum WeaponSlot {
        Primary,
        Secondary
    }

    public RaycastWeapon CurrentWeapon {
        get {
            return _weapons[_current];
        }
    }

    public WeaponSlot CurrentWeaponSlot {
        get {
            return (WeaponSlot)_current;
        }
    }
    RaycastWeapon[] _weapons = new RaycastWeapon[2];
    int _current = 0;
    Animator _animator;
    MeshSockets _sockets;
    WeaponIk _weaponIk;
    Transform _currentTarget;
    WeaponState _weaponState = WeaponState.Holstered;
    public float inaccuracy = 0.0f;
    public float dropForce = 1.5f;
    GameObject _magazineHand;

    public bool IsActive() {
        return _weaponState == WeaponState.Active;
    }

    public bool IsHolstered() {
        return _weaponState == WeaponState.Holstered;
    }

    public bool IsReloading() {
        return _weaponState == WeaponState.Reloading;
    }

    private void Awake() {
        _animator = GetComponent<Animator>();
        _sockets = GetComponent<MeshSockets>();
        _weaponIk = GetComponent<WeaponIk>();
    }

    private void Update() {
        if (_currentTarget && CurrentWeapon && IsActive()) {
            Vector3 target = _currentTarget.position + _weaponIk.targetOffset;
            target += Random.insideUnitSphere * inaccuracy;
            CurrentWeapon.UpdateWeapon(Time.deltaTime, target);
        }
    }

    public void SetFiring(bool enabled) {
        if (enabled) {
            CurrentWeapon.StartFiring();
        } else {
            CurrentWeapon.StopFiring();
        }
    }

    public void DropWeapon() {
        if (CurrentWeapon) {
            CurrentWeapon.transform.SetParent(null);
            CurrentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            CurrentWeapon.gameObject.AddComponent<Rigidbody>();
            _weapons[_current] = null;
        }
    }

    public bool HasWeapon() {
        return CurrentWeapon != null;
    }

    public void SetTarget(Transform target) {
        _weaponIk.SetTargetTransform(target);
        _currentTarget = target;
    }

    public void Equip(RaycastWeapon weapon) {
        _current = (int)weapon.weaponSlot;
        _weapons[_current] = weapon;
        _sockets.Attach(weapon.transform, weapon.holsterSocket);
    }

    public void ActivateWeapon() {
        StartCoroutine(EquipWeaponAnimation());
    }

    public void DeactivateWeapon() {
        SetTarget(null);
        SetFiring(false);
        StartCoroutine(HolsterWeaponAnimation());
    }

    public void ReloadWeapon() {
        if (IsActive()) {
            StartCoroutine(ReloadWeaponAnimation());
        }
    }

    public void SwitchWeapon(WeaponSlot slot) {
        if (_weapons[(int)slot] == null) {
            return;
        }

        if (IsHolstered()) {
            _current = (int)slot;
            ActivateWeapon();
            return;
        }

        int equipIndex = (int)slot;
        if (IsActive() && _current != equipIndex) {
            StartCoroutine(SwitchWeaponAnimation(equipIndex));
        }
    }

    public int Count() {
        int count = 0;
        foreach (var weapon in _weapons) {
            if (weapon != null) {
                count++;
            }
        }
        return count;
    }

    IEnumerator EquipWeaponAnimation() {
        _weaponState = WeaponState.Activating;
        _animator.runtimeAnimatorController = CurrentWeapon.animator;
        _animator.SetBool("equip", true);
        yield return new WaitForSeconds(0.5f);
        while(_animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f) {
            yield return null;
        }

        _weaponIk.enabled = true;
        _weaponIk.SetAimTransform(CurrentWeapon.raycastOrigin);
        _weaponState = WeaponState.Active;
    }

    IEnumerator HolsterWeaponAnimation() {
        _weaponState = WeaponState.Holstering;
        _animator.SetBool("equip", false);
        _weaponIk.enabled = false;
        yield return new WaitForSeconds(0.5f);
        while (_animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f) {
            yield return null;
        }

        _weaponState = WeaponState.Holstered;
    }

    IEnumerator ReloadWeaponAnimation() {
        _weaponState = WeaponState.Reloading;
        _animator.SetTrigger("reload_weapon");
        _weaponIk.enabled = false;
        yield return new WaitForSeconds(0.5f);
        while (_animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f) {
            yield return null;
        }

        _weaponIk.enabled = true;
        _weaponState = WeaponState.Active;
    }

    IEnumerator SwitchWeaponAnimation(int index) {
        yield return StartCoroutine(HolsterWeaponAnimation());
        _current = index;
        yield return StartCoroutine(EquipWeaponAnimation());
    }

    public void OnAnimationEvent(string eventName) {
        switch (eventName) {
            case "attach_weapon":
                AttachWeapon();
                break;
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }

    void AttachWeapon() {
        bool equipping = _animator.GetBool("equip");
        if (equipping) {
            _sockets.Attach(CurrentWeapon.transform, MeshSockets.SocketId.RightHand);
        } else {
            _sockets.Attach(CurrentWeapon.transform, CurrentWeapon.holsterSocket);
        }
    }

    void DetachMagazine() {
        var leftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
        RaycastWeapon weapon = CurrentWeapon;
        _magazineHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }

    void DropMagazine() {
        GameObject droppedMagazine = Instantiate(_magazineHand, _magazineHand.transform.position, _magazineHand.transform.rotation);
        droppedMagazine.SetActive(true);
        Rigidbody body = droppedMagazine.AddComponent<Rigidbody>();

        Vector3 dropDirection = -gameObject.transform.right;
        dropDirection += Vector3.down;

        body.AddForce(dropDirection * dropForce, ForceMode.Impulse);
        droppedMagazine.AddComponent<BoxCollider>();
        _magazineHand.SetActive(false);
    }

    void RefillMagazine() {
        _magazineHand.SetActive(true);
    }

    void AttachMagazine() {
        RaycastWeapon weapon = CurrentWeapon;
        weapon.magazine.SetActive(true);
        Destroy(_magazineHand);
        weapon.RefillAmmo();
        _animator.ResetTrigger("reload_weapon");
    }

    public void RefillAmmo(int clipCount) {
        var weapon = CurrentWeapon;
        if (weapon) {
            weapon.clipCount += clipCount;
        }
    }

    public bool IsLowAmmo() {
        var weapon = CurrentWeapon;
        if (weapon) {
            return weapon.IsLowAmmo();
        }
        return false;
    }
}
