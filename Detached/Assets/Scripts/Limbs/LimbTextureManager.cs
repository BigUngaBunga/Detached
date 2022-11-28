using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbTextureManager : MonoBehaviour
{
    [SerializeField] Color detaBlue;
    [SerializeField] Color detaWhite;
    [SerializeField] Color chedBlack;
    [SerializeField] Color chedGold;
    private SkinnedMeshRenderer rend;
    private Material[] materials;
  
    public void ChangeColorOfLimb(ItemManager.Limb_enum limb, GameObject objectToChange, bool isdeta)
    {
        UpdateMaterials(objectToChange);
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

                    /*�ndra bool p� n�r limbs tas upp.
                     Vid hooken p� detached s� �ndra smaitidgt f�rgen p� benet.
                     �ndra dropp f�rgen.
                     L�gg till Texture manager */
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

    private void UpdateMaterials(GameObject obj)
    {
        rend = obj.GetComponent<SkinnedMeshRenderer>();
        materials = rend.materials;
    }  
}
