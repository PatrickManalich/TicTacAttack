using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* An abstract class that all tic tacs must inherit from. Contains multiple abstract functions that
 * all tic tacs must override. Also contains protected abstract properties that all tic tacs must
 * override. */
public abstract class TicTac : MonoBehaviour {

    public abstract SE.Flavor Flavor { get; }   // Flavor property
    public abstract SE.PathRoute PathRoute { get; } // Path route property
    public abstract SE.PathDistance CurrPathDistance { get; set; }  // Path distance property

    private Base baseScript; // A reference to the Base script that instantiated this tic tac
    private Container containerScript; // A reference to the Container script that collected this tic tac
    private Animator animator;      // A reference to the animator in the child GameObject
    private const float minArrivalDistance = 0.005f;    // The minimum distance for a tic tac to arrive at its target

    protected abstract float Speed { get; } // A float property that dictates the tic tac's speed
    protected static Collector collectorScript;  // A reference to the collector GameObjects's script

    /* Takes in a base script reference and initializes its private variable. */
    public void InitializeVariables(GameObject baseGO, GameObject collector) {
        baseScript = baseGO.GetComponent<Base>();
        collectorScript = collector.GetComponent<Collector>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    /* Takes in a path position and delay time and calls the private coroutine "Advance". */
    public void StartAdvanceCoroutine(Vector3 pathPosition, float delayTime) {
        StopAllCoroutines();
        StartCoroutine(Advance(pathPosition, delayTime));
    }

    public void StartCollectCoroutine(GameObject container, Transform containerOpeningTransform, float collectSpeed, Transform containerTransform, Vector3 collectLocalPosition) {
        StopAllCoroutines();
        containerScript = container.GetComponent<Container>();
        StartCoroutine(Collect(containerOpeningTransform, collectSpeed, containerTransform, collectLocalPosition));
    }

    /* Abstract attack method to be implemented by children who inherit this class. */
    public abstract void Attack();

    /* Takes in a path position and delay time and moves towards that path position. Waits a delay time amount of
     * seconds and then begins attacking once the path position has been reached. */
    private IEnumerator Advance(Vector3 pathPosition, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        animator.SetTrigger("AdvanceTrigger");

        pathPosition = V3E.SetY(pathPosition, transform.position.y);    // Set height of path position to tic tac's height
        while (Vector3.Distance(transform.position, pathPosition) > minArrivalDistance) {
            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, pathPosition, step);
            yield return null;
        }
        transform.LookAt(V3E.SetY(collectorScript.transform.position, transform.position.y));
        if (CurrPathDistance == SE.PathDistance.Close)
            animator.SetTrigger("AttackTrigger");
        else
            animator.SetTrigger("IdleTrigger");
    }

    private IEnumerator Collect(Transform containerOpeningTransform, float collectSpeed, Transform containerTransform, Vector3 collectLocalPosition) {
        animator.SetTrigger("CollectTrigger");
        Remove();

        while (Vector3.Distance(transform.position, containerOpeningTransform.position) > minArrivalDistance) {
            float step = collectSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, containerOpeningTransform.position, step);
            yield return null;
        }
        animator.SetTrigger("ExitTrigger");
        transform.parent = containerTransform;
        transform.localPosition = collectLocalPosition * transform.GetChild(0).localScale.x;
        transform.localRotation = Quaternion.identity;
        containerScript.TicTacCollected(gameObject);
    }

    private void OnDestroy() {
        StopAllCoroutines();
        if(transform.parent == null)    // Only remove if the tic tac was collected, so Remove() doesn't get called twice
            Remove();
    }

    /* Notifies the base that this tic tac has been removed. */
    protected virtual void Remove() { baseScript.TicTacRemoved(PathRoute, CurrPathDistance, gameObject.GetInstanceID()); }

}