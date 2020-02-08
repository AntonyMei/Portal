using UnityEngine;


/// <summary>
/// Attach to the mirror
/// </summary>
[ExecuteInEditMode]
public class MirrorManager : MonoBehaviour {
    public GameObject mirrorPlane;  //镜子
    public Camera mainCamera;   //主摄像机
    private Camera mirrorCamera; //镜像摄像机


    private void Start() {
        mirrorCamera = GetComponent<Camera>();
    }


    private void Update() {
        if (null == mirrorPlane || null == mirrorCamera || null == mainCamera) return;
        Vector3 postionInMirrorSpace = mirrorPlane.transform.InverseTransformPoint(mainCamera.transform.position); //将主摄像机的世界坐标位置转换为镜子的局部坐标位置
        postionInMirrorSpace.y = -postionInMirrorSpace.y;                                                    //一般y为镜面的法线方向
        mirrorCamera.transform.position = mirrorPlane.transform.TransformPoint(postionInMirrorSpace);                 //转回到世界坐标系的位置
    }
}
