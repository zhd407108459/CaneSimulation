using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IVisitable
{
    List<IVisitor> visitors = new List<IVisitor>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) + OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >
            1.8f)
        {
            Debug.Log("Interaction Attempted!");
            // Visit any available Visitables stored
            foreach (var visitor in visitors)
            {
                Accept(visitor);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        // check for valid Visitable
        var newVisitors = other.GetComponents<IVisitor>();
        foreach (var visitor in newVisitors)
        {
            visitors.Add(visitor);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove related visitors
        var newVisitors = other.GetComponents<IVisitor>();
        foreach (var visitor in newVisitors)
        {
            visitors.Remove(visitor);
        }
    }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}