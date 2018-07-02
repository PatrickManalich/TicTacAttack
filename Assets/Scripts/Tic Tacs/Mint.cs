using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A tic tac flavor that is on the ranged path route and shoots small bullets at the collector
 * at fast intervals for low damage. */
public class Mint : TicTac {

    public override TicTacTag.Flavor Flavor { get { return flavor; } }
    public override TicTacTag.PathRoute PathRoute { get { return pathRoute; } }
    public override TicTacTag.PathDistance PathDistance {
        get { return pathDistance; }
        set { pathDistance = value; }
    }
    public GameObject bulletPrefab;

    private const TicTacTag.Flavor flavor = TicTacTag.Flavor.Mint;
    private const TicTacTag.PathRoute pathRoute = TicTacTag.PathRoute.Ranged;
    private TicTacTag.PathDistance pathDistance = TicTacTag.PathDistance.None;
    private const float speed = .75f;
    private GameObject bullet;
    private Rigidbody bulletRigidbody;
    private const float bulletSpeed = 6.0f;
    private const float bulletArc = 2.0f;

    private void Awake() {
        bullet = Instantiate(bulletPrefab);
        bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bullet.GetComponent<Renderer>().material.color = transform.GetChild(0).GetComponent<Renderer>().material.color;
        bullet.SetActive(false);
    }

    protected override float Speed { get { return speed; } }

    public override void Attack() {
        if (!bullet.activeSelf)
            bullet.SetActive(true);
        bullet.transform.position = transform.position;
        bulletRigidbody.velocity = transform.forward * bulletSpeed + Vector3.up * bulletArc;
    }

    public override void Remove() {
        base.Remove();
        Destroy(bullet);
    }
}