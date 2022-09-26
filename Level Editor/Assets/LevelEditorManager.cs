using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public GameObject[] ItemImage;
    public int CurrentButtonPressed;

    public int CurrentObjectSelected;
    private GameObject selectedObject;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        Vector3 worldPosition = Vector3.zero;
        worldPosition = GetMousePosition3D();


        if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonPressed].Clicked)
        {
            ItemButtons[CurrentButtonPressed].Clicked = false;
            Instantiate(ItemPrefabs[CurrentButtonPressed], new Vector3(worldPosition.x, ItemButtons[CurrentButtonPressed].height, worldPosition.z), Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("ItemImage"));
        }
        ObjectSelection();
    }

    private void ObjectSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject.GetComponent<Target>() != null)
                {
                    selectedObject = hitInfo.collider.gameObject;
                    //selectedObject.transform.parent.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }

    public Vector3 GetMousePosition3D()
    {
        Vector3 worldPosition = Vector3.zero;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            worldPosition = raycastHit.point;
        }

        return worldPosition;
    }
}
