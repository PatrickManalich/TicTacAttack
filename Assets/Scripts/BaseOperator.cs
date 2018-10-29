using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Cordinates base spawning at specified times using the private spawn model class */
public class BaseOperator : MonoBehaviour {

    public List<GameObject> bases;


    private class SpawnModel {  // Encapsulates all the necessary information to spawn tic tacs
        public SE.PathLetter pathLetter;    // The path letter of the path that tic tacs will spawn on
        public int spawnAmount; // How many tic tacs to spawn
        public SE.Flavor flavor;    // What flavor of tic tacs to spawn
        public float spawnTime; // How long to wait before spawning the tic tacs
        public SpawnModel(SE.PathLetter pathLetter, int spawnAmount, SE.Flavor flavor, float spawnTime) {
            this.pathLetter = pathLetter;
            this.spawnAmount = spawnAmount;
            this.flavor = flavor;
            this.spawnTime = spawnTime;
        }
    }
    private Dictionary<SE.PathLetter, Base> baseScriptDictionary;  // Dictionary of cached base scripts
    private Queue<SpawnModel> spawnModels;

    private void Awake() {
        baseScriptDictionary = new Dictionary<SE.PathLetter, Base>();  // Initialize base script dictionary
        foreach(GameObject baseGO in bases){
            Base baseScript = baseGO.GetComponent<Base>();
            baseScriptDictionary.Add(baseScript.pathLetter, baseScript);
        }

        spawnModels = new Queue<SpawnModel>();

        spawnModels.Enqueue(new SpawnModel(SE.PathLetter.A, 75, SE.Flavor.Mint, 0.0f));
    }

    private void Start() {
        if(spawnModels.Count > 0)
            StartCoroutine(SpawnTicTacsFromSpawnModels());
    }


    private IEnumerator SpawnTicTacsFromSpawnModels() {
        SpawnModel spawnModel = spawnModels.Dequeue();

        if (!baseScriptDictionary.ContainsKey(spawnModel.pathLetter)) {
            Debug.LogWarning("Base with path letter " + spawnModel.pathLetter + " doesn't exist.");
            yield return null;
        } else if(spawnModel.spawnAmount <= 0) {
            Debug.LogWarning("Shouldn't be spawning " + spawnModel.spawnAmount + " tic tacs.");
            yield return null;
        } else if(spawnModel.spawnTime < 0) {
            Debug.LogWarning("Spawn time of " + spawnModel.spawnTime + " is invalid.");
            yield return null;
        }

        yield return new WaitForSeconds(spawnModel.spawnTime);
        baseScriptDictionary[spawnModel.pathLetter].SpawnEnemies(spawnModel.flavor, spawnModel.spawnAmount);
        
        if (spawnModels.Count == 0)
            yield return null;
        else
            yield return StartCoroutine(SpawnTicTacsFromSpawnModels());
    }
    
}