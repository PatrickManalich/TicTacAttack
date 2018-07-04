using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Takes in OVR index trigger input and updates the collect area. */
public class Container : MonoBehaviour {

    public GameObject collectArea;  // The GameObject that marks the area of collection

    private OVRInput.Button indexTrigger;   // Either PrimaryIndexTrigger for right or SecondaryIndexTrigger for left
    private Transform containerOpeningTransform;    // The transform of the container opening
    private bool updateCollectArea; // If true, will update the position of the collect area

    private void Awake() {
        if (string.Equals(transform.parent.name, "LeftHandAnchor")) // Initialize index trigger
            indexTrigger = OVRInput.Button.PrimaryIndexTrigger;
        else if (string.Equals(transform.parent.name, "RightHandAnchor"))
            indexTrigger = OVRInput.Button.SecondaryIndexTrigger;
        else
            Debug.LogWarning("Container GameObject isn't parented correctly with OVRCameraRig");

        foreach(Transform child in transform) { // Initialize container opening transform
            if (string.Equals(child.name, "ContainerOpening"))
                containerOpeningTransform = child;
        }
        if (!containerOpeningTransform)
            Debug.LogWarning("Container GameObject doesn't have a container opening under it");

        collectArea.SetActive(false);
        updateCollectArea = false;
    }

    private void Update() {
        if (OVRInput.GetDown(indexTrigger)) {   // When first pressing index trigger
            updateCollectArea = true;
            collectArea.SetActive(true);
        } else if (OVRInput.GetUp(indexTrigger)) {  // When letting go of index trigger
            Collider[] hitColliders = Physics.OverlapSphere(collectArea.transform.position, collectArea.transform.localScale.x / 2.0f, 1 << LayerMask.NameToLayer("TicTac"));
            foreach(Collider hitCollider in hitColliders) {
                hitCollider.gameObject.SetActive(false);
            }
            collectArea.SetActive(false);
            updateCollectArea = false;
        }

        if (updateCollectArea) {
            RaycastHit hit;
            if (Physics.Raycast(containerOpeningTransform.position, containerOpeningTransform.up, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Field"))) {
                collectArea.transform.position = hit.point;
            }
        }
            
    }

}
