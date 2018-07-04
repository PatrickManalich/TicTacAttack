using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Spawns and initializes tic tacs. Keeps track of two paths, a melee path and a ranged path. Manages tic tacs 
 * when they are destroyed and updates paths. */
public class Base : MonoBehaviour {

    public List<GameObject> ticTacPrefabs;   // A list of tic tac prefabs that will be cloned when spawning
    public GameObject meleePathGO;          // The empty GameObject which holds a close, mid, and far cylinder
                                            // GameObject. Used for setting up the melee path
    public GameObject rangedPathGO;         // The empty GameObject which holds a close, mid, and far cylinder
                                            // GameObject. Used for setting up the ranged path
    
    private class Path {    // Encapsulates all necessary information for a path
        public int count = 0;   // The number of tic tacs currently on this path
        public float[] spawnRadii = new float[3];   // The spawn radii for close, mid, and far
        public Vector3[] spawnCenters = new Vector3[3]; // The spawn centers for close, mid, and far
        public Queue<TicTac> midEnemyScripts = new Queue<TicTac>();   // Queue of tic tac scripts on mid
        public Queue<TicTac> farEnemyScripts = new Queue<TicTac>();   // Queue of tic tac scripts on far
    }
    private static Base baseScript; // Reference of this base script for passing onto instantiated tic tacs
    private static Collector collectorScript;   // Reference of the collector script for passing onto instantiated tic tacs
    private Path meleePath;         // The melee path object
    private Path rangedPath;        // The ranged path object
    private static readonly int[] pathLimits = new int[3] { 20, 30, 40 };  // An array of the close, mid, and far path limits
    private Queue<float> delayTimes;    // A queue of random delay times to vary when tic tacs are spawned
    private const float delayTimeMax = 1.0f;    // The maximum amount of time an tic tac can wait before spawning
    private Queue<Vector3> spawnPositions;  // A queue of random spawn positions
    private const float spawnRadius = 0.3f; // The spawn radius of the tic tacs
    private Dictionary<TicTacTag.Flavor, GameObject> ticTacDictionary; // Dictionary of tic tac prefabs, with their types as keys


    /* Takes in an tic tac flavor and a spawn amount, and spawns that many tic tacs of that flavor. Will only spawn up to the
     * far path limit. */
    public void SpawnEnemies(TicTacTag.Flavor flavor, int spawnAmount) {
        if (!ticTacDictionary.ContainsKey(flavor)) { Debug.LogWarning("Tic Tac with flavor " + flavor + " doesn't exist."); return; }

        TicTacTag.PathRoute pathRoute = ticTacDictionary[flavor].GetComponent<TicTac>().PathRoute;
        Path ticTacPath = (pathRoute == TicTacTag.PathRoute.Melee) ? meleePath : rangedPath;

        for (int i = 0; i < spawnAmount && pathLimits[2] - ticTacPath.count > 0; i++) {
            Vector3 spawnPosition = spawnPositions.Dequeue();  // Get spawn position
            float heightOffset = ticTacDictionary[flavor].transform.GetChild(0).localScale.y +
                (transform.position.y - (transform.localScale.y / 2.0f));   // Used to spawn object above field
            spawnPosition = V3E.SetY(spawnPosition, heightOffset);
            spawnPositions.Enqueue(spawnPosition);

            GameObject ticTac = Instantiate(ticTacDictionary[flavor], spawnPosition, Quaternion.identity); // Instantiate
            TicTac ticTacScript = ticTac.GetComponent<TicTac>();
            ticTacScript.InitializeVariables(ref baseScript, ref collectorScript);

            float delayTime = delayTimes.Dequeue();  // Get delay time
            delayTimes.Enqueue(delayTime);

            if (ticTacPath.count < pathLimits[0]) { // Set path distance and start moving towards a new path position
                ticTacScript.PathDistance = TicTacTag.PathDistance.Close;
                ticTacScript.StartAdvanceCoroutine(((Random.insideUnitSphere * ticTacPath.spawnRadii[0]) + ticTacPath.spawnCenters[0]), delayTime);
            } else if (ticTacPath.count < pathLimits[1]) {
                ticTacScript.PathDistance = TicTacTag.PathDistance.Mid;
                ticTacPath.midEnemyScripts.Enqueue(ticTacScript);
                ticTacScript.StartAdvanceCoroutine(((Random.insideUnitSphere * ticTacPath.spawnRadii[1]) + ticTacPath.spawnCenters[1]), delayTime);
            } else {            // farPathLimit
                ticTacScript.PathDistance = TicTacTag.PathDistance.Far;
                ticTacPath.farEnemyScripts.Enqueue(ticTacScript);
                ticTacScript.StartAdvanceCoroutine(((Random.insideUnitSphere * ticTacPath.spawnRadii[2]) + ticTacPath.spawnCenters[2]), delayTime);
            }
            ticTacPath.count++;
        }
    }

    /* Takes in an tic tac path route and a tic tac path distance, updates the correct path. Will move tic tacs forward from queues
     * depending on where the removed tic tac was previously on the path. */
    public void TicTacRemoved(TicTacTag.PathRoute pathRoute, TicTacTag.PathDistance pathDistance) {
        Path ticTacPath = (pathRoute == TicTacTag.PathRoute.Melee) ? meleePath : rangedPath;

        if (pathDistance == TicTacTag.PathDistance.Close && ticTacPath.midEnemyScripts.Count > 0) {  // Destroyed tic tac was close
            TicTac midEnemyScript = ticTacPath.midEnemyScripts.Dequeue();
            midEnemyScript.PathDistance = TicTacTag.PathDistance.Close;
            midEnemyScript.StartAdvanceCoroutine((Random.insideUnitSphere * ticTacPath.spawnRadii[0]) + ticTacPath.spawnCenters[0], 0.0f);
            if (ticTacPath.farEnemyScripts.Count > 0) {
                TicTac farEnemyScript = ticTacPath.farEnemyScripts.Dequeue();
                farEnemyScript.PathDistance = TicTacTag.PathDistance.Mid;
                farEnemyScript.StartAdvanceCoroutine((Random.insideUnitSphere * ticTacPath.spawnRadii[1]) + ticTacPath.spawnCenters[1], 0.0f);
                ticTacPath.midEnemyScripts.Enqueue(farEnemyScript);
            }
        } else if (pathDistance == TicTacTag.PathDistance.Mid && ticTacPath.farEnemyScripts.Count > 0) {  // Destroyed tic tac was mid
            TicTac farEnemyScript = ticTacPath.farEnemyScripts.Dequeue();
            farEnemyScript.PathDistance = TicTacTag.PathDistance.Mid;
            farEnemyScript.StartAdvanceCoroutine((Random.insideUnitSphere * ticTacPath.spawnRadii[1]) + ticTacPath.spawnCenters[1], 0.0f);
            ticTacPath.midEnemyScripts.Enqueue(farEnemyScript);
        }   // Do nothing if the destroyed tic tac was far

        ticTacPath.count--;
    }

    /* Initializes private variables and populates the paths. */
    private void Awake() {
        baseScript = gameObject.GetComponent<Base>();
        collectorScript = GameObject.Find("Collector").GetComponent<Collector>();

        ticTacDictionary = new Dictionary<TicTacTag.Flavor, GameObject>(); // Initialize tic tac dictionary
        foreach(GameObject ticTacPrefab in ticTacPrefabs) {
            ticTacDictionary.Add(ticTacPrefab.GetComponent<TicTac>().Flavor, ticTacPrefab);
        }

        meleePath = new Path(); // Initialize melee path based on "meleePathGO" children
        foreach (Transform child in meleePathGO.transform) {
            switch (child.name) {
                case "Close":
                    meleePath.spawnRadii[0] = child.localScale.x / 2.0f;
                    meleePath.spawnCenters[0] = child.position;
                    break;
                case "Mid":
                    meleePath.spawnRadii[1] = child.localScale.x / 2.0f;
                    meleePath.spawnCenters[1] = child.position;
                    break;
                case "Far":
                    meleePath.spawnRadii[2] = child.localScale.x / 2.0f;
                    meleePath.spawnCenters[2] = child.position;
                    break;
                default:
                    Debug.LogWarning("Child of path " + meleePathGO.name + " has incorrect name.");
                    break;
            }
        }
        rangedPath = new Path();// Initialize ranged path based on "rangedPathGO" children
        foreach (Transform child in rangedPathGO.transform) {
            switch (child.name) {
                case "Close":
                    rangedPath.spawnRadii[0] = child.localScale.x / 2.0f;
                    rangedPath.spawnCenters[0] = child.position;
                    break;
                case "Mid":
                    rangedPath.spawnRadii[1] = child.localScale.x / 2.0f;
                    rangedPath.spawnCenters[1] = child.position;
                    break;
                case "Far":
                    rangedPath.spawnRadii[2] = child.localScale.x / 2.0f;
                    rangedPath.spawnCenters[2] = child.position;
                    break;
                default:
                    Debug.LogWarning("Child of path " + rangedPathGO.name + " has incorrect name.");
                    break;
            }
        }

        delayTimes = new Queue<float>();    // Initializes delay times
        for (int i = 0; i < pathLimits[2]; i++) {
            float randomTime = Random.Range(0.0f, delayTimeMax);
            delayTimes.Enqueue(randomTime);
        }

        spawnPositions = new Queue<Vector3>();  // Initializes spawn positions
        for(int i = 0; i < pathLimits[2]; i++) {
            Vector3 unitSphereCenter = transform.position + transform.forward * transform.localScale.z / 2;
            Vector3 randomPosition = (Random.insideUnitSphere * spawnRadius) + unitSphereCenter;
            spawnPositions.Enqueue(randomPosition);
        }
    }

}