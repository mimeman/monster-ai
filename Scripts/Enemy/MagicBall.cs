using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBall : MonoBehaviour
{
    public float damage; //데미지

    void OnCollisionEnter(Collision collision)  //매직미사일이 충돌했을 경우
    {
        livingEntity attackTarget = collision.gameObject.GetComponent<livingEntity>();

        attackTarget.OnDamage(damage);
    }
}
