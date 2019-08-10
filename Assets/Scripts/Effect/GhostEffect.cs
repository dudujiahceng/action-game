using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour {
    class GhostItem : MonoBehaviour
    {
        public float duration;
        public float deleteTime;
        public MeshRenderer meshRenderer;

        void Update()
        {
            float tempTime = deleteTime - Time.time;
            if(tempTime <= 0)
            {
                GameObject.Destroy(gameObject);
            }
            else if(meshRenderer.material)
            {
                float rate = tempTime / duration;
                Color cal = meshRenderer.material.GetColor("_RimColor");
                cal.a *= rate;
                meshRenderer.material.SetColor("_RimColor", cal);
            }
        }
    }

    public float duration;
    public float interval;
    [Range(-1, 2)]
    public float intension;
    private float lasttime;
    private Vector3 lastPos;
    private bool startGhostEffect;

    private SkinnedMeshRenderer[] meshRenders;
    private Shader XRayShader;

    private void InitializeParams()
    {
        duration         = 1f;
        interval         = 0.1f;
        intension        = 1;
        lasttime         = 0;
        lastPos          = Vector3.zero;
        startGhostEffect = false;
    }
	void Start () {
        InitializeParams();
        meshRenders = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        XRayShader = Shader.Find("MyShader/XRayShader");
	}
	
	private void CreateGhost()
    {
        lastPos = transform.position;
        if(Time.time - lasttime < interval)
        {
            return;
        }
        lasttime = Time.time;
        if (meshRenders == null)
            return;
        for(int i = 0; i != meshRenders.Length; ++i)
        {
            Mesh mesh = new Mesh();
            meshRenders[i].BakeMesh(mesh);
            GameObject go = new GameObject();
            go.hideFlags = HideFlags.HideAndDontSave;

            GhostItem item = go.AddComponent<GhostItem>();
            item.duration = duration;
            item.deleteTime = Time.time + duration;

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            MeshRenderer meshRen = go.AddComponent<MeshRenderer>();
            meshRen.material = meshRenders[i].material;
            meshRen.material.shader = XRayShader;
            meshRen.material.SetFloat("_RimIntensity", intension);
            go.transform.localScale = meshRenders[i].transform.localScale;
            go.transform.position = meshRenders[i].transform.position;
            go.transform.rotation = meshRenders[i].transform.rotation;

            item.meshRenderer = meshRen;               
        }
    }

    public void StartGhostEffect()
    {
        startGhostEffect = true;
    }
    public void StopGhostEffect()
    {
        startGhostEffect = false;
    }
	void Update () {
		if(startGhostEffect)
        {
            CreateGhost();
        }
	}
}
