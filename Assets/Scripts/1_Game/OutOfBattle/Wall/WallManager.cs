using UnityEngine;

public class WallManager : MonoBehaviour
{
    Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = transform.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_pos", PlayerManager.Instance.transform.position);
    }
}
