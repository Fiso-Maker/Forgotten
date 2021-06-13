using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//���� ���� ����
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

    BulletFire fire;

    public GameObject bulletPos;
    public GameObject jump_att_position;

    public float jumpDelay = 2f;
    public float patternDelay = 5f;
    public float currentTime = 0f;
    public float attackRange = 10f;

    public bool isDie = false;
    public enum State
    {
        idle,
        move,
        attack_jump,
        attack_shoot
    }
    public State state = State.idle;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();

        target = GameObject.Find("Player").transform;

        health.Initialize(initHealth, initHealth);

        bulletPos.SetActive(false);

        jump_att_position.SetActive(false);

        isDie = false;
    }

    // Update is called once per frame
    void Update()
    {

        
        health_check();
        rotate();
        if(!isDie)
        {
            gravity_check();
            switch (state)
            {
                case State.idle:
                    idle();
                    // print("idle");
                    break;
                case State.move:
                    move();
                    // print("move");
                    break;
                case State.attack_jump:
                    att_jump();
                    // print("jump");
                    break;
                case State.attack_shoot:
                    att_shoot();
                    // print("shoot");
                    break;
            }
        }
        

    }

    void rotate()
    {
        if(isDie)
        {
            float rotatespeed = 2f;
            this.transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,0,90),rotatespeed*Time.deltaTime);
        }
        else
        {
            dir = target.position - transform.position;

            dir.y=0;
        
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);
        }
        
    }
    void gravity_check()
    {
        cc.Move(Vector3.up * yVelocity);

        if(cc.isGrounded)
        {
            yVelocity = 0;
            jump_att_position.SetActive(false);
        }
        else
        {
            yVelocity += gravity*Time.deltaTime;
        }
    }
    void idle()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= patternDelay)
        {
            currentTime = 0;
            
             // 랜덤하게 0부터 1까지, 0은 움직임 및 점프 공격 패턴, 1은 총알 발사 패턴
            int a = Random.Range(0, 2);
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
        
        // print(distance);

        cc.SimpleMove(dir.normalized * speed);

        if (distance < attackRange)
        {
            currentTime = 0;
            state = State.attack_jump;
        }
        currentTime += Time.deltaTime;
        if(currentTime >= patternDelay)
        {
            currentTime = 0;
            
            state = State.attack_shoot;
        }
    }
    void att_jump()
    {
        yVelocity = jumpPower; // 점프

        currentTime += Time.deltaTime;
        if(currentTime >= jumpDelay) // 시간 체크
        {
            currentTime = 0;
            
            jump_att_position.SetActive(true);
            // 점프 지점 전체 데미지용 콜라이더 활성화
            state = State.idle;
        }
    }
    void att_shoot()
    {
        // 총알 발사 스크립트 활성화
        bulletPos.SetActive(true);
        currentTime += Time.deltaTime;
        if(currentTime >= patternDelay) // 시간 체크
        {
            currentTime = 0;
            
            // 비활성화
            bulletPos.SetActive(false);

            state=State.idle;
        }
    }
    void health_check()
    {
        if(health.MyCurrentValue == 0)
        {
            StartCoroutine("Die");
        }
    }
    IEnumerator Die(){
        if(!isDie)
        {
            isDie = true;
            yield return new WaitForSeconds(3f);
            GameManager.instance.GameOver_Win();
        }
        
    }
}
