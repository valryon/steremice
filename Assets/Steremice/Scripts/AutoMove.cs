using UnityEngine;

namespace Steremice
{
    public class AutoMove : MonoBehaviour
    {
        public float speed = 1;

        public Vector2 direction = new Vector2(-1,0);

        private Rigidbody2D rbody;

        private void Awake()
        {
          rbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
          if (rbody != null)
          {
            rbody.position += direction * speed * Time.deltaTime;
          }
          else
          {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
          }
        }
    }
}