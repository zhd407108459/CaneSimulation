using UnityEngine;

public class Interactable : MonoBehaviour, IVisitable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Accept(IVisitor visitor)
    {
        throw new System.NotImplementedException();
    }
}
