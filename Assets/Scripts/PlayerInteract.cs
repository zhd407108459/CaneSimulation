using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IVisitor
{
    List<IVisitable> visitables = new List<IVisitable>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) >
            1.8f)
        {
            Debug.Log("Interaction Attempted!");
            // Visit any available Visitables stored
            foreach (var visitable in visitables)
            {
                Visit(visitable);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        // Setup visitor programming pattern
        // check for valid Visitable
    }

    void OnTriggerExit(Collider other)
    {
        // Remove related visitors
    }

    public void Visit(IVisitable visitable)
    {
        throw new System.NotImplementedException();
    }
}