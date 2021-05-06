using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5;

    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        // transform.LookAt(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            var tar = other.gameObject.GetComponent<PlayerCtrl>();
            tar.health.MyCurrentValue -= 10;

            gameObject.SetActive(false);
        }
        else if(other.gameObject.name.Contains("wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
