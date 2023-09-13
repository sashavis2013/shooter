using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEject : MonoBehaviour
{
    public KeyCode ejectKey = KeyCode.Escape;
    FlyCamera _flyCam;
    public bool canLockIn=true;

    private void Awake() {
        _flyCam = GetComponent<FlyCamera>();
    }

    public void EjectMouse()
    {
        Cinemachine.CinemachineBrain cam = GetComponent<Cinemachine.CinemachineBrain>();
        if (cam) {
            cam.enabled = false;
        }

        CharacterAiming aiming = FindObjectOfType<CharacterAiming>();
        if (aiming) {
            aiming.enabled = false;
        }

        CharacterLocomotion locomotion = FindObjectOfType<CharacterLocomotion>();
        if (locomotion) {
            locomotion.enabled = false;
        }

        if (_flyCam) {
            _flyCam.enabled = true;
        }
        else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(ejectKey))
        {
            EjectMouse();
        }
        if (Input.GetMouseButtonDown(0)&&canLockIn) {
            Cinemachine.CinemachineBrain cam = GetComponent<Cinemachine.CinemachineBrain>();
            if (cam) {
                cam.enabled = true;
            }

            CharacterAiming aiming = FindObjectOfType<CharacterAiming>();
            if (aiming) {
                aiming.enabled = true;
            }

            CharacterLocomotion locomotion = FindObjectOfType<CharacterLocomotion>();
            if (locomotion) {
                locomotion.enabled = true;
            }

            if (_flyCam) {
                _flyCam.enabled = false;
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
