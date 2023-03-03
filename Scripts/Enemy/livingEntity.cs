using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class livingEntity : MonoBehaviour  //livingEntity -> 살아있는 개체
{
    //Entity클래스는 몬스터,플레이어한테 들어가는 공통클래스

    public float startingHealth; //시작 체력
    public float health;  //현재 체력
    public bool dead;     //사망 상태
    public event Action onDeath; //사망 시 발동할 이벤트



    //생명체가 활성화될 떄 상태를 리셋
    public virtual void OnEnable() //OnEnable은 호출되면 실행함  ex)gameObject.SetActive(true);
    {     
        dead = false ; //사망하지 않은 상태로 시작
        health = startingHealth; //체력을 시작 체력으로 초기화    
    }

    //피해를 받는 기능
    public virtual void OnDamage(float damage) //virtual로 쓴 이유는 상속 받는 클래스에서 재정의 할라꼬
    {
        //데미지만큼 체력 감소
        health -= damage; // health = health - damage;

        //체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    //사망 처리
    public virtual void Die()
    {      
        if (onDeath != null) onDeath(); //onDeath 이벤트에 등록된 메서드가 있다면 실행
        dead = true;
        Destroy(gameObject);
    }

}
