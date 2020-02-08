using UnityEngine;

/// <summary>
/// Plane管理脚本 —— 挂载新建的Camera上
/// </summary>
[ExecuteInEditMode] //编辑模式中执行
public class MirrorRenderer : MonoBehaviour {
    public GameObject mirrorPlane; //镜子Plane
    public bool estimateViewFrustum = true;
    public bool setNearClipPlane = true;   //是否设置近剪切平面
    public float nearClipDistanceOffset = -0.01f; //近剪切平面的距离
    private Camera mirrorCamera;                    //镜像摄像机
    private Vector3 vn;                              //屏幕的法线
    private float l;                               //到屏幕左边缘的距离
    private float r;                               //到屏幕右边缘的距离
    private float b;                               //到屏幕下边缘的距离
    private float t;                               //到屏幕上边缘的距离
    private float d;                               //从镜像摄像机到屏幕的距离
    private float n;                               //镜像摄像机的近剪切面的距离
    private float f;                               //镜像摄像机的远剪切面的距离
    private Vector3 pa;                              //世界坐标系的左下角
    private Vector3 pb;                              //世界坐标系的右下角
    private Vector3 pc;                              //世界坐标系的左上角
    private Vector3 pe;                              //镜像观察角度的世界坐标位置
    private Vector3 va;                              //从镜像摄像机到左下角
    private Vector3 vb;                              //从镜像摄像机到右下角
    private Vector3 vc;                              //从镜像摄像机到左上角
    private Vector3 vr;                              //屏幕的右侧旋转轴
    private Vector3 vu;                              //屏幕的上侧旋转轴
    private Matrix4x4 p = new Matrix4x4();
    private Matrix4x4 rm = new Matrix4x4();
    private Matrix4x4 tm = new Matrix4x4();
    private Quaternion q = new Quaternion();


    private void Start() {
        mirrorCamera = GetComponent<Camera>();
    }


    private void Update() {
        if (null == mirrorPlane || null == mirrorCamera) return;
        pa = mirrorPlane.transform.TransformPoint(new Vector3(-5.0f, 0.0f, -5.0f)); //世界坐标系的左下角
        pb = mirrorPlane.transform.TransformPoint(new Vector3(5.0f, 0.0f, -5.0f)); //世界坐标系的右下角
        pc = mirrorPlane.transform.TransformPoint(new Vector3(-5.0f, 0.0f, 5.0f));  //世界坐标系的左上角
        pe = transform.position;                                                    //镜像观察角度的世界坐标位置
        n = mirrorCamera.nearClipPlane;                                            //镜像摄像机的近剪切面的距离
        f = mirrorCamera.farClipPlane;                                             //镜像摄像机的远剪切面的距离
        va = pa - pe;                                                               //从镜像摄像机到左下角
        vb = pb - pe;                                                               //从镜像摄像机到右下角
        vc = pc - pe;                                                               //从镜像摄像机到左上角
        vr = pb - pa;                                                               //屏幕的右侧旋转轴
        vu = pc - pa;                                                               //屏幕的上侧旋转轴
        if (Vector3.Dot(-Vector3.Cross(va, vc), vb) < 0.0f)                         //如果看向镜子的背面
        {
            vu = -vu;
            pa = pc;
            pb = pa + vr;
            pc = pa + vu;
            va = pa - pe;
            vb = pb - pe;
            vc = pc - pe;
        }
        vr.Normalize();
        vu.Normalize();
        vn = -Vector3.Cross(vr, vu); //两个向量的叉乘，最后在取负，因为Unity是使用左手坐标系
        vn.Normalize();
        d = -Vector3.Dot(va, vn);
        if (setNearClipPlane) {
            n = d + nearClipDistanceOffset;
            mirrorCamera.nearClipPlane = n;
        }
        l = Vector3.Dot(vr, va) * n / d;
        r = Vector3.Dot(vr, vb) * n / d;
        b = Vector3.Dot(vu, va) * n / d;
        t = Vector3.Dot(vu, vc) * n / d;


        //投影矩阵
        p[0, 0] = 2.0f * n / (r - l);
        p[0, 1] = 0.0f;
        p[0, 2] = (r + l) / (r - l);
        p[0, 3] = 0.0f;

        p[1, 0] = 0.0f;
        p[1, 1] = 2.0f * n / (t - b);
        p[1, 2] = (t + b) / (t - b);
        p[1, 3] = 0.0f;

        p[2, 0] = 0.0f;
        p[2, 1] = 0.0f;
        p[2, 2] = (f + n) / (n - f);
        p[2, 3] = 2.0f * f * n / (n - f);

        p[3, 0] = 0.0f;
        p[3, 1] = 0.0f;
        p[3, 2] = -1.0f;
        p[3, 3] = 0.0f;

        //旋转矩阵
        rm[0, 0] = vr.x;
        rm[0, 1] = vr.y;
        rm[0, 2] = vr.z;
        rm[0, 3] = 0.0f;

        rm[1, 0] = vu.x;
        rm[1, 1] = vu.y;
        rm[1, 2] = vu.z;
        rm[1, 3] = 0.0f;

        rm[2, 0] = vn.x;
        rm[2, 1] = vn.y;
        rm[2, 2] = vn.z;
        rm[2, 3] = 0.0f;

        rm[3, 0] = 0.0f;
        rm[3, 1] = 0.0f;
        rm[3, 2] = 0.0f;
        rm[3, 3] = 1.0f;

        tm[0, 0] = 1.0f;
        tm[0, 1] = 0.0f;
        tm[0, 2] = 0.0f;
        tm[0, 3] = -pe.x;

        tm[1, 0] = 0.0f;
        tm[1, 1] = 1.0f;
        tm[1, 2] = 0.0f;
        tm[1, 3] = -pe.y;

        tm[2, 0] = 0.0f;
        tm[2, 1] = 0.0f;
        tm[2, 2] = 1.0f;
        tm[2, 3] = -pe.z;

        tm[3, 0] = 0.0f;
        tm[3, 1] = 0.0f;
        tm[3, 2] = 0.0f;
        tm[3, 3] = 1.0f;


        mirrorCamera.projectionMatrix = p; //矩阵组
        mirrorCamera.worldToCameraMatrix = rm * tm;
        if (!estimateViewFrustum) return;
        q.SetLookRotation((0.5f * (pb + pc) - pe), vu); //旋转摄像机
        mirrorCamera.transform.rotation = q;            //聚焦到屏幕的中心点

        //估值 —— 三目简写
        mirrorCamera.fieldOfView = mirrorCamera.aspect >= 1.0 ? Mathf.Rad2Deg * Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude) / va.magnitude) : Mathf.Rad2Deg / mirrorCamera.aspect * Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude) / va.magnitude);
        //在摄像机角度考虑，保证视锥足够宽
    }
}