using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // 控制摄像机移动的速度

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            // 获取WASD键的输入
            float horizontalInput = Input.GetAxis("Horizontal"); // 左右移动
            float verticalInput = Input.GetAxis("Vertical"); // 前后移动

            // 根据输入计算新的位置
            Vector3 newPosition = transform.position + new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

            // 设置摄像机的位置
            transform.position = newPosition;
        }
    }
}