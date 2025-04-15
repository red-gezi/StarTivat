using System;
using UnityEngine;

namespace ShinySSRR {

    [ExecuteAlways]
    public class ExcludeReflections : MonoBehaviour {

        [NonSerialized]
        public Renderer[] renderers;

        private void OnEnable() {
            Refresh();
            ShinySSRR.RegisterExcludeReflections(this);
        }


        private void OnDisable() {
            ShinySSRR.UnregisterExcludeReflections(this);
        }

        public void Refresh() {
            renderers = GetComponentsInChildren<Renderer>();
        }

    }

}