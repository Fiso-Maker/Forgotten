using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5;
    Vector3 dir;
    Transform playerPos;
    // Start is called before the first frame update
    void Start()
    {
    }
    void OnEnable()
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        dir = playerPos.transform.position - transform.position;
        dir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {   
        transform.Translate(dir * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            var tar = other.gameObject.GetComponent<PlayerCtrl>();
            tar.health.MyCurrentValue -= 10;

            gameObject.SetActive(false);
        }
        else if(other.gameObject.CompareTag("wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
