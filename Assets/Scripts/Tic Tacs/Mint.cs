using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A tic tac flavor that is on the ranged path route and shoots small projectiles at the collector
 * at fast intervals for low damage. */
public class Mint : TicTac {

    public override SE.Flavor Flavor { get { return flavor; } }
    public override SE.PathRoute PathRoute { get { return pathRoute; } }
    public override SE.PathDistance CurrPathDistance {
        get { return currPathDistance; }
        set { currPathDistance = value; }
    }
    public GameObject projectilePrefab;

    private const SE.Flavor flavor = SE.Flavor.Mint;
    private const SE.PathRoute pathRoute = SE.PathRoute.Ranged;
    private SE.PathDistance currPathDistance = SE.PathDistance.None;
    private const int damage = 2;
    private const float speed = .75f;
    private GameObject projectile;
    private Rigidbody projectileRigidbody;
    private const float projectileSpeed = 4.0f;
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

    protected override void Remove() {
        base.Remove();
        Destroy(projectile);
    }
}