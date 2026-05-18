using UnityEngine;

public class PlayerMovementCc : MonoBehaviour
{
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;

    [SerializeField] float mouseSpeed = 1.5f;
    [SerializeField] float gravity = -9.81f;

    float xRot;
    Vector3 velo;
    Transform camTr;

    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        camTr = Camera.main.transform;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        // 유니티 x축 회전은 마우스 반전처럼 느껴져 -= 사용
        xRot -= mouseY;
        // 회전축 최대 -90~90으로 제한, 한 바퀴 도는 걸 방지하기 위함.
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        // 좌우 회전은 플레이어 자체가 회전, 위아래는 카메라만 회전
        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        // 플레이어가 바닥에 닿아 있고, 아래로 떨어지는 속도가 있다면
        // 너무 빠르게 계속 떨어지지 않도록 작은 아래 방향 값으로 고정
        if (cc.isGrounded && velo.y < 0f) velo.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 movDir = transform.right * h + transform.forward * v;
        cc.Move(movDir * curSpeed * Time.deltaTime);

        // 매 프레임 중력을 적용해서 y 속도를 아래 방향으로 증가시킴
        velo.y += gravity * Time.deltaTime;
        cc.Move(velo * Time.deltaTime);
    }
}