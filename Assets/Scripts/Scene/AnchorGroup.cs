using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorGroup : MonoBehaviour
{
    [Tooltip("The number of anchors in this group"), HideInInspector]
    public int AnchorCount;
    [Tooltip("The map that stores the connection of anchors"), HideInInspector]
    public bool[,] AnchorMap;

    private void Start() {
        // Get the number of anchors to initialize
        AnchorCount = transform.childCount;
        AnchorMap = new bool[AnchorCount, AnchorCount];
    }

    /// <summary>
    /// Get the mass center connected component containing the input anchor
    /// </summary>
    /// <param name="input_anchor"> Id of the anchor contained </param>
    /// <returns></returns>
    public Vector3 GetMassCenter(int anchor_id) {
        // Get the connected component
        // Based on BFS
        // The queue that stores querys.
        Queue<int> query_list = new Queue<int>();
        // The list that stores id of the anchors in connected component
        List<int> anchor_list = new List<int>();
        query_list.Enqueue(anchor_id);
        anchor_list.Add(anchor_id);
        while (query_list.Count != 0) {
            int cur = query_list.Dequeue();
            for (int i = 0; i < AnchorCount; i++) {
                if (AnchorMap[cur, i] && !anchor_list.Contains(i)) {
                    anchor_list.Add(i);
                    query_list.Enqueue(i);
                }
            }
        }
        // Calculate mass center
        Vector3 mass_center = new Vector3();
        foreach (int index in anchor_list) {
            mass_center += transform.GetChild(index).position;
        }
        mass_center /= anchor_list.Count;
        return mass_center;
    }

    /// <summary>
    /// Get the mass center connected component containing the input anchor
    /// </summary>
    /// <param name="input_anchor"> Id of the anchor contained </param>
    /// <param name="anchor_count"> The number of anchors in connected component </param>
    /// <returns></returns>
    public Vector3 GetMassCenter(int anchor_id, out int anchor_count) {
        // Get the connected component
        // Based on BFS
        // The queue that stores querys.
        Queue<int> query_list = new Queue<int>();
        // The list that stores id of the anchors in connected component
        List<int> anchor_list = new List<int>();
        query_list.Enqueue(anchor_id);
        anchor_list.Add(anchor_id);
        while (query_list.Count != 0) {
            int cur = query_list.Dequeue();
            for (int i = 0; i < AnchorCount; i++) {
                if (AnchorMap[cur, i] && !anchor_list.Contains(i)) {
                    anchor_list.Add(i);
                    query_list.Enqueue(i);
                }
            }
        }
        // Calculate mass center
        Vector3 mass_center = new Vector3();
        foreach (int index in anchor_list) {
            mass_center += transform.GetChild(index).position;
        }
        mass_center /= anchor_list.Count;
        // Output
        anchor_count = anchor_list.Count;
        return mass_center;
    }
}
