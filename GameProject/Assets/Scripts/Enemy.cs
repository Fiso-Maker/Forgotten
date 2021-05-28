using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//몬스터 점프 구현
public class Enemy : MonoBehaviour
{
    public Stat health;
    private float initHealth = 100;

    CharacterController cc;

    Transform target;

    public float speed = 5;
    public float rotSpeed = 5;
    public float yVelocity = 0;
    public float gravity = -20;
    public float jumpPower;

    Vector3 dir;
    float distance;

    public float patternDelay = 5f;
    public float currentTime = 0f;
    public float attackRange = 10f;
    public enum State
    {
        idle,
        move,
        attack_jump,
        attack_shoot,
        die
    }
    public State state = State.idle;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();

        target = GameObject.Find("Player").transform;

        health.Initialize(initHealth, initHealth);
    }

    // Update is called once per frame
    void Update()
    {
        dir = target.position - transform.position;

        if (cc.isGrounded)
        {
            yVelocity = 0;
        }

        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        switch (state)
            {
            case State.idle:
                idle();
                print("idle");
                break;
            case State.move:
                move();
                print("move");
                break;
            case State.attack_jump:
                att_jump();
                print("jump");
                break;
            case State.attack_shoot:
                att_shoot();
                print("shoot");
                break;
            case State.die:
                die();
                print("die");
                break;
            }

        cc.SimpleMove(Vector3.up * dir.y);

        dir.y = 0;
        // 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

    }

    void idle()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= patternDelay)
        {
            currentTime = 0;
            
            int a = Random.Range(0, 2); // 0부터 1까지

            if (a==0)
            {
                state = State.move;
            }
            else if(a==1)
            {
                state = State.attack_shoot;
            }
        }
    }
    void move()
    {
        distance = dir.magnitude;
        
        print(distance);

        cc.SimpleMove(dir.normalized * speed);

        if (distance < attackRange)
        {
            state = State.attack_jump;
        }
    }
    void att_jump()
    {

        if (Vector3.Distance(target.position, transform.position) > attackRange)
        {
            state = State.attack_shoot;
        }
    }
    void att_shoot()
    {

    }
    void die()
    {

    }
   

}
