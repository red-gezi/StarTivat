using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // ����������ƶ����ٶ�

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            // ��ȡWASD��������
            float horizontalInput = Input.GetAxis("Horizontal"); // �����ƶ�
            float verticalInput = Input.GetAxis("Vertical"); // ǰ���ƶ�

            // ������������µ�λ��
            Vector3 newPosition = transform.position + new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

            // �����������λ��
            transform.position = newPosition;
        }
    }
}