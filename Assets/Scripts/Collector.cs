using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles the collector's health and taking in OVR thumbstick input for snap turning. */
public class Collector : MonoBehaviour {

    private Transform oVRCameraRigTransform;  // The transform of the OVRCameraRig
    private int health;                 // The current health of the collector
    private const int fullHealth = 1000;    // The health of the collector at the start of the scene
    private const float rotationRatchet = 45.0f;    // The Euler degrees each snap turn rotates the parent transform
    private bool readyToSnapTurn; // Set to true when a snap turn has occurred, code requires one frame of centered thumbstick to enable another snap turn

    /* Takes in a damage parameter and subtracts that from the health. */
    public void Damage(int damage) {
        health -= damage;
        if(health <= 0) {
            //
        }
    }

    private void Awake() {
        oVRCameraRigTransform = transform.parent.transform;
        health = fullHealth;
    }

    private void Update() {
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)) {
            if (readyToSnapTurn) {  // If ready to snap turn left
                Vector3 euler = oVRCameraRigTransform.rotation.eulerAngles;
                euler.y -= rotationRatchet;
                oVRCameraRigTransform.rotation = Quaternion.Euler(euler);
                readyToSnapTurn = false;
            }
        } else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) {
            if (readyToSnapTurn) {  // If ready to snap turn right
                Vector3 euler = oVRCameraRigTransform.rotation.eulerAngles;
                euler.y += rotationRatchet;
                oVRCameraRigTransform.rotation = Quaternion.Euler(euler);
                readyToSnapTurn = false;
            }
        } else if(!readyToSnapTurn)
            readyToSnapTurn = true;
    }

    private void OnTriggerEnter(Collider other) {
        other.gameObject.SetActive(false);  // Sets projectiles inactive after triggering the collector
    }

}
