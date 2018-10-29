using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour {

    private AbilityHandler abilityHandlerScript;
    private List<GameObject> hitTicTacs;
    private const float explosionForce = 30.0f;
    private const float upwardsModifier = 5.0f;
    private const float delayTime = 1.5f;
    private bool triggered; // Guarantees the orb explosion is only triggered once

    public void InitializeVariables(GameObject abilityHandler) {
        abilityHandlerScript = abilityHandler.GetComponent<AbilityHandler>();
        hitTicTacs = new List<GameObject>();
        triggered = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (!triggered) {
            Vector3 hitPosition = V3E.SetY(transform.position, other.transform.position.y); // So explosion starts from inside the field rather
                                                                                            // than at the orb's height
            Collider[] hitColliders = Physics.OverlapSphere(hitPosition, transform.localScale.x / 2.0f, 1 << LayerMask.NameToLayer("TicTac"));
            foreach (Collider hitCollider in hitColliders) {
                Destroy(hitCollider.GetComponent<Animator>());
                Rigidbody hitRigidbody = hitCollider.GetComponent<Rigidbody>();
                hitRigidbody.isKinematic = false;
                hitRigidbody.useGravity = true;
                hitRigidbody.AddExplosionForce(explosionForce, hitPosition, transform.localScale.x / 2.0f, upwardsModifier);
                hitTicTacs.Add(hitRigidbody.transform.parent.gameObject);
            }
            StartCoroutine(RemoveTicTacs());
            triggered = true;
        }
    }

    private IEnumerator RemoveTicTacs() {
        yield return new WaitForSeconds(delayTime);
        foreach (GameObject ticTac in hitTicTacs) { Destroy(ticTac); }
        abilityHandlerScript.OrbRemoved();
        Destroy(gameObject);
    }

}
