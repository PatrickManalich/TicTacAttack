using UnityEngine;

/* Vector 3 Extension class that simplifies setting specific coordinate of a Vector3. */
public static class V3E {
    public static Vector3 SetX(this Vector3 vector3, float newX) {
        vector3.x = newX;
        return vector3;
    }
    public static Vector3 SetY(this Vector3 vector3, float newY) {
        vector3.y = newY;
        return vector3;
    }
    public static Vector3 SetZ(this Vector3 vector3, float newZ) {
        vector3.z = newZ;
        return vector3;
    }
}