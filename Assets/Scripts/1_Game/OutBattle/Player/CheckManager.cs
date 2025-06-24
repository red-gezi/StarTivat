using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    [ShowInInspector]
     List<InteractiveManager> interactives = new List<InteractiveManager>();
    public GameObject InteractiveUi;
    public static CheckManager Instance;
    private void Awake()
    {
        Instance= this;
        //transform.parent=transform.parent.GetChild(0);
    }
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            interactives.FirstOrDefault()?.Interactive();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        InteractiveManager target = other.GetComponent<InteractiveManager>();
        if (target != null&&target.CanTrigger)
        {
            interactives.Add(target);
            InteractiveUi.SetActive(true);
            Debug.Log(other.gameObject.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        InteractiveManager target = other.GetComponent<InteractiveManager>();
        if (target != null)
        {
            interactives.Remove(target);
            if (interactives.Count == 0)
            {
                InteractiveUi.SetActive(false);
            }
            Debug.Log(other.gameObject.name);
        }
    }
    public void RemoveInteractObject(InteractiveManager interactiveManager)
    {
        if (interactives.Contains(interactiveManager))
        {
            interactives.Remove(interactiveManager);
            if (interactives.Count == 0)
            {
                InteractiveUi.SetActive(false);
            }
        }
    }
}
