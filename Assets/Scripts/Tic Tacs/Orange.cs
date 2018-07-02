using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A tic tac flavor that is on the melee path route and punches the collector at fast intervals
 * with low damage. */
public class Orange : TicTac {

    public override TicTacTag.Flavor Flavor { get { return flavor; } }
    public override TicTacTag.PathRoute PathRoute { get { return pathRoute; } }
    public override TicTacTag.PathDistance PathDistance {
        get { return pathDistance; }
        set { pathDistance = value; }
    }

    private const TicTacTag.Flavor flavor = TicTacTag.Flavor.Orange;
    private const TicTacTag.PathRoute pathRoute = TicTacTag.PathRoute.Melee;
    private TicTacTag.PathDistance pathDistance = TicTacTag.PathDistance.None;
    private const float speed = 1.5f;

    protected override float Speed { get { return speed; } }

    public override void Attack() {
        //Debug.Log("Orange attacked!");
    }

}