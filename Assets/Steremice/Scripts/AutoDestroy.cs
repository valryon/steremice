using UnityEngine;

namespace Steremice
{
    public class AutoDestroy : MonoBehaviour
    {
        public float ttl = 1;

        private void Start()
        {
            Destroy(gameObject, ttl);
        }
    }
}