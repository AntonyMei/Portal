using UnityEngine;
public class _CameraControllerInfinite : MonoBehaviour {
    class CameraRotation {
        public float yaw;
        public float pitch;
        public float roll;
        public void SetFromTransform(Transform t) {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
        }
        public void LerpTowards(CameraRotation target, float rotationLerpPct) {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
        }

        public void UpdateTransform(Transform t) {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
        }
    }

    CameraRotation m_TargetCameraState = new CameraRotation();
    CameraRotation m_InterpolatingCameraState = new CameraRotation();

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

    void OnEnable() { RefershCameraRotation(); }

    public void RefershCameraRotation() {
        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);
    }

    void Update() {
        // Disable Rotation when Mouse0 is pressed
        if(Input.GetKey(KeyCode.Mouse0)) { return; }
        // Rotation
        var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
        var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
        m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
        m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, rotationLerpPct);
        m_InterpolatingCameraState.UpdateTransform(transform);
    }
}