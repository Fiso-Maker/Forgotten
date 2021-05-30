using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터 점프 공격 구현
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
    public float RecoveryTime = 0f; // 스테미너 회복 시간
    public GameObject player;

    public GameObject weapon;
    public GameObject unequip;
    Rigidbody rb;

    Animator anim;  // 애니메이터 컴포넌트를 담기 위한 변수

    bool isAttack; // 공격중인지 확인

    bool isAttackPose; // 발도상태인지 확인

    bool isAttackBtnClick; // 공격키 입력 확인

    bool isAttackPose_Change;   //발도 or 납도 중

    bool isWalk; // 걷는지 확인

    bool isRun; // 뛰는지 확인

    bool isRunBtn; // 뛰기 버튼 입력 확인

    bool isDodge; // 회피 확인

    bool isCrouch; // 앉기 확인

    bool isDodgeorCrouch;

    Vector3 moveDir;
    Vector3 dodgeDir;

    Weapon equipWeapon;
    
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

        equipWeapon = weapon.GetComponent<Weapon>();
        equipWeapon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if(GameManager.instance.currentGameState == GameState.inGame)
        // {
            
        // }
        inputKey();
        
        if(!isAttack && !isAttackPose_Change) // 공격중이 아닐때 이동
        {
            Move();
        }
        if(isDodgeorCrouch)
        {
            StartCoroutine(Dodge());
        }
        if (isAttackBtnClick)    // 공격 코루틴
        {
            StartCoroutine(Attack_St());
        }
        if (isAttackPose)        // 공격종료 코루틴
        {
            Attack_ED_Check();
        }
        else
        {
            Crouch();
        }
        
        PrintAnimation();
    }

    IEnumerator Dodge()
    {
        if (!isDodge && isDodgeorCrouch && moveDir != Vector3.zero && stamina.MyCurrentValue >= 20f)
        {
            dodgeDir = moveDir;
            anim.SetTrigger("doDodge");
            isDodge = true;
            isCrouch = false;
            stamina.MyCurrentValue -= 20f;

            yield return new WaitForSeconds(0.75f);
            isDodge = false;
        }
    }

    IEnumerator Attack_St()
    {
        if (!isAttack && isAttackBtnClick && !isAttackPose)
        {
            isAttackPose = true;
            anim.SetTrigger("StartAttack");
            anim.SetFloat("drawSword", 1.0f);
            isAttackPose_Change = true;
            yield return new WaitForSeconds(0.5f);
            equipWeapon.gameObject.SetActive(true);
            unequip.SetActive(false);
            yield return new WaitForSeconds(1f);
            isAttackPose_Change = false;

        }
        else if (isAttackPose && isAttackBtnClick && !isAttack)
        {
            anim.SetTrigger("doAttack");
            isAttack = true;
            equipWeapon.use();
            yield return new WaitForSeconds(1.5f);
            isAttack = false;
        }
    }
    IEnumerator Attack_ED()
    {
        isAttackPose = false;
        anim.SetFloat("drawSword", -1.0f);
        isAttackPose_Change = true;
        yield return new WaitForSeconds(0.3f);
        equipWeapon.gameObject.SetActive(false);
        unequip.SetActive(true);
        isAttackPose_Change = false;
        
    }
    void Attack_ED_Check()
    {
        if (isAttackPose && isRun)
        {
            StartCoroutine(Attack_ED());
        }
    }

    
    private void St_Recovery()
    {
        if(stamina.MyCurrentValue >= 100)
        {
            RecoveryTime = 0f;
            return;
        }
        RecoveryTime += Time.deltaTime;
        if(RecoveryTime >= 2)
        {
            stamina.MyCurrentValue += 5f * Time.deltaTime;
        }
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
        isRunBtn = Input.GetButton("Run");
        isRun = isRunBtn && stamina.MyCurrentValue > 0;
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
            isRun = false;
            isCrouch = false;
            RecoveryTime = 0f;
        }
        else if(isRun && moveDir != Vector3.zero)
        {
            transform.Translate(moveDir * runSpeed * Time.deltaTime,Space.World);
            stamina.MyCurrentValue -= 10f * Time.deltaTime;
            isCrouch = false;
            RecoveryTime = 0f;
        }
        else if(isCrouch)
        {
            transform.Translate(moveDir * moveSpeed * 0.7f * Time.deltaTime,Space.World);
            St_Recovery();
        }
        else if(isWalk)
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime,Space.World);
            St_Recovery();
        }
        else
        {
            St_Recovery();
        }
    }
}
