using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseXSpeed = 100f;
    public float mouseYSpeed = 200f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    public float targetDistance = 2f;
    public float currentDistance = 2f;
    public float mouseX;
    public float mouseY;
    Vector3 gravity;
    //摄像机位置
    public Transform cameraPos;
    public Transform focusPos;
    private Animator animator;
    public static PlayerManager Instance;
    private void Awake() => Instance = this;
    void Start() => animator = transform.GetChild(0).GetComponent<Animator>();
    void FixedUpdate()
    {
        ////////////////////////////////////////////////////////控制摄像机位置////////////////////////////////////////////////////
        targetDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mouseX += Input.GetAxis("Mouse X") * mouseXSpeed * Time.fixedDeltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseYSpeed * Time.fixedDeltaTime;
        //限制数值
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        currentDistance=Mathf.Lerp(currentDistance, targetDistance, Time.fixedDeltaTime);  
        mouseX = Mathf.Repeat(mouseX + 180f, 360f) - 180f;
        mouseY = Mathf.Clamp(mouseY, -60, 60);
        //计算坐标
        float x = Mathf.Sin(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        float y = Mathf.Sin(mouseY * Mathf.Deg2Rad);
        float z = -Mathf.Cos(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        //获得当前玩家角色的角度
        cameraPos.position = new Vector3(x, y, z) * currentDistance + focusPos.position;
        cameraPos.LookAt(focusPos.position, transform.up);
        Camera.main.transform.position = cameraPos.transform.position;
        Camera.main.transform.eulerAngles = cameraPos.transform.eulerAngles;

        ////////////////////////////////////////////////////////控制角色朝向//////////////////////////////////////////////////////
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        //计算在不同摄像机角度、不同人物角度下人物四个方向的朝向
        Vector3 localRight = -Vector3.Cross(Camera.main.transform.forward, transform.up).normalized;
        Vector3 localForward = Vector3.Cross(localRight, transform.up).normalized;
        if (verticalInput != 0)
        {
            //控制摄像机初始位置
            float angle = Vector3.SignedAngle(transform.forward, localForward * verticalInput, transform.up);
            //Debug.Log("角度为" + angle);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        if (horizontalInput != 0)
        {
            //控制摄像机初始位置
            float angle = Vector3.SignedAngle(transform.forward, localRight * horizontalInput, transform.up);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        ////////////////////////////////////////////////////////控制角色移动////////////////////////////////////////////////////////
        Vector3 moveVector = transform.GetChild(0).forward * Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput)) * moveSpeed * Time.fixedDeltaTime;
        //Debug.Log("移动方向"+ moveVector);
        transform.position += (moveVector);
        //设置动画
        animator.SetBool("IsRun", verticalInput != 0 || horizontalInput != 0);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("按下f");
            ScreenBreak.Instance.Show();
        }
    }
}