using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedPlatform : Activator
{
    protected override void Activate()
    {
        base.Activate();
        gameObject.SetActive(true);
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }
}
