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

    public bool isDodge; // 회피 확인

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

        try{
            health.Initialize(initHealth, initHealth);
            stamina.Initialize(initStamina,initStamina);
        }
        catch(NullReferenceException)
        {}
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
        if(GameManager.instance.currentGameState == GameState.inGame)
        {   
            HP_Check();
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
        }
        PrintAnimation();
    }

    IEnumerator Dodge()
    {
        // 스페이스 입력(조건 : 이동중, 스테이너가 20이상)
        if (!isDodge && isDodgeorCrouch && moveDir != Vector3.zero && stamina.MyCurrentValue >= 20f)
        {
            dodgeDir = moveDir; // 구르기 방향 고정
            anim.SetTrigger("doDodge"); // 구르기 애니메이션
            isDodge = true; // 무적상태
            isCrouch = false; // 웅크리기 상태일때 구르면 웅크리기 해제
            stamina.MyCurrentValue -= 20f; // 스테미너 소모

            yield return new WaitForSeconds(0.75f);
            isDodge = false; // 무적종료
        }
    }

    IEnumerator Attack_St()
    {
        if (!isAttack && isAttackBtnClick && !isAttackPose) // 납도 상태일때 클릭
        {
            isAttackPose = true;
            anim.SetTrigger("StartAttack"); // 무기 활용하는 애니메이션 시동
            anim.SetFloat("drawSword", 1.0f);   // 무기 발도
            isAttackPose_Change = true; // 발도 상태 체크
            yield return new WaitForSeconds(0.5f);

            equipWeapon.gameObject.SetActive(true); // 손에 적용한 무기 띄우기
            unequip.SetActive(false);   // 등에 매단 무기 지우기
            yield return new WaitForSeconds(1f);

            isAttackPose_Change = false;    // 무기 발도모션 중 이동불가

        }
        else if (isAttackPose && isAttackBtnClick && !isAttack) //발도 상태일때 클릭
        {
            anim.SetTrigger("doAttack");    // 공격하기
            isAttack = true;    // 공격중
            equipWeapon.use();  // 무기 타격 콜라이더 활성화
            yield return new WaitForSeconds(1.5f);
            isAttack = false;   // 공격종료
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
        // 스페이스 입력(조건 : 이동중이 아님)
        if(!isCrouch && isDodgeorCrouch && moveDir == Vector3.zero)
        {
            anim.SetTrigger("doCrouch"); // 애니메이션 시동
            isCrouch = true; // 웅크리기 시전 후 이속 감소
        }
        // 구르거나 공격했거나 shitf 키 입력 후 웅크리기 해제
        else if(isCrouch && isDodgeorCrouch && moveDir == Vector3.zero)
        {
            isCrouch = false; // 이속 감소 해제
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
        anim.SetBool("isRun",isRun);
        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isCrouch",isCrouch);
        anim.SetBool("isAttackPose",isAttackPose);
    }

    void HP_Check()
    {
        if(health.MyCurrentValue <=0)
        {
            GameManager.instance.GameOver_Lose();
        }
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
