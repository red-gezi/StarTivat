using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
[RequireComponent(typeof(InteractiveSystem))]
public class ChestManager : MonoBehaviour
{
    public GameObject point;
    public List<GameObject> bodys;
    List<Material> materials;
    // Start is called before the first frame update
    void Start()
    {
        materials = bodys.Select(body => body.GetComponent<Renderer>().material).ToList();
    }
    public async void OpenChest()
    {
        GetComponent<InteractiveSystem>().CloseTrigger();
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            point.transform.eulerAngles = Vector3.left * 90 * progress;
        });
        //触发打开箱子事件
        await Task.Delay(2000);
        await CustomThread.TimerAsync(1f, (progress) =>
        {
            materials.ForEach(material =>
            {
                material.SetFloat("_diffusion", Mathf.Pow(1 - progress, 2));
            });
        });
        Destroy(gameObject);
    }
}
