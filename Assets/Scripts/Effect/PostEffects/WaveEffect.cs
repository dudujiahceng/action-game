using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEffect : PostEffectsBase {

    public Shader waveShader;
    public Material waveMaterial = null;
    private Vector3 center;
    public float waveStrength;
    public float waveFactor;
    public float timeScale;
    public float waveRadius;

    private float waveStartTime;
    public float waveSpeed;
    public float waveWidth;

    public bool shockWave = false;


    void Awake()
    {
        shockWave       = false;
        waveStrength    = 0.1f;
        waveFactor      = 5;
        timeScale       = -10;
        waveRadius      = 20;
        waveSpeed       = 15;
        waveWidth       = 2;
    }

    IEnumerator timeCount(float time)
    {
        yield return new WaitForSeconds(time);
        shockWave = false;
    }
    //Start wave effect from external command
    public void startShockWave(Vector3 centerPos)
    {
        shockWave = true;
        center = centerPos;
        StartCoroutine(timeCount(1));
    }

    public Material material
    {
        get
        {
            waveMaterial = CheckShaderAndCreateMaterial(waveShader, waveMaterial);
            return waveMaterial;
        }
    }
    //为了获得摄像机相关参数
    private Camera myCamera;
    //Get carmer component
    public Camera camera
    {
        get
        {
            if (myCamera == null)
            {
                myCamera = GetComponent<Camera>();
            }
            return myCamera;
        }
    }
    private Transform myCameraTransform;
    //Get camera transform
    public Transform cameraTransform
    {
        get
        {
            if (myCameraTransform == null)
            {
                myCameraTransform = camera.transform;
            }

            return myCameraTransform;
        }
    }
    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (shockWave && material != null)
        {
            Matrix4x4 frustumCorners = Matrix4x4.identity;
            float fov = camera.fieldOfView;
            float near = camera.nearClipPlane;
            float aspect = camera.aspect;

            float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            Vector3 toRight = cameraTransform.right * halfHeight * aspect;
            Vector3 toTop = cameraTransform.up * halfHeight;

            Vector3 topLeft = cameraTransform.forward * near + toTop - toRight;
            float scale = topLeft.magnitude / near;

            topLeft.Normalize();
            topLeft *= scale;

            Vector3 topRight = cameraTransform.forward * near + toRight + toTop;
            topRight.Normalize();
            topRight *= scale;

            Vector3 bottomLeft = cameraTransform.forward * near - toTop - toRight;
            bottomLeft.Normalize();
            bottomLeft *= scale;

            Vector3 bottomRight = cameraTransform.forward * near + toRight - toTop;
            bottomRight.Normalize();
            bottomRight *= scale;

            frustumCorners.SetRow(0, bottomLeft);
            frustumCorners.SetRow(1, bottomRight);
            frustumCorners.SetRow(2, topRight);
            frustumCorners.SetRow(3, topLeft);

            float curWaveDistance = (Time.time - waveStartTime) * waveSpeed;
            material.SetMatrix("_FrustumCornersRay", frustumCorners);
            material.SetFloat("_CurWaveDistance", curWaveDistance);

            material.SetFloat("_WaveStrength", waveStrength);
            material.SetFloat("_WaveFactor", waveFactor);
            material.SetFloat("_TimeScale", timeScale);
            material.SetFloat("_WaveRadius", waveRadius);
            material.SetVector("_CenterPos", center);
            material.SetFloat("_WaveWidth", waveWidth);
            Graphics.Blit(src, dest, material);
        }
        else
            Graphics.Blit(src, dest);
    }

    void BaseWaveEffect()
    {            
    }

    void ForwardWaveEffect()
    {

    }

    void Update()
    {
        if(!shockWave)
        {
            waveStartTime = Time.time;
        }
    }
}
