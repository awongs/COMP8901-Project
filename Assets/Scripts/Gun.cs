using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Bullet prefab to instantiate.
    public GameObject bulletObj;

    // Damage value for the gun.
    public int damage;

    public void Fire(Vector3 direction)
    {
        Character.Team team = transform.parent.GetComponent<Character>().team;

        GameObject bulletInstance = Instantiate(bulletObj);
        bulletInstance.transform.position = transform.position;
        bulletInstance.transform.forward = direction;
        bulletInstance.GetComponent<Bullet>().Activate(team, damage);
    }
}
