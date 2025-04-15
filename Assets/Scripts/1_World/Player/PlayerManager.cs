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
    //�����λ��
    public Transform cameraPos;
    public Transform focusPos;
    private Animator animator;
    public static PlayerManager Instance;
    private void Awake() => Instance = this;
    void Start() => animator = transform.GetChild(0).GetComponent<Animator>();
    void FixedUpdate()
    {
        ////////////////////////////////////////////////////////���������λ��////////////////////////////////////////////////////
        targetDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mouseX += Input.GetAxis("Mouse X") * mouseXSpeed * Time.fixedDeltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseYSpeed * Time.fixedDeltaTime;
        //������ֵ
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        currentDistance=Mathf.Lerp(currentDistance, targetDistance, Time.fixedDeltaTime);  
        mouseX = Mathf.Repeat(mouseX + 180f, 360f) - 180f;
        mouseY = Mathf.Clamp(mouseY, -60, 60);
        //��������
        float x = Mathf.Sin(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        float y = Mathf.Sin(mouseY * Mathf.Deg2Rad);
        float z = -Mathf.Cos(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        //��õ�ǰ��ҽ�ɫ�ĽǶ�
        cameraPos.position = new Vector3(x, y, z) * currentDistance + focusPos.position;
        cameraPos.LookAt(focusPos.position, transform.up);
        Camera.main.transform.position = cameraPos.transform.position;
        Camera.main.transform.eulerAngles = cameraPos.transform.eulerAngles;

        ////////////////////////////////////////////////////////���ƽ�ɫ����//////////////////////////////////////////////////////
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        //�����ڲ�ͬ������Ƕȡ���ͬ����Ƕ��������ĸ�����ĳ���
        Vector3 localRight = -Vector3.Cross(Camera.main.transform.forward, transform.up).normalized;
        Vector3 localForward = Vector3.Cross(localRight, transform.up).normalized;
        if (verticalInput != 0)
        {
            //�����������ʼλ��
            float angle = Vector3.SignedAngle(transform.forward, localForward * verticalInput, transform.up);
            //Debug.Log("�Ƕ�Ϊ" + angle);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        if (horizontalInput != 0)
        {
            //�����������ʼλ��
            float angle = Vector3.SignedAngle(transform.forward, localRight * horizontalInput, transform.up);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        ////////////////////////////////////////////////////////���ƽ�ɫ�ƶ�////////////////////////////////////////////////////////
        Vector3 moveVector = transform.GetChild(0).forward * Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput)) * moveSpeed * Time.fixedDeltaTime;
        //Debug.Log("�ƶ�����"+ moveVector);
        transform.position += (moveVector);
        //���ö���
        animator.SetBool("IsRun", verticalInput != 0 || horizontalInput != 0);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("����f");
            ScreenBreak.Instance.Show();
        }
    }
}