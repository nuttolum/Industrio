using UnityEngine;

public class FPSController : MonoBehaviour
{

    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = 0.1f;

    CharacterController controller;
    Camera cam;
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 smoothHoldV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    GameObject heldObj;
    GameObject heldPoint;
    bool lookingDown = false;
    bool jumping;
    float lastGroundedTime;
    bool disabled;

    void Start()
    {

        cam = Camera.main;
        heldPoint = new GameObject();
        heldPoint.transform.parent = cam.transform;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController>();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
    }
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && heldObj != null)
        {
            int layerMask = 1 << 3;
            layerMask = ~layerMask;
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f, layerMask))
            {
                heldPoint.transform.position = hit.point;
            }
            else
            {
                heldPoint.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5f;
            }

            Rigidbody rb = heldObj.GetComponent<Rigidbody>();
            Vector3 targetVel = ((heldPoint.transform.position - heldObj.transform.position) * 8f);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVel, ref smoothHoldV, smoothMoveTime);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = !disabled;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f))
            {
                if (hit.collider.gameObject.GetComponent<ProductionObject>() != null)
                {
                    heldObj = hit.collider.gameObject;
                    heldObj.transform.parent = Camera.main.transform;

                }
            }
        }
        if (Input.GetMouseButtonUp(0) && heldObj != null)
        {
            heldObj.transform.parent = null;
            heldObj = null;
        }


        if (disabled)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.Tab)) lookingDown = false;
        if (Input.GetKeyDown(KeyCode.E) && lookingDown == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10f))
            {
                print(hit.collider.gameObject.tag);
                if (hit.collider.gameObject.tag == "display" && hit.collider.gameObject.transform.parent.GetComponent<BaseScript>().state == "idle")
                {
                    lookingDown = true;
                    hit.collider.gameObject.transform.parent.GetComponent<BaseScript>().SwitchCam();
                }
            }
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        float currentSpeed = (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);

        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f))
            {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }

        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt(mX * mX + mY * mY);
        if (mMag > 5)
        {
            mX = 0;
            mY = 0;
        }

        yaw += mX * mouseSensitivity;
        pitch -= mY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;

    }


}
