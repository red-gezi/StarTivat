using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();
    static SoundEffectManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void Play(string name)
    {
        var newSource = Instance.gameObject.AddComponent<AudioSource>();
        newSource.clip = Instance.clips.First(x => x.name == name);
        newSource.Play();
    }
}
