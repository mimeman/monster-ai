using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject monster1;
    [SerializeField]
    private GameObject monster2;

    [SerializeField]
    private Transform playerTarget;

    public float spawnDelayTime;
    private int checkNumOfMonster;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;

        spawnDelayTime = 0;
    }

    void Update()
    {
        spawnDelayTime += Time.deltaTime;
        if (spawnDelayTime >= 2f)
        {
            while (checkNumOfMonster < 3)
            {
                SpawnMonster();
            }
        }
    }

    public void SpawnMonster()
    {
        float xPos = Random.RandomRange(-14f, 14f);
        float zPos = Random.RandomRange(-45f, 45f);

        Vector3 MonsterSpawnerPos = new Vector3(xPos, 3, zPos);

        this.gameObject.transform.position = MonsterSpawnerPos;

        GameObject monster = Instantiate(RandomMonster(), transform.position, transform.rotation);

        checkNumOfMonster++;

        monster.transform.LookAt(playerTarget);

        spawnDelayTime = 0;
    }

    public GameObject RandomMonster()
    {
        int whoMonster = Random.RandomRange(0, 2) + 1;
        if (whoMonster == 1)
        {
            return monster1;
        }
        else if (whoMonster == 2)
        {
            return monster2;
        }
        return monster1;
    }

}