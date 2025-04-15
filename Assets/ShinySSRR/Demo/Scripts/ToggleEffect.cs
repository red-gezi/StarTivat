using UnityEngine;

namespace ShinySSRR {
    public class ToggleEffect : MonoBehaviour {

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                ShinySSRR.isEnabled = !ShinySSRR.isEnabled;
            }
        }
    }

}