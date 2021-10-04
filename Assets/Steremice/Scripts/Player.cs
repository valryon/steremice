using DG.Tweening;
using UnityEngine;

namespace Steremice
{
  public class Player : MonoBehaviour
  {
    public float speed = 21;
    public float shootCooldown = 0.5f;
    public GameObject bulletPrefab;
    public Vector3 bulletOrigin = new Vector3(1, 0);

    private bool isPressed;
    private Vector2 previousPosition;
    private float cooldown;
    private Rigidbody2D rbody;

    private void Awake()
    {
      rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
      // Shoot
      if (isPressed)
      {
        if (cooldown > 0) cooldown -= Time.deltaTime;
        else
        {
          cooldown = shootCooldown;
          Shoot();
        }
      }

      // Movement
      // -- Mouse
      if (Input.GetMouseButton(0))
      {
        UpdateInputs(Input.mousePosition);
      }
      // -- Touch
      else if (Input.touchCount > 0)
      {
        UpdateInputs(Input.touches[0].position);
      }
      else
      {
        isPressed = false;
      }
    }

    private void UpdateInputs(Vector2 screenPosition)
    {
      Vector2 relativePosition = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
      if (isPressed == false)
      {
        isPressed = true;
      }
      else
      {
        Vector3 move = (relativePosition - previousPosition);
        Vector3 newPosition = (Vector2)(move * speed * Time.deltaTime) + rbody.position;

        if (SteremiceGame.Instance.IsOnScreen(newPosition))
        {
          rbody.position = newPosition;
        }
      }

      previousPosition = relativePosition;
    }

    private void Shoot()
    {
      var bullet = Instantiate(bulletPrefab, transform.position + bulletOrigin, Quaternion.identity);
      bullet.transform.SetParent(SteremiceGame.Root, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      var cheese = other.GetComponent<Cheese>();
      if (cheese)
      {
        SteremiceGame.Instance.Score += cheese.points;

        var go = cheese.gameObject;

        go.GetComponent<SpriteRenderer>().DOFade(0, 0.35f).SetEase(Ease.InCubic)
          .OnComplete(() =>
            {
              if (go) Destroy(go);
            }
          );
        Destroy(cheese);
      }
    }
  }
}