using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {
    public EventSystem eventSystem;
    public GameObject selectedObject;

    private bool buttonSelected;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetAxisRaw("Vertical") != 0 && !buttonSelected)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        } else if ( (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) && buttonSelected )
        {
            eventSystem.SetSelectedGameObject(null);
            buttonSelected = false;
        }
    }

    private void OnDisable()
    {
        Debug.Log("Script was removed");
        buttonSelected = false;
    }
}