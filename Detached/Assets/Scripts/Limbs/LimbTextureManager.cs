using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbTextureManager : MonoBehaviour
{
    [SerializeField] Color detaBlue;
    [SerializeField] Color detaWhite;
    [SerializeField] Color chedBlack;
    [SerializeField] Color chedGold;
    private MeshRenderer rend;
    private Material[] materials;

    [SerializeField] 

    public void UpdateColor()
    {
        
        materials = rend.materials;
        CheckLimb();
    }

    public void ChangeColorOfLimb(ItemManager.Limb_enum limb, GameObject objectToChange, bool isdeta)
    {
        if (isdeta)
        {
            switch (limb) // Deta colors, based on the actual order on the prefabs
            {
                case ItemManager.Limb_enum.Arm:
                    materials[1].color = detaBlue;
                    materials[2].color = detaWhite;
                    break;
                case ItemManager.Limb_enum.Leg:
                    materials[0].color = detaWhite;
                    materials[1].color = detaBlue;
                    break;

                    /*Ändra bool på när limbs tas upp.
                     Vid hooken på detached så ändra smaitidgt färgen på benet.
                     Ändra dropp färgen.
                     Lägg till Texture manager */
            }

        }
        else
        {
            switch (limb)
            {
                case ItemManager.Limb_enum.Arm:
                    materials[1].color = chedGold;
                    materials[2].color = chedBlack;
                    break;
                case ItemManager.Limb_enum.Leg:
                    materials[0].color = chedBlack;
                    materials[1].color = chedGold;
                    break;                  
            }
        }

    }

    public void CheckLimb()
    {
        if (GetComponentInParent<SceneObjectItemManager>().isDeta)
        {
            switch (gameObject.transform.GetChild(0).gameObject.tag) // Deta colors, based on the actual order on the prefabs
            {
                case "Arm":
                    materials[1].color = detaBlue;
                    materials[2].color = detaWhite;
                    break;
                case "Leg":
                    materials[0].color = detaWhite;
                    materials[1].color = detaBlue;
                    break;
                case "Head":
                    materials[2].color = detaWhite;
                    materials[3].color = detaBlue;
                    break;

            }
        }
        else
        {
            switch (gameObject.transform.GetChild(0).gameObject.tag) // ched colors, based on the actual order on the prefabs
            {
                case "Arm":
                    materials[1].color = chedGold;
                    materials[2].color = chedBlack;
                    break;
                case "Leg":
                    materials[0].color = chedBlack;
                    materials[1].color = chedGold;
                    break;
                case "Head":
                    materials[2].color = chedBlack;
                    materials[3].color = chedGold;
                    break;

            }
        }
    }
}
