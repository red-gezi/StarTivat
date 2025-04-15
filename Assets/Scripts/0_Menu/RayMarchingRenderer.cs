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
        // 创建一个平面网格
        CreatePlaneMesh();

        // 将网格附加到当前游戏对象
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
        // 创建一个简单的平面网格
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
        // 获取屏幕尺寸
        int width = Screen.width;
        int height = Screen.height;

        // 创建一个纹理来存储结果
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        RenderTexture.active = renderTexture;

        // 创建一个临时渲染纹理
        RenderTexture tempTexture = new RenderTexture(width, height, 24);

        // 渲染Ray Marching
        Graphics.Blit(null, tempTexture, material);

        // 将结果复制到主纹理
        Graphics.Blit(tempTexture, renderTexture);

        // 清理临时纹理
        RenderTexture.ReleaseTemporary(tempTexture);

        // 将结果显示在屏幕上
        RenderTexture.active = null;
        Graphics.Blit(renderTexture, null as RenderTexture);  // 显式指定为 RenderTexture 类型

        // 清理渲染纹理
        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
