using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
   public enum Type{Melee, Range};
   public Type type;
   public int damage;
   public float rate;
   public BoxCollider meleeArea;

    public void use()
    {
        if(type == Type.Melee)
        {
            StartCoroutine("Swing");
        }
    }
    IEnumerator Swing(){
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1.4f);
        meleeArea.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            print("attack_success");
            var tar = other.gameObject.GetComponent<Enemy>();
            tar.health.MyCurrentValue -= 10;

            meleeArea.enabled = false;
        }
    }
}
