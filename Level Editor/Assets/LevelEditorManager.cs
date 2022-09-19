using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public int CurrentButtonPressed;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        Vector3 worldPosition = Vector3.zero;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            worldPosition = raycastHit.point;
        }


        if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonPressed].Clicked)
        {
            ItemButtons[CurrentButtonPressed].Clicked = false;
            Instantiate(ItemPrefabs[CurrentButtonPressed], new Vector3(worldPosition.x, ItemButtons[CurrentButtonPressed].height, worldPosition.z), Quaternion.identity);
        }
    }
}
