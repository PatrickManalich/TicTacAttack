using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cordinates base spawning at specified times using the private spawn model class */
public class BaseOperator : MonoBehaviour {

    public GameObject baseA;    // The reference of the base A game object
    public GameObject baseB;    // The reference of the base B game object
    public GameObject baseC;    // The reference of the base C game object
    public GameObject baseD;    // The reference of the base D game object

    private class SpawnModel {  // Encapsulates all the necessary information to spawn tic tacs
        public int spawnAmount; // How many tic tacs to spawn
        public TicTacTag.Flavor flavor;    // What flavor of tic tacs to spawn
        public float spawnTime; // How long to wait before spawning the tic tacs
        public SpawnModel(int spawnAmount, TicTacTag.Flavor flavor, float spawnTime) {
            this.spawnAmount = spawnAmount;
            this.flavor = flavor;
            this.spawnTime = spawnTime;
        }
    }
    private Base baseAScript;   // Cached script for base A
    private Base baseBScript;   // Cached script for base B
    private Base baseCScript;   // Cached script for base C
    private Base baseDScript;   // Cached script for base D
    private Dictionary<string, Base> baseScriptDictionary;  // Dictionary of cached base scripts
    private Queue<SpawnModel> baseASpawnModels; // A queue of spawn models to be spawned by base A
    private Queue<SpawnModel> baseBSpawnModels; // A queue of spawn models to be spawned by base B
    private Queue<SpawnModel> baseCSpawnModels; // A queue of spawn models to be spawned by base C
    private Queue<SpawnModel> baseDSpawnModels; // A queue of spawn models to be spawned by base D

    private void Awake() {
        baseAScript = baseA.GetComponent<Base>();   // Initialize base scripts
        baseBScript = baseB.GetComponent<Base>();
        baseCScript = baseC.GetComponent<Base>();
        baseDScript = baseD.GetComponent<Base>();

        baseScriptDictionary = new Dictionary<string, Base>();  // Initialize base script dictionary
        baseScriptDictionary.Add(baseA.name, baseAScript);
        baseScriptDictionary.Add(baseB.name, baseBScript);
        baseScriptDictionary.Add(baseC.name, baseCScript);
        baseScriptDictionary.Add(baseD.name, baseDScript);

        baseASpawnModels = new Queue<SpawnModel>(); // Initialize spawn models for each base
        baseBSpawnModels = new Queue<SpawnModel>();
        baseCSpawnModels = new Queue<SpawnModel>();
        baseDSpawnModels = new Queue<SpawnModel>();

        // Wave 1
        baseASpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Orange, 5.0f));
        baseBSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Mint, 5.0f));
        baseCSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.PeachAndPassionFruit, 5.0f));
        baseDSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Spearmint, 5.0f));
        // Wave 2
        baseASpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Mint, 5.0f));
        baseBSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.PeachAndPassionFruit, 5.0f));
        baseCSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Spearmint, 5.0f));
        baseDSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Orange, 5.0f));
        // Wave 3
        baseASpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.PeachAndPassionFruit, 5.0f));
        baseBSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Spearmint, 5.0f));
        baseCSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Orange, 5.0f));
        baseDSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Mint, 5.0f));
        // Wave 4
        baseASpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Spearmint, 5.0f));
        baseBSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Orange, 5.0f));
        baseCSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.Mint, 5.0f));
        baseDSpawnModels.Enqueue(new SpawnModel(5, TicTacTag.Flavor.PeachAndPassionFruit, 5.0f));
    }

    private void Start() {
        if(baseASpawnModels.Count > 0)  // Start 4 base spawn coroutines
            StartCoroutine(BaseSpawn(baseA.name, baseASpawnModels.Dequeue()));
        if (baseBSpawnModels.Count > 0)
            StartCoroutine(BaseSpawn(baseB.name, baseBSpawnModels.Dequeue()));
        if (baseCSpawnModels.Count > 0)
            StartCoroutine(BaseSpawn(baseC.name, baseCSpawnModels.Dequeue()));
        if (baseDSpawnModels.Count > 0)
            StartCoroutine(BaseSpawn(baseD.name, baseDSpawnModels.Dequeue()));
    }

    /* Takes in a base name and spawn model, and spawns tic tacs from the respective base using the
     * information from the spawn model.*/
    private IEnumerator BaseSpawn(string baseName, SpawnModel spawnModel) {
        if (!baseScriptDictionary.ContainsKey(baseName)) {
            Debug.LogWarning("Base with name " + baseName + " doesn't exist.");
            yield return null;
        } else if(spawnModel.spawnAmount <= 0) {
            Debug.LogWarning("Shouldn't be spawning " + spawnModel.spawnAmount + " tic tacs.");
            yield return null;
        } else if(spawnModel.spawnTime <= 0) {
            Debug.LogWarning("Spawn time of " + spawnModel.spawnTime + " is invalid.");
            yield return null;
        }

        yield return new WaitForSeconds(spawnModel.spawnTime);
        Debug.Log((int)Time.time + " secs: Spawning " + spawnModel.spawnAmount + " " + spawnModel.flavor + "s from " + baseName);
        baseScriptDictionary[baseName].SpawnEnemies(spawnModel.flavor, spawnModel.spawnAmount);

        Queue<SpawnModel> spawnModels;              // Store a reference to the correct spawn model queue
        if (string.Equals(baseName, baseA.name))
            spawnModels = baseASpawnModels;
        else if (string.Equals(baseName, baseB.name))
            spawnModels = baseBSpawnModels;
        else if (string.Equals(baseName, baseC.name))
            spawnModels = baseCSpawnModels;
        else
            spawnModels = baseDSpawnModels;
        
        if (spawnModels.Count == 0) {
            Debug.Log(baseName + " finished spawning tic tacs");
            yield return null;
        } else
            yield return StartCoroutine(BaseSpawn(baseName, spawnModels.Dequeue()));
    }
    
}