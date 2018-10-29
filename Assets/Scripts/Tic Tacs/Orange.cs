using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A tic tac flavor that is on the melee path route and punches the collector at fast intervals
 * with low damage. */
public class Orange : TicTac {

    public override SE.Flavor Flavor { get { return flavor; } }
    public override SE.PathRoute PathRoute { get { return pathRoute; } }
    public override SE.PathDistance CurrPathDistance {
        get { return currPathDistance; }
        set { currPathDistance = value; }
    }

    private const SE.Flavor flavor = SE.Flavor.Orange;
    private const SE.PathRoute pathRoute = SE.PathRoute.Melee;
    private SE.PathDistance currPathDistance = SE.PathDistance.None;
    private const int damage = 2;
    private const float speed = 1.5f;

    protected override float Speed { get { return speed; } }

    public override void Attack() {
        TicTac.collectorScript.Damage(damage);
    }

}