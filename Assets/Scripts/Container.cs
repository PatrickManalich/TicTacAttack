using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Takes in OVR index trigger input and updates the collect area. */
public class Container : MonoBehaviour {

    public GameObject collectAreaPrefab;  // The Prefab that marks the area of collection

    private GameObject collectArea;     // The instantiation of the collect area prefab
    private AbilityHandler abilityHandlerScript;
    private OVRInput.Button handTrigger;    // Either PrimaryHandTrigger for right or SecondaryHandTrigger for left
    private OVRInput.Button indexTrigger;   // Either PrimaryIndexTrigger for right or SecondaryIndexTrigger for left
    private Transform containerOpeningTransform;    // The transform of the container opening
    private Transform containerTransform;   // The transform of the container
    private static readonly Vector3 smallCollectAreaLocalScale = new Vector3(0.2f, 0.07f, 0.2f); // Local scale for small collect area
    private static readonly Vector3 largeCollectAreaLocalScale = new Vector3(0.4f, 0.07f, 0.4f); // Local scale for large collect area
    private bool updateCollectArea; // If true, will update the position of the collect area
    private const float collectSpeed = 2.0f;    // The speed that the tic tac is collected at
    private int collectedTicTacCount;   // How many tic tacs have currently been collected. Can't use collectedTicTacs.Count because
                                        // the tic tacs have to arrive to the container first before being added to the list
    private List<GameObject> collectedTicTacs; // A list of the collected tic tacs
    private Queue<Vector3> collectLocalPositions;   // A queue of where tic tacs will be placed locally inside the container
    private bool usingAbility;


    public void AbilityFinished() {
        collectedTicTacCount = 0;
        collectedTicTacs.Clear();
        foreach (Transform child in transform) {
            if (child.name.Contains(name))  // If the child's name has "Container" in it, then it won't be destroyed
                child.gameObject.SetActive(true);   // Enable container children
            else
                Destroy(child.gameObject);
        }
        usingAbility = false;
    }

    public void TicTacCollected(GameObject collectedTicTac) { collectedTicTacs.Add(collectedTicTac); }

    private void Awake() {
        collectArea = Instantiate(collectAreaPrefab);
        abilityHandlerScript = GetComponent<AbilityHandler>();

        if (string.Equals(transform.parent.name, "LeftHandAnchor")) {   // Initialize hand and index triggers
            handTrigger = OVRInput.Button.PrimaryHandTrigger;
            indexTrigger = OVRInput.Button.PrimaryIndexTrigger;
        } else if (string.Equals(transform.parent.name, "RightHandAnchor")) {
            handTrigger = OVRInput.Button.SecondaryHandTrigger;
            indexTrigger = OVRInput.Button.SecondaryIndexTrigger;
        } else
            Debug.LogWarning("Container GameObject isn't parented correctly with OVRCameraRig");

        foreach(Transform child in transform) { // Initialize container opening transform
            if (string.Equals(child.name, "ContainerOpening"))
                containerOpeningTransform = child;
        }
        if (!containerOpeningTransform)
            Debug.LogWarning("Container GameObject doesn't have a container opening under it");
        containerTransform = transform;
        collectArea.transform.localScale = smallCollectAreaLocalScale;
        collectArea.SetActive(false);
        updateCollectArea = false;

        collectedTicTacCount = 0;
        collectedTicTacs = new List<GameObject>();  // Initialize collected tic tacs

        collectLocalPositions = new Queue<Vector3>();       // Initialize collect local positions
        collectLocalPositions.Enqueue(new Vector3(-1, -1));
        collectLocalPositions.Enqueue(new Vector3(0, -1));
        collectLocalPositions.Enqueue(new Vector3(1, -1));
        collectLocalPositions.Enqueue(new Vector3(-1, 1));
        collectLocalPositions.Enqueue(new Vector3(0, 1));
        collectLocalPositions.Enqueue(new Vector3(1, 1));

        usingAbility = false;
    }

    private void Update() {
        if (!usingAbility) {
            if (OVRInput.GetDown(handTrigger)) {   // When first pressing hand trigger
                if (collectedTicTacs.Count < collectLocalPositions.Count) {   // Collecting
                    updateCollectArea = true;
                    collectArea.SetActive(true);
                } else {  // Activating ability
                    abilityHandlerScript.ActivateAbility(collectedTicTacs);
                    usingAbility = true;
                }
            } else if (OVRInput.GetUp(handTrigger)) {  // When letting go of hand trigger
                if (collectedTicTacs.Count < collectLocalPositions.Count) {  // Collecting
                    Collider[] hitColliders = Physics.OverlapSphere(collectArea.transform.position, collectArea.transform.localScale.x / 2.0f, 1 << LayerMask.NameToLayer("TicTac"));
                    for (int i = 0; i < hitColliders.Length && collectedTicTacCount < collectLocalPositions.Count; i++) {
                        hitColliders[i].gameObject.layer = LayerMask.NameToLayer("Default");
                        Vector3 collectLocalPosition = collectLocalPositions.Dequeue();
                        collectLocalPositions.Enqueue(collectLocalPosition);
                        TicTac ticTacScript = hitColliders[i].transform.parent.gameObject.GetComponent<TicTac>();
                        ticTacScript.StartCollectCoroutine(gameObject, containerOpeningTransform, collectSpeed, containerTransform, collectLocalPosition);
                        collectedTicTacCount++;
                    }
                    collectArea.SetActive(false);
                    updateCollectArea = false;
                }
            }

            if (OVRInput.GetDown(indexTrigger)) {   // When first pressing index trigger
                if (collectedTicTacs.Count < collectLocalPositions.Count)   // Collecting
                    collectArea.transform.localScale = collectArea.transform.localScale == smallCollectAreaLocalScale ? largeCollectAreaLocalScale : smallCollectAreaLocalScale;
                else {  // Activating ability
                    abilityHandlerScript.ActivateAbility(collectedTicTacs);
                    usingAbility = true;
                }    
            }


            if (updateCollectArea) {
                RaycastHit raycastHit;
                if (Physics.Raycast(containerOpeningTransform.position, containerOpeningTransform.up, out raycastHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Field"))) {
                    collectArea.transform.position = raycastHit.point;
                }
            }

        }
    }
}
