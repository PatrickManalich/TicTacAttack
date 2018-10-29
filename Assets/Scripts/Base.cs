using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Spawns and initializes tic tacs. Keeps track of two paths, a melee path and a ranged path. Manages tic tacs 
 * when they are destroyed and updates paths. */
public class Base : MonoBehaviour {

    public SE.PathLetter pathLetter;
    public List<GameObject> ticTacPrefabs;   // A list of tic tac prefabs that will be cloned when spawning

    private GameObject pathAreas;
    private GameObject collector;
    private class Path {    // Encapsulates all necessary information for a path
        public float[] radii = new float[3];   // The spawn radii for close, mid, and far
        public Vector3[] centers = new Vector3[3]; // The spawn centers for close, mid, and far
        public Dictionary<int, TicTac> scriptDict = new Dictionary<int, TicTac>();  // A dictionary whose values consist of the scripts of 
                                                                                    // the tic tacs on this path, with keys of the tic tacs'
                                                                                    // instance IDs
        public TicTac GetFirstScript(SE.PathDistance pathDistance) {
            foreach (TicTac ticTacScript in scriptDict.Values) {
                if (ticTacScript.CurrPathDistance == pathDistance)
                    return ticTacScript;
            }
            return null;
        }
    }
    private Path meleePath;         // The melee path object
    private Path rangedPath;        // The ranged path object
    private static readonly int[] pathLimits = new int[3] { 25, 50, 75};  // An array of the close, mid, and far path limits
    private Queue<float> delayTimes;    // A queue of random delay times to vary when tic tacs are spawned
    private const float delayTimeMax = 1.0f;    // The maximum amount of time an tic tac can wait before spawning
    private Queue<Vector3> spawnPositions;  // A queue of random spawn positions
    private const float spawnRadius = 0.3f; // The spawn radius of the tic tacs
    private Dictionary<SE.Flavor, GameObject> ticTacPrefabDict; // Dictionary of tic tac prefabs, with their types as keys


    /* Takes in an tic tac flavor and a spawn amount, and spawns that many tic tacs of that flavor. Will only spawn up to the
     * far path limit. */
    public void SpawnEnemies(SE.Flavor flavor, int spawnAmount) {
        if (!ticTacPrefabDict.ContainsKey(flavor)) { Debug.LogWarning("Tic Tac with flavor " + flavor + " doesn't exist."); return; }

        SE.PathRoute pathRoute = ticTacPrefabDict[flavor].GetComponent<TicTac>().PathRoute;
        Path path = (pathRoute == SE.PathRoute.Melee) ? meleePath : rangedPath;

        for (int i = 0; i < spawnAmount && pathLimits[2] - path.scriptDict.Count > 0; i++) {
            Vector3 spawnPosition = spawnPositions.Dequeue();  // Get spawn position
            float heightOffset = ticTacPrefabDict[flavor].transform.GetChild(0).localScale.y +
                (transform.position.y - (transform.localScale.y / 2.0f));   // Used to spawn object above field
            spawnPosition = V3E.SetY(spawnPosition, heightOffset);
            spawnPositions.Enqueue(spawnPosition);

            GameObject ticTac = Instantiate(ticTacPrefabDict[flavor], spawnPosition, Quaternion.identity); // Instantiate
            TicTac ticTacScript = ticTac.GetComponent<TicTac>();
            ticTacScript.InitializeVariables(gameObject, collector);

            float delayTime = delayTimes.Dequeue();  // Get delay time
            delayTimes.Enqueue(delayTime);

            if (path.scriptDict.Count < pathLimits[0]) { // Set path distance and start moving towards a new path position
                ticTacScript.CurrPathDistance = SE.PathDistance.Close;
                ticTacScript.StartAdvanceCoroutine(((Random.insideUnitSphere * path.radii[0]) + path.centers[0]), delayTime);
                path.scriptDict.Add(ticTac.GetInstanceID(), ticTacScript);
            } else if (path.scriptDict.Count < pathLimits[1]) {
                ticTacScript.CurrPathDistance = SE.PathDistance.Mid;
                ticTacScript.StartAdvanceCoroutine(((Random.insideUnitSphere * path.radii[1]) + path.centers[1]), delayTime);
                path.scriptDict.Add(ticTac.GetInstanceID(), ticTacScript);
            } else {            // farPathLimit
                ticTacScript.CurrPathDistance = SE.PathDistance.Far;
                ticTacScript.StartAdvanceCoroutine(((Random.insideUnitSphere * path.radii[2]) + path.centers[2]), delayTime);
                path.scriptDict.Add(ticTac.GetInstanceID(), ticTacScript);
            }

        }
    }

    /* Takes in a tic tac's path route and instance ID, and updates the correct path. Will move tic tacs forward from queues
     * depending on where the removed tic tac was previously on the path. */
    public void TicTacRemoved(SE.PathRoute removedPathRoute, SE.PathDistance removedPathDistance, int removedInstanceID) {
        Path removedPath = (removedPathRoute == SE.PathRoute.Melee) ? meleePath : rangedPath;
        TicTac removedScript = removedPath.scriptDict[removedInstanceID];

        if(removedPathDistance == SE.PathDistance.Close && removedPath.scriptDict.Count > pathLimits[0]) {
            TicTac firstMidScript = removedPath.GetFirstScript(SE.PathDistance.Mid);    // Move first mid script up to close distance
            firstMidScript.CurrPathDistance = SE.PathDistance.Close;
            firstMidScript.StartAdvanceCoroutine((Random.insideUnitSphere * removedPath.radii[0]) + removedPath.centers[0], 0.0f);

            if (removedPath.scriptDict.Count > pathLimits[1]) {
                TicTac firstFarScript = removedPath.GetFirstScript(SE.PathDistance.Far);    // Move first far script up to mid distance
                firstFarScript.CurrPathDistance = SE.PathDistance.Mid;
                firstFarScript.StartAdvanceCoroutine((Random.insideUnitSphere * removedPath.radii[1]) + removedPath.centers[1], 0.0f);
            }
        } else if(removedPathDistance == SE.PathDistance.Mid && removedPath.scriptDict.Count > pathLimits[1]) {    // Removed tic tac was far
            TicTac firstFarScript = removedPath.GetFirstScript(SE.PathDistance.Far);    // Move first far script up to mid distance
            firstFarScript.CurrPathDistance = SE.PathDistance.Mid;
            firstFarScript.StartAdvanceCoroutine((Random.insideUnitSphere * removedPath.radii[1]) + removedPath.centers[1], 0.0f);
        }

        removedPath.scriptDict.Remove(removedInstanceID);
    }

    /* Initializes private variables and populates the paths. */
    private void Awake() {
        pathAreas = GameObject.Find("PathAreas");
        if (pathAreas == null)
            Debug.LogWarning("Please name path areas GameObject \"PathAreas\"");
        collector = GameObject.Find("Collector");
        if (collector == null)
            Debug.LogWarning("Please name collector GameObject \"Collector\"");

        ticTacPrefabDict = new Dictionary<SE.Flavor, GameObject>(); // Initialize tic tac dictionary
        foreach(GameObject ticTacPrefab in ticTacPrefabs) { ticTacPrefabDict.Add(ticTacPrefab.GetComponent<TicTac>().Flavor, ticTacPrefab); }

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

    /* Need to wait for path areas script to setup before calling functions on it. */
    private void Start() {
        PathAreas pathAreasScript = pathAreas.GetComponent<PathAreas>();

        meleePath = new Path(); // Initialize melee path
        meleePath.radii = pathAreasScript.GetRadii(pathLetter, SE.PathRoute.Melee);
        meleePath.centers = pathAreasScript.GetCenters(pathLetter, SE.PathRoute.Melee);

        rangedPath = new Path(); // Initialize ranged path
        rangedPath.radii = pathAreasScript.GetRadii(pathLetter, SE.PathRoute.Ranged);
        rangedPath.centers = pathAreasScript.GetCenters(pathLetter, SE.PathRoute.Ranged);
    }

}