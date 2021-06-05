using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_att_damaged : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            var tar = other.gameObject.GetComponent<PlayerCtrl>();

            if(tar.isDodge == false)
            {
                tar.health.MyCurrentValue -= 10;
            }
        }
    }
}
