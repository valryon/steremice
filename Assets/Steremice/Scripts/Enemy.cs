using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Steremice
{
  [Serializable]
  public struct PathPoint
  {
    public Vector3 point;
    public float speedFactor;
  }

  public class Enemy : MonoBehaviour
  {
    public float speed = 0.15f;
    public GameObject bulletPrefab;
    public float shootCooldown = 1f;
    public int behaviour = 0;
    public List<PathPoint> path = new List<PathPoint>();

    private Rigidbody2D rbody;
    private Vector3 pathOrigin;
    private float cooldown;
    private int index;

    private void Start()
    {
      rbody = GetComponent<Rigidbody2D>();
      cooldown = shootCooldown;
      GeneratePath();
    }

    private void GeneratePath()
    {
      pathOrigin = transform.position;

      int slices = Random.Range(1, 3);
      for (int i = 0; i < slices; i++)
      {
        PathPoint point = new PathPoint();
        point.point = new Vector3(Random.Range(SteremiceGame.Instance.Rect.min.x, SteremiceGame.Instance.Rect.max.x),
          Random.Range(SteremiceGame.Instance.Rect.min.y, SteremiceGame.Instance.Rect.max.y));
        point.speedFactor = Random.Range(0.8f, 1.2f);
        path.Add(point);
      }

      PathPoint final = new PathPoint();
      final.point = new Vector3(-10,
        Random.Range(SteremiceGame.Instance.Rect.min.y, SteremiceGame.Instance.Rect.max.y));
      path.Add(final);

      index = 0;
    }

    private void Update()
    {
      // Move
      var nextPoint = path[index];
      if (Vector3.Distance(nextPoint.point, transform.position) > 0.15f)
      {
        Vector3 dir = nextPoint.point - transform.position;
        rbody.position += (Vector2)(dir * speed * nextPoint.speedFactor * Time.deltaTime);
      }
      else
      {
        index++;
        if (index >= path.Count)
        {
          Destroy(gameObject);
        }
      }

      // and Shoot. Classic.
      cooldown -= Time.deltaTime;
      if (cooldown <= 0)
      {
        cooldown = shootCooldown;
        StartCoroutine(Shoot());
      }
    }

    private IEnumerator Shoot()
    {
      void CreateBullet(Vector3 dir)
      {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.SetParent(SteremiceGame.Root, true);
        var move = bullet.GetComponent<AutoMove>();

        move.direction = dir;
      }

      if (behaviour == 0)
      {
        if (SteremiceGame.Instance.Player != null)
        {
          CreateBullet(Vector3.Normalize(SteremiceGame.Instance.Player.transform.position - transform.position));
        }
      }
      else if (behaviour == 1)
      {
        CreateBullet(new Vector2(0.5f, 0.5f));
        CreateBullet(new Vector2(0.5f, -0.5f));
        CreateBullet(new Vector2(-0.5f, 0.5f));
        CreateBullet(new Vector2(-0.5f, -0.5f));
        
        yield return new WaitForSeconds(0.15f);
        
        CreateBullet(new Vector2(0.5f, 0.5f));
        CreateBullet(new Vector2(0.5f, -0.5f));
        CreateBullet(new Vector2(-0.5f, 0.5f));
        CreateBullet(new Vector2(-0.5f, -0.5f));
      }
      else
      {
        CreateBullet(new Vector3(-1, 0));
        yield return new WaitForSeconds(0.25f);
        CreateBullet(new Vector3(-1, 0));
        yield return new WaitForSeconds(0.25f);
        CreateBullet(new Vector3(-1, 0));
      }
    }

    private void OnDrawGizmosSelected()
    {
      Vector3 position = pathOrigin;
      Vector3 prev = position;
      foreach (var p in path)
      {
        for (float t = 0; t < 1f; t += 0.1f)
        {
          Vector3 b = Vector3.Lerp(position, p.point, t);

          Gizmos.DrawSphere(b, 0.25f);
          Gizmos.DrawLine(b, prev);
          prev = b;
        }

        position = p.point;
      }
    }
  }
}