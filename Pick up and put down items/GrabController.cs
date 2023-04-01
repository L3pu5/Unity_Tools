using UnityEngine;

// Lepus' pick up and put down controller.
// This script uses the main Camera to project a ray through the centre of the screen
// You may wish to LOCK the player's mouse to the centre to use this.

// For my use case, I also have a 'HandTarget' which puts the object in my player's hand.
// This is just a transform/position but you could put this off the map to hide it from the player.
// Since I want to both have the player shown holding the item, hand projecting an image, I do it this way.


public class GrabController : MonoBehaviour
{
    /// <summary>
    /// The location that the item will bind to when picked up.
    /// </summary>
    public GameObject HandTarget;
    /// <summary>
    /// The speed in degrees to rotate per second using E and Q.
    /// </summary>
    public float RotationSpeed = 30.0f;
    
    /// <summary>
    /// The item we are currently hovering over with our mouse.
    /// </summary>
    GameObject hoveredObject = null;
    /// <summary>
    /// The furniture/surface we are currently hovering over with our mouse.
    /// </summary>
    GameObject hoveredFurnitureObject = null;
    /// <summary>
    /// The temporary object being a placeholder for our drop.
    /// </summary>
    GameObject previewObject = null;
    /// <summary>
    /// The gameObject we are holding.
    /// </summary>
    GameObject heldObject = null;
    /// <summary>
    /// The previous rotation state to inherit for our preview since we are picking up the object as-is.
    /// </summary>
    /// <returns></returns>
    Quaternion heldObjectPreviousRotation = Quaternion.Euler(Vector3.zero);
    /// <summary>
    /// A boolean for whether we are pointing at a up facing surface.
    /// </summary>
    bool canDrop = true;

    // Update is called once per frame
    void Update()
    {
        project_ray();
        handle_click();
        handle_object_manipulation();
    }

    private void handle_grab_item(Ray cameraRay)
    {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(cameraRay.origin, cameraRay.direction, out hitInfo, 10f, LayerMask.GetMask("Grab_Item")))
        {
            hoveredObject = hitInfo.transform.gameObject;
            while (hoveredObject.transform.parent != null && hoveredObject.transform.parent.name != "Trash")
            {
                hoveredObject = hoveredObject.transform.parent.gameObject;
            }
        }
    }

    private void enable_preview_object(bool enable)
    {
        if (previewObject == null)
            return;

        canDrop = enable;
        previewObject.SetActive(enable);
    }

    private bool hit_normal_is_up(RaycastHit hitInfo)
    {
        return (hitInfo.normal == Vector3.up);
    }

    private void handle_furniture_item(Ray cameraRay)
    {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(cameraRay.origin, cameraRay.direction, out hitInfo, 10f, LayerMask.GetMask("Grab_Furniture")))
        {
            hoveredFurnitureObject = hitInfo.transform.gameObject;
            while (hoveredFurnitureObject.transform.parent != null && hoveredFurnitureObject.transform.parent.name != "Trash")
            {
                hoveredFurnitureObject = hoveredFurnitureObject.transform.parent.gameObject;
            }
            if (hit_normal_is_up(hitInfo))
            {
                enable_preview_object(true);
                handle_hover_drop(hitInfo);
            }
        }
        else
        {
            enable_preview_object(false);
        }
    }


    private void handle_hover_drop(RaycastHit hitInfo)
    {
        //If we are holding something, show where we would drop it.
        if (heldObject!= null)
        {
            //If no preview object, make a new one
            if (previewObject == null)
            {
                previewObject = Instantiate(heldObject);
                //save_hovered_object_materials();
                previewObject.name = "TEMPORARY_PLACEMENT_PREVIEW";
                previewObject.transform.rotation = heldObjectPreviousRotation;
                previewObject.transform.localScale = new Vector3(1, 1, 1);
                previewObject.transform.position = hitInfo.point;
            }
            //Otherwise move to our new position
            else
            {
                previewObject.transform.position = hitInfo.point;
            }
        }
    }

    private void project_ray()
    {
        hoveredObject = null;
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        handle_furniture_item(cameraRay);
        handle_grab_item(cameraRay);
    }

    private void handle_object_manipulation()
    {
        if (previewObject != null && previewObject.activeInHierarchy)
        {
            if(Input.GetKey(KeyCode.E))
            {
                previewObject.transform.Rotate(0, RotationSpeed * Time.deltaTime,0);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                previewObject.transform.Rotate(0, -RotationSpeed * Time.deltaTime, 0);
            }
        }
    }

    private void handle_click() {
        if(hoveredObject != null && heldObject == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                heldObject = hoveredObject;
                heldObjectPreviousRotation = heldObject.transform.localRotation;
                heldObject.transform.parent = HandTarget.transform;
                heldObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 130f, 90f));
                heldObject.transform.localPosition = Vector3.zero;
                return;
            }
        }
        if(heldObject != null && canDrop)
        {
            if (Input.GetMouseButtonDown(0))
            {
                previewObject = null;
                GameObject.Destroy(heldObject);
            }
        }
    }
}