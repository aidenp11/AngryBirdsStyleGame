using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitBlueBird : Bird
{
    // Start is called before the first frame update
    protected override void Start()
    {
        //Main use is preventing setting of values done in Bird.Start()
        removeFromBirdList = true;
    }

    public override void OnThrow()
    {
        State = Assets.Scripts.BirdState.Thrown;
    }
}
