using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* An abstract class that all tic tacs must inherit from. Contains multiple abstract functions that
 * all tic tacs must override. Also contains protected abstract properties that all tic tacs must
 * override. */
public abstract class TicTac : MonoBehaviour {

    public abstract TicTacTag.Flavor Flavor { get; }   // Flavor property
    public abstract TicTacTag.PathRoute PathRoute { get; } // Path route property
    public abstract TicTacTag.PathDistance PathDistance { get; set; }  // Path distance property

    private static Base baseScript; // A reference to the Base script that instantiated this tic tac
    private Animator animator;      // A reference to the animator in the child game object
    private Vector3 collectorPosition;   // The position of the collector

    protected abstract float Speed { get; } // A float property that dictates the tic tac's speed

    /* Takes in a base script reference and initializes its private variable. */
    public void InitializeVariables(ref Base baseScript) {
        TicTac.baseScript = baseScript;
        animator = transform.GetChild(0).GetComponent<Animator>();
        collectorPosition = new Vector3(0.0f, transform.position.y, 0.0f);
    }

    /* Takes in a path position and delay time and calls the private coroutine "Advance". */
    public void StartAdvanceCoroutine(Vector3 pathPosition, float delayTime) {
        StopAllCoroutines();
        StartCoroutine(Advance(pathPosition, delayTime));
    }

    /* Abstract attack method to be implemented by children who inherit this class. */
    public abstract void Attack();

    /* Notifies the base that this tic tac has been removed and destroys itself. */
    public virtual void Remove() {
        baseScript.TicTacRemoved(PathRoute, PathDistance);
        Destroy(gameObject);
    }

    /* Takes in a path position and delay time and moves towards that path position. Waits a delay time amount of
     * seconds and then begins attacking once the path position has been reached. */
    private IEnumerator Advance(Vector3 pathPosition, float delayTime) {
        yield return new WaitForSeconds(delayTime);
        animator.SetTrigger("MovingTrigger");

        pathPosition = V3E.SetY(pathPosition, transform.position.y);    // Set height of path position to tic tac's height
        while (Vector3.Distance(transform.position, pathPosition) > 0.005f) {
            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, pathPosition, step);
            yield return null;
        }
        transform.LookAt(collectorPosition);
        if (PathDistance == TicTacTag.PathDistance.Close)
            animator.SetTrigger("AttackTrigger");
        else
            animator.SetTrigger("IdleTrigger");
    }

}