using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used to access path information such as the radius and scale of a specific path. */
public class PathAreas : MonoBehaviour {

    private Dictionary<SE.PathLetter, float[]> meleeRadiiDictionary;
    private Dictionary<SE.PathLetter, float[]> rangedRadiiDictionary;
    private Dictionary<SE.PathLetter, Vector3[]> meleeCentersDictionary;
    private Dictionary<SE.PathLetter, Vector3[]> rangedCentersDictionary;


    public float[] GetRadii(SE.PathLetter pathLetter, SE.PathRoute pathRoute) {
        Dictionary<SE.PathLetter, float[]> radiiDictionary = (pathRoute == SE.PathRoute.Melee) ? meleeRadiiDictionary : rangedRadiiDictionary;
        return radiiDictionary[pathLetter];
    }

    public Vector3[] GetCenters(SE.PathLetter pathLetter, SE.PathRoute pathRoute) {
        Dictionary<SE.PathLetter, Vector3[]> centersDictionary = (pathRoute == SE.PathRoute.Melee) ? meleeCentersDictionary : rangedCentersDictionary;
        return centersDictionary[pathLetter];
    }

    private void Awake() {
        meleeRadiiDictionary = new Dictionary<SE.PathLetter, float[]>();
        rangedRadiiDictionary = new Dictionary<SE.PathLetter, float[]>();
        meleeCentersDictionary = new Dictionary<SE.PathLetter, Vector3[]>();
        rangedCentersDictionary = new Dictionary<SE.PathLetter, Vector3[]>();

        foreach (Transform child in transform) {    // Go through path letters
            float[] meleeRadii = new float[3];
            float[] rangedRadii = new float[3];
            Vector3[] meleeCenters = new Vector3[3];
            Vector3[] rangedCenters = new Vector3[3];

            foreach (Transform grandchild in child.transform) {  // Go through path routes
                if (string.Equals(grandchild.name, "Melee")) {
                    for (int i = 0; i < 3; i++) {    // Go through path distances
                        Transform grandGrandchild = grandchild.GetChild(i);
                        meleeRadii[i] = grandGrandchild.localScale.x / 2.0f;
                        meleeCenters[i] = grandGrandchild.position;
                    }
                } else if (string.Equals(grandchild.name, "Ranged")) {
                    for (int i = 0; i < 3; i++) {    // Go through path distances
                        Transform grandGrandchild = grandchild.GetChild(i);
                        rangedRadii[i] = grandGrandchild.localScale.x / 2.0f;
                        rangedCenters[i] = grandGrandchild.position;
                    }
                } else
                    Debug.LogWarning("Path route " + grandchild.name + " is not valid");
            }

            SE.PathLetter pathLetter = default(SE.PathLetter);
            switch (child.name) {
                case "A":
                    pathLetter = SE.PathLetter.A;
                    break;
                case "B":
                    pathLetter = SE.PathLetter.B;
                    break;
                case "C":
                    pathLetter = SE.PathLetter.C;
                    break;
                case "D":
                    pathLetter = SE.PathLetter.D;
                    break;
                default:
                    Debug.LogWarning("Path letter " + child.name + " is not valid");
                    break;
            }
            meleeRadiiDictionary.Add(pathLetter, meleeRadii);
            rangedRadiiDictionary.Add(pathLetter, rangedRadii);
            meleeCentersDictionary.Add(pathLetter, meleeCenters);
            rangedCentersDictionary.Add(pathLetter, rangedCenters);
        }
    }
}
