using MagicaCloth2;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public enum CameraMode
{
    Free,
    CameraTrack,
}
public enum AttackMode
{
    MeleeAttack,
    RangedAttack,
}
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
    public bool canMove = true;
    public bool canAttack = true;
    public bool isBusy = false;
    //摄像机位置
    public Transform cameraPos;
    public Transform focusPos;
    private Animator animator;
    public static PlayerManager Instance;
    public List<FocusManager> focusTargetList;
    public FocusManager focusTarget;
    public GameObject bullet;
    //所有角色的模型数据
    public GameObject CharaList;
    public Character currentChara => transform.GetChild(0)?.GetComponent<Character>();
    public AttackMode CurrentAttackMode { get; set; }
    private void Awake() => Instance = this;
    void FixedUpdate()
    {
        if (currentChara == null || animator == null)
        {
            return;
        }
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            animator.SetBool("IsRun", false);
            return;
        }


        ////////////////////////////////////////////////////////控制摄像机位置////////////////////////////////////////////////////
        targetDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mouseX += Input.GetAxis("Mouse X") * mouseXSpeed * Time.fixedDeltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseYSpeed * Time.fixedDeltaTime;
        //限制数值
        mouseX = Mathf.Repeat(mouseX + 180f, 360f) - 180f;
        mouseY = Mathf.Clamp(mouseY, -60, 60);
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        float finalDistance = targetDistance * Mathf.Lerp(1, 0.5f, Mathf.Abs(mouseY) / 60f);
        finalDistance = Mathf.Clamp(finalDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, finalDistance, Time.fixedDeltaTime * 2);

        //计算坐标
        float x = Mathf.Sin(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        float y = Mathf.Sin(mouseY * Mathf.Deg2Rad);
        float z = -Mathf.Cos(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        //获得当前玩家角色的角度
        cameraPos.position = new Vector3(x, y, z) * currentDistance + focusPos.position;
        cameraPos.LookAt(focusPos.position, transform.up);

        //////////////////////////////////////////////////////控制角色朝向//////////////////////////////////////////////////////
        if (!isBusy)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            // 获取摄像机的前向和右向向量（在世界坐标系中）
            Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;

            // 根据摄像机的前向和右向向量来计算输入方向
            Vector3 inputDirection = (cameraRight * horizontalInput + cameraForward * verticalInput);

            if (inputDirection != Vector3.zero)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    Debug.Log("中断攻击状态");
                    animator.SetTrigger("IsInterrupt");
                }
                inputDirection.Normalize();  // 归一化输入方向

                // 根据输入方向计算目标朝向
                float angle = Vector3.SignedAngle(transform.forward, inputDirection, transform.up);
                transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);

                // 根据输入方向计算移动向量
                Vector3 moveVector = inputDirection * Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput)) * moveSpeed * Time.fixedDeltaTime;
                //transform.position += (moveVector);
                GetComponent<Rigidbody>().MovePosition(transform.position + moveVector);
            }

            animator.SetBool("IsRun", inputDirection != Vector3.zero);
        }

        //设置动画
        //animator.SetBool("IsRun", verticalInput != 0 || horizontalInput != 0);


    }
    private async void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            await ScreenWarpManager.ShowScreen();
            await Task.Delay(1000);
            await ScreenWarpManager.CloseScreen();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameFlowSystem.Instance.SwitchInBattleMode(InBattleSystem.Instance.players, new OutOfBattleEnemyDatas()
            {
                enemyDatas = new List<EnemyData>()
                {
                    new EnemyData(){CurrentEnemyName= EnemyName.Qiuqiu,},
                    new EnemyData(){CurrentEnemyName= EnemyName.Qiuqiu,},
                    new EnemyData(){CurrentEnemyName= EnemyName.Qiuqiu,},
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            var roomData = new RoomConfigData()
            {
                CurrentRoomType = RoomType.BattleRoom,
                SelectableSceneModel = new() { SceneModelType.椛染之庭, SceneModelType.西风教堂, SceneModelType.西风骑士团 },
                DoorCount = new() { 2, 3 },
            }
            .SetEnemyTag(RoomTag.EnemyCount3);
            await RoomSystem.EnterRoom(roomData);
        }
    }
    public async void OnMouseClickCanve()
    {
        if (!isBusy)
        {
            isBusy = true;
            Debug.Log("进入繁忙状态");
            currentChara.transform.localPosition = Vector3.zero;
            animator.SetBool("IsRun", false);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                Debug.Log("中断攻击状态");
                animator.SetTrigger("IsInterrupt");
            }
            //朝向最近的单位
            if (focusTarget != null)
            {
                Vector3 rawDirection = focusTarget.transform.position - transform.position;
                // 清除Y轴分量
                Vector3 horizontalDirection = new Vector3(rawDirection.x, 0, rawDirection.z);
                float angle = Vector3.SignedAngle(transform.forward, horizontalDirection, transform.up);
                transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
            }
            await Task.Delay(200);
            //攻击
            switch (CurrentAttackMode)
            {
                case AttackMode.MeleeAttack: await MeleeAttackAsync(); break;
                case AttackMode.RangedAttack: await RangedAttackAsync(); break;
            }
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                isBusy = false;
                Debug.Log("解除繁忙状态");
            });
        }
    }
    private async void Update()
    {


        //if (Input.GetMouseButtonDown(1))
        //{
        //    //加速
        //    animator.SetTrigger("IsAttack2");
        //}
        //设置玩家注视敌人
        FocusManager currentFocusEnemy = focusTargetList
               .Where(enemy => enemy.focusWeight > 0)
               .OrderByDescending(enemy => enemy.focusWeight)
               .FirstOrDefault();
        if (focusTarget != currentFocusEnemy)
        {
            focusTarget?.CloseFocusIcon();
            focusTarget = currentFocusEnemy;
            focusTarget?.ShowFocusIcon();

        }
    }
    [Button("切换人物")]
    public void SwitchChara(PlayerName charaName)
    {
        // 查找子物体（直接子物体，不递归）
        if (currentChara != null && currentChara.name == charaName.ToString())
        {
            Debug.LogWarning("同人物无法切换");
            return;
        }
        currentChara?.gameObject.SetActive(false);
        Transform targetChara = transform.Find(charaName.ToString());
        if (targetChara == null)
        {
            //拷贝一个
            var originalChara = CharaList.transform.Find(charaName.ToString());
            Debug.Log(originalChara);
            targetChara = Instantiate(originalChara, transform);
            targetChara.name = originalChara.name;
        }
        if (targetChara != null)
        {
            // 将子物体移动到首位
            targetChara.SetAsFirstSibling();
            targetChara.gameObject.SetActive(true);
            Debug.Log($"已将 {charaName} 移动到首位");
            //重定位控制目标
            animator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
            //播放个特效
        }
        else
        {
            Debug.LogError($"未找到名称为 {charaName} 的子物体");
        }
    }
    public async Task MeleeAttackAsync()
    {
        Debug.Log("进入攻击");
        animator.SetTrigger("IsAttack");
        await Task.Delay(400);
        var newBullet = Instantiate(bullet);
        newBullet.SetActive(true);
        newBullet.transform.position = bullet.transform.position;
        newBullet.GetComponent<Rigidbody>().AddForce(transform.GetChild(0).forward * 10, ForceMode.Impulse);

    }
    public async Task RangedAttackAsync()
    {
        Debug.Log("进入攻击");
        animator.SetTrigger("IsAttack");
        await Task.Delay(400);
        var newBullet = Instantiate(bullet);
        newBullet.SetActive(true);
        newBullet.transform.position = bullet.transform.position;
        newBullet.GetComponent<Rigidbody>().AddForce(transform.GetChild(0).forward * 10, ForceMode.Impulse);
    }

    internal void OnHit()
    {
        Debug.LogError("我被偷袭啦!");
    }
    public void ResetCameraView()
    {
        mouseX = 0;
        mouseY = 0;
    }
}
