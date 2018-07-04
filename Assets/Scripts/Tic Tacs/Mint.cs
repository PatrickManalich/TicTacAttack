using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A tic tac flavor that is on the ranged path route and shoots small projectiles at the collector
 * at fast intervals for low damage. */
public class Mint : TicTac {

    public override TicTacTag.Flavor Flavor { get { return flavor; } }
    public override TicTacTag.PathRoute PathRoute { get { return pathRoute; } }
    public override TicTacTag.PathDistance PathDistance {
        get { return pathDistance; }
        set { pathDistance = value; }
    }
    public GameObject projectilePrefab;

    private const TicTacTag.Flavor flavor = TicTacTag.Flavor.Mint;
    private const TicTacTag.PathRoute pathRoute = TicTacTag.PathRoute.Ranged;
    private TicTacTag.PathDistance pathDistance = TicTacTag.PathDistance.None;
    private const int damage = 2;
    private const float speed = .75f;
    private GameObject projectile;
    private Rigidbody projectileRigidbody;
    private const float projectileSpeed = 6.0f;
    private const float projectileArc = 2.0f;

    private void Awake() {
        projectile = Instantiate(projectilePrefab);
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectile.GetComponent<Renderer>().material.color = transform.GetChild(0).GetComponent<Renderer>().material.color;
        projectile.SetActive(false);
    }

    protected override float Speed { get { return speed; } }

    public override void Attack() {
        if (!projectile.activeSelf)
            projectile.SetActive(true);
        projectile.transform.position = transform.position;
        projectileRigidbody.velocity = transform.forward * projectileSpeed + Vector3.up * projectileArc;
        TicTac.collectorScript.Damage(damage);
    }

    public override void Remove() {
        base.Remove();
        Destroy(projectile);
    }
}