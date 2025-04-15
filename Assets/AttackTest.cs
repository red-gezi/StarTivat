using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTest : MonoBehaviour
{
    public GameObject basePrefab;
    public GameObject showPrefab;
    public float value;
    public float scale;
    [Button("´¥·¢¹¥»÷Ð§¹û")]
    async void AttackEffect()
    {
        Mesh bashMesh = basePrefab.GetComponent<MeshFilter>().mesh;
        Mesh showMesh = showPrefab.GetComponent<MeshFilter>().mesh;
        Vector3[] originalVertices = bashMesh.vertices;
        showMesh.SetVertices(originalVertices);
        showPrefab.GetComponent<Renderer>().material.color = Color.white;
        await CustomThread.TimerAsync(0.4f, (progress) =>
        {
            showPrefab.transform.localScale = Vector3.one * scale * progress;
            showPrefab.transform.position = Vector3.Lerp(-Vector3.one * scale, Vector3.zero, progress);
        });
        Vector3[] targetVertices = new Vector3[originalVertices.Length];
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                var scale = 1 + Mathf.Cos(Mathf.PI / 2 * bashMesh.vertices[i].z) * value * progress;
                targetVertices[i] = new Vector3(originalVertices[i].x * scale, originalVertices[i].y * scale, originalVertices[i].z);
            }
            showMesh.SetVertices(targetVertices);
        });
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                var scale = 1 + Mathf.Cos(Mathf.PI / 2 * bashMesh.vertices[i].z) * value * (1 - progress);
                targetVertices[i] = new Vector3(originalVertices[i].x * scale, originalVertices[i].y * scale, originalVertices[i].z);
            }
            showMesh.SetVertices(targetVertices);
        });
        await CustomThread.TimerAsync(0.3f, (progress) =>
        {
            showPrefab.transform.localScale = Vector3.one * scale * Mathf.Lerp(1, 0.5f, progress);
        });
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            showPrefab.transform.localScale = Vector3.one * scale * Mathf.Lerp(0.5f, 2, progress);
            showPrefab.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1 - progress);
        });
    }
}
