using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleInteraction();
    }

    private void handleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitinfo, InteractRange))
            {
                if (hitinfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}

