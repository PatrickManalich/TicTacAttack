using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * 
 */
public static class V3E {

    public static Vector3 SetX(this Vector3 vectorParam, float newX) {
        vectorParam.x = newX;
        return vectorParam;
    }
    public static Vector3 SetY(this Vector3 vectorParam, float newY) {
        vectorParam.y = newY;
        return vectorParam;
    }
    public static Vector3 SetZ(this Vector3 vectorParam, float newZ) {
        vectorParam.z = newZ;
        return vectorParam;
    }
}