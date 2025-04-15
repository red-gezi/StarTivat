using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RayMarchingRenderer : MonoBehaviour
{
    public Camera mainCamera;
    public Shader rayMarchingShader;
    public Material material;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private void Start()
    {
        // ����һ��ƽ������
        CreatePlaneMesh();

        // �����񸽼ӵ���ǰ��Ϸ����
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer.material = material;
        meshFilter.mesh = mesh;
    }

    private void Update()
    {
        RenderRayMarching();
    }

    private void CreatePlaneMesh()
    {
        // ����һ���򵥵�ƽ������
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f, 0f, -1f),
            new Vector3(1f, 0f, -1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3(1f, 0f, 1f)
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f)
        };

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void RenderRayMarching()
    {
        // ��ȡ��Ļ�ߴ�
        int width = Screen.width;
        int height = Screen.height;

        // ����һ���������洢���
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        RenderTexture.active = renderTexture;

        // ����һ����ʱ��Ⱦ����
        RenderTexture tempTexture = new RenderTexture(width, height, 24);

        // ��ȾRay Marching
        Graphics.Blit(null, tempTexture, material);

        // ��������Ƶ�������
        Graphics.Blit(tempTexture, renderTexture);

        // ������ʱ����
        RenderTexture.ReleaseTemporary(tempTexture);

        // �������ʾ����Ļ��
        RenderTexture.active = null;
        Graphics.Blit(renderTexture, null as RenderTexture);  // ��ʽָ��Ϊ RenderTexture ����

        // ������Ⱦ����
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
