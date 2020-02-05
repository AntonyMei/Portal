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
    public bool isFixed = true;

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

    private bool HasEntered = false;

    void Update() {
        if (!isFixed) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if(transform.position.y < Other.transform.position.y) {
                renderer.material = LowerMaterial;
            } else {
                renderer.material = UpperMaterial;
            }
        }
    }
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
    public Mesh CreateFractureMesh() {
        Mesh frac = new Mesh();
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(UnityEngine.Random.value,
            UnityEngine.Random.value, UnityEngine.Random.value);
        vertices[2] = new Vector3(UnityEngine.Random.value,
            UnityEngine.Random.value, UnityEngine.Random.value);
        frac.vertices = vertices;
        int[] triangles = new int[6];
        triangles[0] = 0; triangles[1] = 1;
        triangles[2] = 2; triangles[3] = 0;
        triangles[4] = 2; triangles[5] = 1;
        frac.triangles = triangles;
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++) { uvs[i] = Vector2.zero; }
        frac.uv = uvs;
        return frac;
    }
    public void GenerateFractures(GameObject breaked_object) {
        // Create fractures
        GameObject fracture_root = new GameObject();
        fracture_root.name = $"{breaked_object.transform.parent.name}Fracture";
        fracture_root.AddComponent<Fracture>().Duration = FractureDuration;
        Material mat = breaked_object.GetComponent<MeshRenderer>().material;
        for (int i = 0; i < FractureCount; i++) {
            GameObject frac = new GameObject();
            // Rotation, scale, etc
            frac.name = $"Frac{i}";
            frac.transform.parent = fracture_root.transform;
            frac.transform.rotation = GetRandomQuaternion();
            frac.transform.position =
                breaked_object.transform.position + GetRandomPointOnUnitSphere();
            frac.transform.localScale *= FractureLowerBound
                + (FractureUpperBound - FractureLowerBound) * Random.value;
            frac.transform.localScale *= breaked_object.transform.localScale.x;
            // Renderer
            frac.AddComponent<MeshFilter>().mesh = CreateFractureMesh();
            frac.AddComponent<MeshRenderer>().material = mat;
            // Physics
            Rigidbody rigidbody = frac.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.AddExplosionForce(ForceMagnitude * breaked_object.transform.localScale.x,
                breaked_object.transform.position, 15f);
        }
    }
    public void OnCollisionEnter(Collision collision) {
        if (HasEntered) return; HasEntered = true;
        if(collision.gameObject.tag == "Player" || true) {
            // Translate
            collision.gameObject.transform.position = Other.transform.position;
            GenerateFractures(gameObject);
            GenerateFractures(Other);
            // Destroy Portal
            GameObject.Destroy(gameObject);
            GameObject.Destroy(Other);
        }
    }
    public Quaternion GetRandomQuaternion() {
        return new Quaternion(-1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value);
    }
    public Vector3 GetRandomPointOnUnitSphere() {
        Vector3 vec = new Vector3(Random.value - 0.5f, 
            Random.value - 0.5f, Random.value - 0.5f);
        vec.Normalize(); return vec;
    }
}