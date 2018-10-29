using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour {

    public GameObject orbPrefab;    // The Prefab for the orb
    public Material orangeMaterial;
    public Material mintMaterial;
    public Material peachAndPassionFruitMaterial;
    public Material spearmintMaterial;

    private Container containerScript;
    private int orbsAlive;
    private OVRInput.Button handTrigger;    // Either PrimaryHandTrigger for right or SecondaryHandTrigger for left
    private OVRInput.Button indexTrigger;   // Either PrimaryIndexTrigger for right or SecondaryIndexTrigger for left
    // Orange orb relevant variables
    private static readonly Vector3 orangeOrbLocalPosition = new Vector3(0.0f, 1.25f, 0.0f); // Local position for the orange orb
    private static readonly Vector3 orangeOrbLocalScale = new Vector3(0.4f, 0.4f, 0.4f); // Local scale for the orange orb
    // Mint orb relevant variables
    private const int mintOrbLimit = 3;
    private const float mintOrbSpeed = 10.0f;
    private const float mintOrbArc = 5.0f;
    private static readonly Vector3 mintOrbLocalScale = new Vector3(0.15f, 0.15f, 0.15f); // Local scale for the mint orb
    private Queue<GameObject> mintOrbs;
    // Peach and passion fruit orb relevant variables
    private static readonly Vector3 peachAndPassionFruitOrbPosition = new Vector3(0.0f, 4.0f, 0.0f); // Position for the 
                                                                                                              // peach and passion fruit orb
    private static readonly Vector3 peachAndPassionFruitOrbLocalScale = new Vector3(1.0f, 1.0f, 1.0f); // Local scale for the 
                                                                                                              // peach and passion fruit orb
    // Spearmint orb relevant variables
    private const int spearmintOrbLimit = 6;
    private static readonly Vector3 spearmintOrbLocalScale = new Vector3(0.15f, 0.15f, 0.15f); // Local scale for the spearmint orb


    public void ActivateAbility(List<GameObject> collectedTicTacs) {
        foreach (Transform child in transform) { child.gameObject.SetActive(false); }    // Hide container children

        SE.Flavor? firstFlavor = null;
        // If all the tic tacs you collected were of the same flavor, you will double the orb in size
        float abilityMultiplier = 1.0f;
        float abilityMultiplierIncrement = 1.0f / (float)(collectedTicTacs.Count - 1);

        foreach (GameObject collectedTicTac in collectedTicTacs) {   // Getting ability multiplier
            SE.Flavor collectedTicTacFlavor = collectedTicTac.GetComponent<TicTac>().Flavor;
            if (firstFlavor == null)
                firstFlavor = collectedTicTacFlavor;
            else if (collectedTicTacFlavor == firstFlavor)
                abilityMultiplier += abilityMultiplierIncrement;
        }

        switch (firstFlavor) {
            case SE.Flavor.Orange:
                ActivateOrangeAbility(abilityMultiplier);
                break;
            case SE.Flavor.Mint:
                ActivateMintAbility(abilityMultiplier);
                break;
            case SE.Flavor.PeachAndPassionFruit:
                ActivatePeachAndPassionFruitAbility(abilityMultiplier);
                break;
            case SE.Flavor.Spearmint:
                //
                break;
            default:
                Debug.LogWarning("Unknown first flavor");
                break;
        }
    }

    public void OrbRemoved() {
        orbsAlive--;
        if (orbsAlive == 0)
            containerScript.AbilityFinished();
    }

    private void Awake() {
        containerScript = GetComponent<Container>();
        orbsAlive = 0;
        mintOrbs = new Queue<GameObject>();

        if (string.Equals(transform.parent.name, "LeftHandAnchor")) {   // Initialize hand and index triggers
            handTrigger = OVRInput.Button.PrimaryHandTrigger;
            indexTrigger = OVRInput.Button.PrimaryIndexTrigger;
        } else if (string.Equals(transform.parent.name, "RightHandAnchor")) {
            handTrigger = OVRInput.Button.SecondaryHandTrigger;
            indexTrigger = OVRInput.Button.SecondaryIndexTrigger;
        } else
            Debug.LogWarning("Container GameObject isn't parented correctly with OVRCameraRig");
    }

    private void Update() {
        if(mintOrbs.Count > 0) {
            if (OVRInput.GetDown(handTrigger) || OVRInput.GetDown(indexTrigger)) {   // When first pressing hand or index trigger
                GameObject mintOrb = mintOrbs.Dequeue();
                mintOrb.transform.parent = null;
                Rigidbody mintOrbRigidBody = mintOrb.GetComponent<Rigidbody>();
                mintOrbRigidBody.isKinematic = false;
                mintOrbRigidBody.useGravity = true;
                mintOrbRigidBody.velocity = transform.parent.forward * mintOrbSpeed + Vector3.up * mintOrbArc;
            }
        }
    }

    private void ActivateOrangeAbility(float abilityMultiplier) {
        GameObject orangeOrb = Instantiate(orbPrefab);
        orangeOrb.GetComponent<Rigidbody>().isKinematic = true;
        orangeOrb.transform.parent = transform;
        orangeOrb.transform.localPosition = orangeOrbLocalPosition;
        orangeOrb.transform.localRotation = Quaternion.identity;
        orangeOrb.transform.localScale = orangeOrbLocalScale * abilityMultiplier;
        orangeOrb.GetComponent<Renderer>().material = orangeMaterial;
        orangeOrb.GetComponent<Orb>().InitializeVariables(gameObject);
        orbsAlive++;
    }

    private void ActivateMintAbility(float abilityMultiplier) {
        for(int i = 0; i < mintOrbLimit; i++) {
            GameObject mintOrb = Instantiate(orbPrefab);
            mintOrb.GetComponent<Rigidbody>().isKinematic = true;
            mintOrb.transform.parent = transform;
            mintOrb.transform.localPosition = Vector3.zero;
            mintOrb.transform.localRotation = Quaternion.identity;
            mintOrb.transform.localScale = mintOrbLocalScale * abilityMultiplier;
            mintOrb.GetComponent<Renderer>().material = mintMaterial;
            mintOrb.GetComponent<Orb>().InitializeVariables(gameObject);
            mintOrbs.Enqueue(mintOrb);
            orbsAlive++;
        }
    }

    private void ActivatePeachAndPassionFruitAbility(float abilityMultiplier) {
        GameObject peachAndPassionFruitOrb = Instantiate(orbPrefab);
        Rigidbody peachAndPassionFruitOrbRigidBody = peachAndPassionFruitOrb.GetComponent<Rigidbody>();
        peachAndPassionFruitOrbRigidBody.isKinematic = false;
        peachAndPassionFruitOrbRigidBody.useGravity = true;
        peachAndPassionFruitOrb.transform.position = peachAndPassionFruitOrbPosition;
        peachAndPassionFruitOrb.transform.rotation = Quaternion.identity;
        peachAndPassionFruitOrb.transform.localScale = peachAndPassionFruitOrbLocalScale * abilityMultiplier;
        peachAndPassionFruitOrb.GetComponent<Renderer>().material = peachAndPassionFruitMaterial;
        peachAndPassionFruitOrb.GetComponent<Orb>().InitializeVariables(gameObject);
        orbsAlive++;
    }

    private void ActivateSpearmintAbility(float abilityMultiplier) {
        for (int i = 0; i < spearmintOrbLimit; i++) {
            GameObject spearmintOrb = Instantiate(orbPrefab);
            Rigidbody spearmintOrbRigidbody = spearmintOrb.GetComponent<Rigidbody>();
            spearmintOrbRigidbody.isKinematic = false;
            spearmintOrbRigidbody.useGravity = true;
            spearmintOrb.transform.position = Vector3.zero;
            spearmintOrb.transform.rotation = Quaternion.identity;
            spearmintOrb.transform.localScale = spearmintOrbLocalScale * abilityMultiplier;
            spearmintOrb.GetComponent<Renderer>().material = spearmintMaterial;
            spearmintOrb.GetComponent<Orb>().InitializeVariables(gameObject);
            //spearmintOrbRigidbody.
            orbsAlive++;
        }
    }
}
