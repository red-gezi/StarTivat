using UnityEngine;
enum CharaIconType
{

}
public class CharaIconManager : MonoBehaviour
{
    private void Update()
    {
        transform.forward = -Camera.main.transform.forward;
    }
}
