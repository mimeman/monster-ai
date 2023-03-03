using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBall : MonoBehaviour
{
    public float damage; //������

    void OnCollisionEnter(Collision collision)  //�����̻����� �浹���� ���
    {
        livingEntity attackTarget = collision.gameObject.GetComponent<livingEntity>();

        attackTarget.OnDamage(damage);
    }
}
