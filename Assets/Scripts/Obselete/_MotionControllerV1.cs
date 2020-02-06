using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The first version of MotionController, do not use this
/// </summary>
public class _MotionControllerV1 : MonoBehaviour {
    class PlayerState {
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t) {
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 translation) {
            x += translation.x;
            y += translation.y;
            z += translation.z;
        }

        public void LerpTowards(PlayerState target, float positionLerpPct) {
            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t) {
            t.position = new Vector3(x, y, z);
        }
    }

    PlayerState m_TargetCameraState = new PlayerState();
    PlayerState m_InterpolatingCameraState = new PlayerState();

    [Tooltip("The camera of the player")]
    public Camera PlayerCamera;

    [Header("Movement Settings")]
    [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
    public float boost = 3.5f;
    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;

    Vector3 GetInputTranslationDirection() {
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            direction += transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            direction -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A)) {
            direction -= transform.right;
        }
        if (Input.GetKey(KeyCode.D)) {
            direction += transform.right;
        }
        return direction;
    }

    void Update() {
        // Set rotation
        
        // Translation
        Vector3 translation = GetInputTranslationDirection() * Time.deltaTime;
        // Speed up movement when shift key held
        if (Input.GetKey(KeyCode.LeftShift)) {
            translation *= 10.0f;
        }
        // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
        boost += Input.mouseScrollDelta.y * 0.2f;
        translation *= Mathf.Pow(2.0f, boost);
        m_TargetCameraState.Translate(translation);
        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct);
        m_InterpolatingCameraState.UpdateTransform(transform);
    }
}