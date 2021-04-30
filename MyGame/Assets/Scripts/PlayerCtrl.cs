using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 대검 콜라이더 생성 및 공격 시에만 콜라이더 setactive 하기
// enemy stat 이용해서 체력 만들어주고 공격 받으면 파괴되게 하기

public class PlayerCtrl : MonoBehaviour
{
    public Stat health;
    public Stat stamina;

    // private float RecoveryTime = 0; // 회복 시간
    private float initHealth = 100;
    private float initStamina = 100;
    public float moveSpeed;
    public float runSpeed;
    public float turnspeed;
    public GameObject player;

    Rigidbody rb;

    Animator anim;  // 애니메이터 컴포넌트를 담기 위한 변수

    bool isAttack; // 공격중인지 확인

    bool isAttackPose; // 발도중인지 확인

    bool isAttackBtnClick; // 공격키 입력 확인

    bool isWalk; // 걷는지 확인

    bool isRun; // 뛰는지 확인

    public bool isDodge; // 회피 확인

    bool isCrouch; // 앉기 확인

    bool isDodgeorCrouch;

    public Vector3 moveDir;
    public Vector3 dodgeDir;
    
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();  // 애니메이터 컴포넌트 가져오기
        isDodge = false;
        isAttackPose = false;
        isAttack = false;

        health.Initialize(initHealth, initHealth);
        stamina.Initialize(initStamina,initStamina);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(GameManager.instance.currentGameState == GameState.inGame)
        // {
            
        // }
        inputKey();
        Attack_St();
        if(!isAttack) // 공격중이 아닐때 이동
        {
            Move();
        }
        Dodge();
        if(!isAttackPose)
        {
            Crouch();
        }
        
        PrintAnimation();
    }

    private void Dodge()
    {
        if(!isDodge && isDodgeorCrouch && moveDir != Vector3.zero && stamina.MyCurrentValue >= 20f)
        {
            dodgeDir = moveDir;
            anim.SetTrigger("doDodge");
            isDodge=true;
            isCrouch=false;
            stamina.MyCurrentValue -= 20f;

            Invoke("DodgeOut",0.75f);
        }
    }
    private void DodgeOut()
    {
        isDodge=false;
    }

    private void Attack_St()
    {
        if(!isAttack && isAttackBtnClick && !isAttackPose)
        {
            isAttackPose = true;     
            anim.SetTrigger("StartAttack");
            anim.SetFloat("drawSword", 1.0f);
        }
        else if(isAttackPose && isAttackBtnClick)
        {
            anim.SetTrigger("doAttack");
            isAttack=true;
            Invoke("Attack_Fin",1.5f);
        }
        else if(isAttackPose && isRun)
        {
            isAttackPose=false;
            anim.SetFloat("drawSword", -1.0f);
        }
    }

    private void Attack_Fin()
    {
        isAttack=false;
    }

    private void Crouch()
    {
        if(!isCrouch && isDodgeorCrouch && moveDir == Vector3.zero)
        {
            anim.SetTrigger("doCrouch");
            isCrouch = true;
        }
        else if(isCrouch && isDodgeorCrouch && moveDir == Vector3.zero)
        {
            isCrouch = false;
        }
    }

    private void inputKey()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        isRun = Input.GetButton("Run");
        isDodgeorCrouch = Input.GetButtonDown("DodgeorCrouch");
        isAttackBtnClick = Input.GetButtonDown("Attack");
        isWalk = h!=0||v!=0;

        moveDir = new Vector3(h,0,v);
    }

    private void PrintAnimation()
    {
        // 공격 중일때는 애니메이션 불가
        anim.SetBool("isRun",isRun);
        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isCrouch",isCrouch);
        anim.SetBool("isAttackPose",isAttackPose);
    }

    void Move()
    {
        moveDir = new Vector3(Camera.main.transform.TransformDirection(moveDir).x,0,Camera.main.transform.TransformDirection(moveDir).z);         

        moveDir.Normalize();

        // 카메라 기준 캐릭터 무브 구현
        if(isDodge)
        {
            player.transform.rotation = Quaternion.RotateTowards (player.transform.rotation, Quaternion.LookRotation (dodgeDir), turnspeed * Time.deltaTime);
        }
        else if (moveDir != Vector3.zero)
        {
            player.transform.rotation = Quaternion.RotateTowards (player.transform.rotation, Quaternion.LookRotation (moveDir), turnspeed * Time.deltaTime);
        }


        // 달리기, 구르기 입력 여부에 따른 이동속도, 방향 차이
        if(isDodge)
        {
            transform.Translate(dodgeDir * runSpeed * Time.deltaTime,Space.World);
            isCrouch = false;
        }
        else if(isRun && stamina.MyCurrentValue > 0 && moveDir != Vector3.zero)
        {
            transform.Translate(moveDir * runSpeed * Time.deltaTime,Space.World);
            stamina.MyCurrentValue -= 10f * Time.deltaTime;
            isCrouch = false;
        }
        else if(isCrouch)
        {
            transform.Translate(moveDir * moveSpeed * 0.7f * Time.deltaTime,Space.World);
            stamina.MyCurrentValue += 5f * Time.deltaTime;
        }
        else if(isWalk)
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime,Space.World);
            stamina.MyCurrentValue += 5f * Time.deltaTime;
        }
        else
        {
            stamina.MyCurrentValue += 5f * Time.deltaTime;
        }
    }
}
