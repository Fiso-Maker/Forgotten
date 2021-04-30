using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{public Transform target;       // 따라갈 대상
    public float targetY;       // 땅을 못보게 하는 용도 (수가 커질수록 기본 시점이 높아짐, 그냥 0 쓰자)

    public float xRotMax;   // x축 회전 (상하 회전) 최댓값 (커질수록 캐릭터의 아래위치에서 볼 수 있음)
    public float xRotMin;   // x축 회전 (상하 회전) 최솟값 (작아질수록 캐릭터의 윗위치에서 볼 수 있음)
    public float rotSpeed;  // 회전 속도
    public float scrollSpeed;  // 스크롤 확대, 축소 속도

    public float distance;  // 기본 거리 설정 (카메라)
    public float minDistance;   // 최소 거리
    public float maxDistance;   // 최대 거리
 
    private float xRot; // x축 회전값
    private float yRot; // y축 회전값
    private Vector3 targetPos;  // 타겟 위치
    private Vector3 dir;    // 카메라가 있어야 하는 방향
 
    void Start()
    {
        // Cursor.lockState= CursorLockMode.Locked;
    }
    private void Update()
    {
        // 마우스 Y값 입력
        xRot += Input.GetAxis("Mouse Y") * rotSpeed * Time.deltaTime;  
        // 마우스 X값 입력 
        yRot += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;  
        // 마우스 스크롤로 거리 조절
        distance += -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime; 

        // 상하 시야 각도 뒤집히지 않게 Mathf.Clamp로 제한 걸기
        xRot = Mathf.Clamp(xRot, xRotMin, xRotMax);

        // 거리 너무 멀거나 너무 가까워지지 않게 Mathf.Clamp로 제한 걸기
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // 플레이어의 y 포지션이 0일때 카메라가 땅을 보지 못하게 하기
        targetPos = target.position + Vector3.up * targetY;

        // x축, y축 회전값에 맞게 방향조절
        // Quaternion.Euler == 집어넣은 값에 맞게 회전시킨 후 반환
        dir = Quaternion.Euler(-xRot, yRot, 0f) * Vector3.forward;

        // 타겟위치로부터 해당방향으로 정해진 거리만큼 멀어지기
        transform.position = targetPos +  dir * -distance;
        
        // 타겟바라보기
        transform.LookAt(targetPos);
    }
}
