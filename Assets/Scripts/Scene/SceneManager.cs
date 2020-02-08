using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// <para> Scene manager is used for infinite scene. </para>
/// </summary>
public class SceneManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The player")]
    public GameObject Player;
    [Tooltip("The script GenerateObject")]
    public GenerateObject GenerateObject;
    [Tooltip("Maxium block number(Cube) rendered at the same time (Must be odd)")]
    public int MaxiumRenderingBlockCount = 5;
    [Tooltip("The side length of a block")]
    public int BlockSideLength = 500;

    // Half of (maximum block count - 1)
    private int half_len;
    // The root for all generated objects in the scene
    private GameObject root;
    // The array that saves the roots of each block
    private GameObject[,,] RootMap;

    void Start() { 
        // Initialize scene manager
        root = new GameObject("Root");
        MaxiumRenderingBlockCount += MaxiumRenderingBlockCount % 2 == 0 ? 1 : 0;
        half_len = (MaxiumRenderingBlockCount - 1) / 2;
        // Initialize root map
        RootMap = new GameObject[501, 501, 501];
        for(int i = 0; i < 501; i++) {
            for(int j = 0; j < 501; j++) {
                for(int k = 0; k < 501; k++) {
                    RootMap[i, j, k] = null;
                }
            }
        }
    }

    void Update() {
        // Calculate the block the player is in
        Vector3 player_pos = Player.transform.position;
        int x_id = GetBlockId((int)player_pos.x, false);
        int y_id = GetBlockId((int)player_pos.y, true);
        int z_id = GetBlockId((int)player_pos.z, false);
        // Destroy objects that are outside randering distance.
        // If the player moves through move than one block in a frame, this function will go wrong.
        for (int x = Mathf.Max(x_id - half_len - 1, 0); x <= Mathf.Min(x_id + half_len + 1, 500); x++) {
            for (int y = Mathf.Max(y_id - half_len - 1, 0); y <= Mathf.Min(y_id + half_len + 1, 500); y++) {
                if (z_id + half_len + 1 <= 500 && RootMap[x, y, z_id + half_len + 1]) {
                    GameObject.Destroy(RootMap[x, y, z_id + half_len + 1]);
                    RootMap[x, y, z_id + half_len + 1] = null;
                }
                if (z_id - half_len - 1 >= 0 && RootMap[x, y, z_id - half_len - 1]) {
                    GameObject.Destroy(RootMap[x, y, z_id - half_len - 1]);
                    RootMap[x, y, z_id - half_len - 1] = null;
                }
            }
        }
        for (int x = Mathf.Max(x_id - half_len - 1, 0); x <= Mathf.Min(x_id + half_len + 1, 500); x++) {
            for (int z = Mathf.Max(z_id - half_len - 1, 0); z <= Mathf.Min(z_id + half_len + 1, 500); z++) {
                if (y_id + half_len + 1 <= 500 && RootMap[x, y_id + half_len + 1, z]) {
                    GameObject.Destroy(RootMap[x, y_id + half_len + 1, z]);
                    RootMap[x, y_id + half_len + 1, z] = null;
                }
                if (y_id - half_len - 1 >= 0 && RootMap[x, y_id - half_len - 1, z]) {
                    GameObject.Destroy(RootMap[x, y_id - half_len - 1, z]);
                    RootMap[x, y_id - half_len - 1, z] = null;
                }
            }
        }
        for (int y = Mathf.Max(y_id - half_len - 1, 0); y <= Mathf.Min(y_id + half_len + 1, 500); y++) {
            for (int z = Mathf.Max(z_id - half_len - 1, 0); z <= Mathf.Min(z_id + half_len + 1, 500); z++) {
                if (x_id + half_len + 1 <= 500 && RootMap[x_id + half_len + 1, y, z]) {
                    GameObject.Destroy(RootMap[x_id + half_len + 1, y, z]);
                    RootMap[x_id + half_len + 1, y, z] = null;
                }
                if (x_id - half_len - 1 >= 0 && RootMap[x_id - half_len - 1, y, z]) {
                    GameObject.Destroy(RootMap[x_id - half_len - 1, y, z]);
                    RootMap[x_id - half_len - 1, y, z] = null;
                }
            }
        }
        // Create objects that previously are not rendered
        for (int x = Mathf.Max(x_id - half_len, 0); x <= Mathf.Min(x_id + half_len, 500); x++)
            for (int y = Mathf.Max(y_id - half_len, 0); y <= Mathf.Min(y_id + half_len, 500); y++) 
                for (int z = Mathf.Max(z_id - half_len, 0); z <= Mathf.Min(z_id + half_len, 500); z++) {
                    if (!RootMap[x, y, z]) {
                        // Set seed
                        int seed = x * 100 + y * 10 + z;
                        UnityEngine.Random.seed = seed;
                        // Generate Local root
                        GameObject local_root = new GameObject();
                        local_root.name = x.ToString() + y.ToString() + z.ToString();
                        local_root.transform.parent = root.transform;
                        // Add Models
                        GameObject portal_root = GenerateObject.GeneratePortal(
                            new GenerateObject.Range((x - 250) * BlockSideLength, (x - 249) * BlockSideLength),
                            new GenerateObject.Range(y * BlockSideLength, (y + 1) * BlockSideLength),
                            new GenerateObject.Range((z - 250) * BlockSideLength, (z - 249) * BlockSideLength));
                        portal_root.transform.parent = local_root.transform;
                        GameObject anchor_root = GenerateObject.GenerateAnchor(
                            new GenerateObject.Range((x - 250) * BlockSideLength, (x - 249) * BlockSideLength),
                            new GenerateObject.Range(y * BlockSideLength, (y + 1) * BlockSideLength),
                            new GenerateObject.Range((z - 250) * BlockSideLength, (z - 249) * BlockSideLength));
                        anchor_root.transform.parent = local_root.transform;
                        GameObject plane_root = GenerateObject.GeneratePlane(
                            new GenerateObject.Range((x - 250) * BlockSideLength, (x - 249) * BlockSideLength),
                            new GenerateObject.Range(y * BlockSideLength, (y + 1) * BlockSideLength),
                            new GenerateObject.Range((z - 250) * BlockSideLength, (z - 249) * BlockSideLength));
                        plane_root.transform.parent = local_root.transform;
                        GameObject trampoline_root = GenerateObject.GenerateTrampoline(
                            new GenerateObject.Range((x - 250) * BlockSideLength, (x - 249) * BlockSideLength),
                            new GenerateObject.Range(y * BlockSideLength, (y + 1) * BlockSideLength),
                            new GenerateObject.Range((z - 250) * BlockSideLength, (z - 249) * BlockSideLength));
                        trampoline_root.transform.parent = local_root.transform;
                        // Save the root in map
                        RootMap[x, y, z] = local_root;
                    }
                }
    }

    /// <summary>
    /// <para> Gets the block id for a given position on an axis </para>
    /// </summary>
    /// <param name="pos"> Position on the axis </param>
    /// <param name="isY"> Is y-axis. Y-axis position are always positive. </param>
    /// <returns> The </returns>
    int GetBlockId(int pos, bool isY) => (pos >= 0 ? pos / BlockSideLength :
            -((-pos) / BlockSideLength + 1)) + (isY ? 0 : 250);
}
