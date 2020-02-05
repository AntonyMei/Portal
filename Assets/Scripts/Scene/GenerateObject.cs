using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class GenerateObject : MonoBehaviour
{
    [Header("Fracture Settings")]
    // Parameters of fixed fractures. Fractures are used as portals.
    // This part is obselete. Use Portal settings instead.
    [Tooltip("The lower bound of fracture size"), HideInInspector]
    public float FractureLowerBound = 5.0f;
    [Tooltip("The upper bound of fracture size"), HideInInspector]
    public float FractureUpperBound = 20.0f;
    [Tooltip("The material of fractures where its lower than the other"), HideInInspector]
    public Material LowerFractureMaterial;
    [Tooltip("The material of fractures where its higher than the other"), HideInInspector]
    public Material UpperFractureMaterial;
    [Tooltip("The density of fractures(Relative to height)."), HideInInspector]
    public AnimationCurve FixedFractureDensity = new AnimationCurve(
        new Keyframe(0, 3, 0, 0), new Keyframe(300, 40, 0, 0),
        new Keyframe(1000, 200, 0, 0), new Keyframe(5000, 10, 0, 0));

    [Header("Portal Settings")]
    // Parameters of portals.
    [Tooltip("The lower bound of fracture size")]
    public float PortalLowerBound = 5.0f;
    [Tooltip("The upper bound of fracture size")]
    public float PortalUpperBound = 10.0f;
    [Tooltip("The material of fractures where its lower than the other")]
    public Material LowerPortalMaterial;
    [Tooltip("The material of fractures where its higher than the other")]
    public Material UpperPortalMaterial;
    [Tooltip("Portal prefab")]
    public GameObject PortalPrefab;
    [Tooltip("The density of fractures(Relative to height).")]
    public AnimationCurve PortalDensity = new AnimationCurve(
        new Keyframe(0, 2, 0, 0), new Keyframe(300, 6, 0, 0),
        new Keyframe(1000, 10, 0, 0), new Keyframe(5000, 2, 0, 0));

    [Header("Anchor Settings")]
    // Parameters of fixed anchors. Anchors are ordinary objects players can attach ray to.
    [Tooltip("The lower bound of anchor size")]
    public float AnchorLowerBound = 3.0f;
    [Tooltip("The upper bound of anchor size")]
    public float AnchorUpperBound = 5.0f;
    [Tooltip("Anchor prefab")]
    public GameObject AnchorPrefab;
    [Tooltip("The density of fixed anchors(Relative to height).")]
    public AnimationCurve AnchorDensity = new AnimationCurve(
        new Keyframe(0, 100, 0, 0), new Keyframe(300, 60, 0, 0),
        new Keyframe(1000, 30, 0, 0), new Keyframe(5000, 10, 0, 0));

    [Header("Trampoline Settings")]
    [Tooltip("The lower bound of trampoline size")]
    public float TrampolineLowerBound = 3.0f;
    [Tooltip("The upper bound of trampoline size")]
    public float TrampolineUpperBound = 5.0f;
    [Tooltip("Trampoline prefab")]
    public GameObject TrampolinePrefab;
    [Tooltip("The density of fixed trampolines(Relative to height).")]
    public AnimationCurve TrampolineDensity = new AnimationCurve(
        new Keyframe(0, 10, 0, 0), new Keyframe(300, 6, 0, 0),
        new Keyframe(1000, 3, 0, 0), new Keyframe(5000, 1, 0, 0));

    [Header("Plane Settings")]
    [Tooltip("The lower bound of plane size.")]
    public float PlaneLowerBound = 1f;
    [Tooltip("The upper bound of plane size.")]
    public float PlaneUpperBound = 5f;
    [Tooltip("The prefab of a plane")]
    public GameObject PlanePrefab;
    [Tooltip("The density of fixed planes(Relative to height).")]
    public AnimationCurve PlaneDensity = new AnimationCurve(
        new Keyframe(0, 3, 0, 0), new Keyframe(300, 6, 0, 0),
        new Keyframe(1000, 2, 0, 0), new Keyframe(5000, 0, 0, 0));

    //Math functions
    public class Range {
        private float min_;
        private float max_;
        private float length_;
        public float min {
            get { return min_; }
            set {
                min_ = value;
                length_ = max_ - min_;
            }
        }
        public float max {
            get { return max_; }
            set {
                max_ = value;
                length_ = max_ - min_;
            }
        }
        public float length {
            get { return length_; }
        }
        public Range() { min_ = 0; max_ = 0; length_ = 0; }
        public Range(float min_val, float max_val) {
            min_ = min_val; max_ = max_val; length_ = max_ - min_;
        }
    }
    public Vector3 GetRandomPos(Range x_range, Range y_range, Range z_range) {
        float x = x_range.min + x_range.length * UnityEngine.Random.value;
        float y = y_range.min + y_range.length * UnityEngine.Random.value;
        float z = z_range.min + z_range.length * UnityEngine.Random.value;
        return new Vector3(x, y, z);
    }
    public Quaternion GetRandomQuaternion() {
        return new Quaternion(-1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value,
                              -1 + 2 * UnityEngine.Random.value);
    }
    public int GetRandInt(int upperbound) {
        return (int)(UnityEngine.Random.value * upperbound);
    }
    public _tp Min<_tp>(_tp a, _tp b) where _tp : IComparable {
        return a.CompareTo(b) < 0 ? a : b;
    }
    public _tp Max<_tp>(_tp a, _tp b) where _tp : IComparable {
        return a.CompareTo(b) < 0 ? b : a;
    }

    // Fracture
    public Mesh CreateFracture() {
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
    [Obsolete("Do not call this function, use portal instead",true)]
    public GameObject GenerateFractures(Range x, Range y, Range z) {
        //Create the root GameObject
        GameObject root = new GameObject {
            name = $"X{(x.min % 500).ToString()}{(x.max % 500).ToString()}" +
                   $"Y{(y.min % 500).ToString()}{(y.max % 500).ToString()}" +
                   $"Z{(z.min % 500).ToString()}{(z.max % 500).ToString()}" +
                   $"Frac"
        };
        //Generate Fractures
        int fixed_fracture_count =
            (int)FixedFractureDensity.Evaluate(Min((y.min + y.max) / 2, 5000));
        fixed_fracture_count += fixed_fracture_count % 2 == 0 ? 0 : 1;
        GameObject last = null;
        for(int i = 0; i < fixed_fracture_count; i++) {
            GameObject frac = new GameObject();
            frac.name = $"{root.name}id{i.ToString()}";
            frac.tag = "Fracture";
            //Set transform
            frac.transform.position = GetRandomPos(x, y, z);
            frac.transform.rotation = GetRandomQuaternion();
            frac.transform.parent = root.transform;
            float size_factor = FractureLowerBound
                                + UnityEngine.Random.value
                                * (FractureUpperBound - FractureLowerBound);
            frac.transform.localScale *= size_factor;
            //Set Mesh
            MeshFilter filter = frac.AddComponent<MeshFilter>();
            filter.mesh = CreateFracture();
            MeshRenderer renderer = frac.AddComponent<MeshRenderer>();
            renderer.material = LowerFractureMaterial;
            //Set Portal
            Portal portal = frac.AddComponent<Portal>();
            portal.LowerMaterial = LowerFractureMaterial;
            portal.UpperMaterial = UpperFractureMaterial;
            portal.isFixed = true;
            if(i % 2 == 0) {
                last = frac;
            } else {
                portal.Other = last;
                var temp = last.GetComponent<Portal>();
                temp.Other = frac; temp.RefreshColor();
                portal.RefreshColor();
            }
        }
        return root;
    }

    // Portal
    public GameObject GeneratePortal(Range x, Range y, Range z) {
        //Create the root GameObject
        GameObject root = new GameObject {
            name = $"X{(x.min % 500).ToString()}{(x.max % 500).ToString()}" +
                   $"Y{(y.min % 500).ToString()}{(y.max % 500).ToString()}" +
                   $"Z{(z.min % 500).ToString()}{(z.max % 500).ToString()}" +
                   $"Portal"
        };
        //Generate Fractures
        int portal_count = 
            (int)PortalDensity.Evaluate(Min((y.min + y.max) / 2, 5000));
        portal_count += portal_count % 2 == 0 ? 0 : 1;
        GameObject last = null;
        for (int i = 0; i < portal_count; i++) {
            GameObject portal_obj = Instantiate(PortalPrefab);
            portal_obj.name = $"{root.name}id{i.ToString()}";
            portal_obj.tag = "Portal";
            //Set transform
            portal_obj.transform.position = GetRandomPos(x, y, z);
            portal_obj.transform.rotation = GetRandomQuaternion();
            portal_obj.transform.parent = root.transform;
            float size_factor = PortalLowerBound
                                + UnityEngine.Random.value
                                * (PortalUpperBound - PortalLowerBound);
            portal_obj.transform.localScale *= size_factor;
            // Get the actual portal
            GameObject actual_portal = portal_obj.transform.GetChild(1).gameObject;
            // Set Portal
            Portal portal = actual_portal.AddComponent<Portal>();
            portal.LowerMaterial = LowerPortalMaterial;
            portal.UpperMaterial = UpperPortalMaterial;
            portal.isFixed = true;
            if (i % 2 == 0) {
                last = actual_portal;
            } else {
                portal.Other = last;
                var temp = last.GetComponent<Portal>();
                temp.Other = actual_portal; temp.RefreshColor();
                portal.RefreshColor();
            }
        }
        return root;
    }

    //Anchor
    public GameObject GenerateAnchor(Range x, Range y, Range z) {
        //Create the root GameObject
        GameObject root = new GameObject {
            name = $"X{(x.min % 500).ToString()}{(x.max % 500).ToString()}" +
                   $"Y{(y.min % 500).ToString()}{(y.max % 500).ToString()}" +
                   $"Z{(z.min % 500).ToString()}{(z.max % 500).ToString()}" +
                   $"Anchor"
        };
        //Generate anchor
        int fixed_anchor_count = 
            (int)AnchorDensity.Evaluate(Min((y.max + y.min) / 2, 5000));
        for(int i = 0; i< fixed_anchor_count; i++) {
            GameObject anchor = null;
            anchor = Instantiate(AnchorPrefab, 
                GetRandomPos(x, y, z), GetRandomQuaternion(), root.transform);
            anchor.tag = "Anchor";
            anchor.name = $"{root.name}id{i.ToString()}";
            float size_factor = AnchorLowerBound
                                + (UnityEngine.Random.value
                                * (AnchorUpperBound - AnchorLowerBound));
            anchor.transform.localScale *= size_factor;
        }
        return root;
    }

    // Trampoline
    public GameObject GenerateTrampoline(Range x, Range y, Range z) {
        //Create the root GameObject
        GameObject root = new GameObject {
            name = $"X{(x.min % 500).ToString()}{(x.max % 500).ToString()}" +
                   $"Y{(y.min % 500).ToString()}{(y.max % 500).ToString()}" +
                   $"Z{(z.min % 500).ToString()}{(z.max % 500).ToString()}" +
                   $"Trampoline"
        };
        //Generate anchor
        int trampoline_count =
            (int)TrampolineDensity.Evaluate(Min((y.max + y.min) / 2, 5000));
        for (int i = 0; i < trampoline_count; i++) {
            GameObject trampoline = Instantiate(TrampolinePrefab,
                GetRandomPos(x, y, z), GetRandomQuaternion(), root.transform);
            trampoline.tag = "Trampoline";
            trampoline.name = $"{root.name}id{i.ToString()}";
            float size_factor = TrampolineLowerBound
                                + (UnityEngine.Random.value
                                * (TrampolineUpperBound - TrampolineLowerBound));
            trampoline.transform.localScale *= size_factor;
            // Add Here
            //
            //
            //
        }
        return root;
    }

    //Plane
    public GameObject GeneratePlane(Range x, Range y, Range z) {
        //Create the root GameObject
        GameObject root = new GameObject {
            name = $"X{(x.min % 500).ToString()}{(x.max % 500).ToString()}" +
                   $"Y{(y.min % 500).ToString()}{(y.max % 500).ToString()}" +
                   $"Z{(z.min % 500).ToString()}{(z.max % 500).ToString()}" +
                   $"Plane"
        };
        //Generate plane
        int fixed_plane_count = 
            (int)PlaneDensity.Evaluate(Min((y.max + y.min) / 2, 5000));
        for (int i = 0; i < fixed_plane_count; i++) {
            GameObject plane = Instantiate(PlanePrefab,
                               GetRandomPos(x, y, z),
                               new Quaternion(),
                               root.transform);
            plane.name = $"{root.name}id{i.ToString()}";
            plane.tag = "Plane"; plane.layer = 9;
            float size_factor = PlaneLowerBound
                                + (UnityEngine.Random.value
                                * (PlaneUpperBound - PlaneLowerBound));
            plane.transform.localScale *= size_factor;
        }
        return root;
    }

    private void Start() {
        //GenerateFractures(new Range(0, 500), new Range(0, 500), new Range(0, 500));
        //UnityEngine.Random.seed = 1;
        //GeneratePortal(new Range(0, 500), new Range(0, 500), new Range(0, 500));
        //GenerateAnchor(new Range(0, 500), new Range(0, 500), new Range(0, 500));
        //GeneratePlane(new Range(0, 500), new Range(0, 500), new Range(0, 500));
    }
}