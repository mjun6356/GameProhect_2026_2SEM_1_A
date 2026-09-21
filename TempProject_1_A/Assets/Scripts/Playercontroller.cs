using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerState
{
    Nomal,
    Pickup,

}

public class Playercontroller : MonoBehaviour
{
    [SerializeField]private Animator animator; //애니메이터

    [SerializeField]private Transform cameraTransform;

    [SerializeField] private float walkSpeed = 3f;

    [SerializeField] private float runSpeed= 6f;

    [SerializeField] private float rotationSpeed = 10f;

    [Header("바닥 설정")]
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;

    private float verticalVelocity;

    private PlayerState currentState = PlayerState.Nomal;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Keyboard keyboard = Keyboard.current;

        if(keyboard == null)
        {
            return;
        }

        ApplyGravity();

        if (currentState != PlayerState.Nomal) return;

        HandleMovement(keyboard);

      
    }




    private void ApplyGravity()
    {
        // 기본 중력 설정
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);  // 컨트롤러에 이동 방향과 속도를 준다.



    }
    private void HandleMovement(Keyboard keyboard)
    {
        Vector2 input = Vector2.zero;

        if (keyboard.aKey.isPressed)
            input.x -= 1f;
        if (keyboard.dKey.isPressed)
            input.x += 1f;
        if (keyboard.sKey.isPressed)
            input.y -= 1f;
        if (keyboard.wKey.isPressed)
            input.y += 1f;

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * input.y + cameraRight * input.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);


        //4. shift 달리기
        bool isRunnig = keyboard.leftShiftKey.isPressed;
        float currentSpeed = isRunnig ? runSpeed : walkSpeed;

        //5. 수평이동

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 기본 중력 설정
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);  // 컨트롤러에 이동 방향과 속도를 준다.

        float animtionSpeed = 0f;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            animtionSpeed = isRunnig ? 1f : 0.5f;
        }

        animator.SetFloat("speed", animtionSpeed, 0.1f, Time.deltaTime);
    }

    public void ChangeState(PlayerState newstate)
    {
        currentState = newstate;

        if (currentState!= PlayerState.Nomal)
        {
            animator.SetFloat("speed", 0);
        }


        Debug.Log("현재 상태 : " +  currentState);

    }

    

}
