using UnityEngine;

namespace ShinySSRR {

    public class Rotate : MonoBehaviour {

        public Vector3 axis = Vector3.up;
        public float speed = 60f;

        void Update() {
            transform.Rotate(axis * (Time.deltaTime * speed));

        }
    }

}