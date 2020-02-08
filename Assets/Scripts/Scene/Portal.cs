using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    [Tooltip("The other part of the portal")]
    public GameObject Other;
    [Tooltip("The material attached to this portal when its higher than Other")]
    public Material UpperMaterial;
    [Tooltip("The material attached to this portal when its lower than Other")]
    public Material LowerMaterial;
    [Tooltip("If toggled, the portal's color will not be refreshed automatically every frame." +
        "i.e. must call RefreshColor() manually")]
    public bool isStatic = true;

    [Header("Fracture Settings")]
    [Tooltip("The lower bound of fracture size")]
    public float FractureLowerBound = 0.2f;
    [Tooltip("The upper bound of fracture size")]
    public float FractureUpperBound = 1f;
    [Tooltip("The number of fractures")]
    public int FractureCount = 100;
    [Tooltip("Duration of fractures")]
    public int FractureDuration = 10;
    [Tooltip("The magnitude of the force")]
    public float ForceMagnitude = 100f;

    private bool HasPlayerEntered = false;

    void Update() {
        // If the portal is non-static, its color will be changed automatically
        // For static portals, must call RefreshColor manually to change color 
        if (!isStatic) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if(transform.position.y < Other.transform.position.y) {
                renderer.material = LowerMaterial;
            } else {
                renderer.material = UpperMaterial;
            }
        }
    }

    /// <summary>
    /// Refreshes the color of the portal
    /// </summary>
    public void RefreshColor() {
        if (Other) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (transform.position.y < Other.transform.position.y) {
                renderer.material = LowerMaterial;
            } else {
                renderer.material = UpperMaterial;
            }
        }
    }

    /// <summary>
    /// <para> Creates a random triangle that is rendered on both sides </para>
    /// </summary>
    /// <returns> The mesh of this  </returns>
    public static Mesh CreateFractureMesh() {
        Mesh frac = new Mesh();
        // Set the vertices of the mesh
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(UnityEngine.Random.value,
            UnityEngine.Random.value, UnityEngine.Random.value);
        vertices[2] = new Vector3(UnityEngine.Random.value,
            UnityEngine.Random.value, UnityEngine.Random.value);
        frac.vertices = vertices;
        // Set the two triangles of the mesh
        int[] triangles = new int[6];
        triangles[0] = 0; triangles[1] = 1;
        triangles[2] = 2; triangles[3] = 0;
        triangles[4] = 2; triangles[5] = 1;
        frac.triangles = triangles;
        // Set UV of the mesh
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++) { uvs[i] = Vector2.zero; }
        frac.uv = uvs;
        return frac;
    }

    /// <summary>
    /// <para> Generate scattering fractures to simulate the break of an object </para>
    /// <para> The object must be destroyed manually by calling GameObject.Destroy(GameObject) </para>
    /// </summary>
    /// <param name="breaked_object"> 
    /// <para> The gameobject that is broken. </para>
    /// <para> The object is only used for its position </para>
    /// </param>
    /// <param name="fracture_duration"> The period of time the fracures will last </param>
    /// <param name="fracture_count"> The amount of fractures </param>
    /// <param name="fracture_lower_bound"> The lower bound of fracture size </param>
    /// <param name="fracture_upper_bound"> The upper bound of fracture size </param>
    /// <param name="force_magnitude"> The magnitude of force added to the fractures </param>
    public static void GenerateFractures(GameObject breaked_object, float fracture_duration,
                                         float fracture_count, float fracture_lower_bound,
                                         float fracture_upper_bound, float force_magnitude) {
        // Create fractures, fractures will last for a period of time
        // The material of fractures is the same as the breaked portal
        GameObject fracture_root = new GameObject();
        fracture_root.name = $"{breaked_object.transform.parent.name}Fracture";
        fracture_root.AddComponent<Fracture>().Duration = fracture_duration;
        Material mat = breaked_object.GetComponent<MeshRenderer>().material;
        // Set the properties of each fracture
        for (int i = 0; i < fracture_count; i++) {
            GameObject frac = new GameObject();
            // Rotation, scale, etc
            frac.name = $"Frac{i}";
            frac.transform.parent = fracture_root.transform;
            frac.transform.rotation = GetRandomQuaternion();
            frac.transform.position =
                breaked_object.transform.position + GetRandomPointOnUnitSphere();
            frac.transform.localScale *= fracture_lower_bound
                + (fracture_upper_bound - fracture_lower_bound) * Random.value;
            frac.transform.localScale *= breaked_object.transform.localScale.x;
            // Set renderer
            frac.AddComponent<MeshFilter>().mesh = CreateFractureMesh();
            frac.AddComponent<MeshRenderer>().material = mat;
            // Add an explosive force on the fracture
            Rigidbody rigidbody = frac.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.AddExplosionForce(force_magnitude * breaked_object.transform.localScale.x,
                breaked_object.transform.position, 15f);
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Player" || true) {
            if (HasPlayerEntered) return; HasPlayerEntered = true;
            // Translate
            collision.gameObject.transform.position = Other.transform.position;
            // Generate fractures
            GenerateFractures(gameObject, FractureDuration, FractureCount,
                FractureLowerBound, FractureUpperBound, ForceMagnitude);
            GenerateFractures(Other, FractureDuration, FractureCount,
                FractureLowerBound, FractureUpperBound, ForceMagnitude);
            // Destroy Portal
            GameObject.Destroy(gameObject);
            GameObject.Destroy(Other);
        }
    }

    /// <summary>
    /// Get a random quaternion
    /// </summary>
    /// <returns></returns>
    public static Quaternion GetRandomQuaternion() {
        return new Quaternion(-1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value);
    }

    /// <summary>
    /// Get a random point on unit sphere
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetRandomPointOnUnitSphere() {
        Vector3 vec = new Vector3(Random.value - 0.5f, 
            Random.value - 0.5f, Random.value - 0.5f);
        vec.Normalize(); return vec;
    }
}