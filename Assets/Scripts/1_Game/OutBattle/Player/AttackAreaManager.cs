using UnityEngine;

public class AttackAreaManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //角色攻击接触可作为焦点物体
        FocusManager focusManager = other.GetComponent<FocusManager>();
        if (focusManager != null)
        {
            Debug.Log("碰撞到焦点物体");
            focusManager.OnHit();
            Destroy(gameObject);
        }
        //角色被怪物攻击
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            Debug.Log("碰撞到角色");
            player.OnHit();
            Destroy(gameObject);
        }
    }
}