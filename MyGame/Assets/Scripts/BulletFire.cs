using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : MonoBehaviour
{
    public GameObject bulletFactory;
    public int poolSize = 10;
    GameObject[] bulletObjectPool;

    public float attackDelay= 5f;
    public float currentTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        bulletObjectPool = new GameObject[poolSize];

        for(int i=0;i<poolSize;i++)
        {
            GameObject bullet = Instantiate(bulletFactory);
            bulletObjectPool[i] = bullet;
            bullet.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime > attackDelay)
        {
            currentTime = 0;
            for(int i =0; i<poolSize; i++)
            {
                GameObject bullet = bulletObjectPool[i];
                if(bullet.activeSelf == false)
                {
                    bullet.transform.position = transform.position;
                    bullet.SetActive(true);
                    break;
                }
            }
        }
    }
}
