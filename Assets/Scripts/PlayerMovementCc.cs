using UnityEngine;

public class PlayerMovementCc : MonoBehaviour
{
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;

    [SerializeField] float mouseSpeed = 1.5f;

    float xRot;
    Vector3 velo;
    Transform camTr;

    CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
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

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void Move()
    {
        if (cc.isGrounded && velo.y < 0f) velo.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 movDir = transform.right * h + transform.forward * v;
        cc.Move(movDir * curSpeed * Time.deltaTime);

        velo.y += -9.81f * Time.deltaTime;
        cc.Move(velo * Time.deltaTime);
    }
}
