using DG.Tweening;
using UnityEngine;

namespace Steremice
{
    public class Health : MonoBehaviour
    {
        public int hp;
        public int points;

        private SpriteRenderer sprite;

        private void Awake()
        {
          sprite = GetComponent<SpriteRenderer>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(gameObject.tag))
            {
                CheckDamages(other.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(gameObject.tag))
            {
                CheckDamages(other.gameObject);
            }
        }

        private void CheckDamages(GameObject other)
        {
            var dmg = other.GetComponent<Damage>();
            if (dmg == null) return;

            hp -= dmg.dmg;
            
            if(dmg.destroyOnHit) Destroy(other.gameObject);
            if (hp <= 0)
            {
              if (points > 0)
              {
                SteremiceGame.Instance.Score += points;
              }
              sprite.DOKill();

              SteremiceGame.Instance.PlayExplosion(transform.position);
              
              Destroy(gameObject);
            }
            else
            {
              if (sprite != null)
              {
                sprite.DOKill();
                sprite.DOColor(Color.red, 0.05f)
                  .OnComplete(() =>
                  {
                      if(sprite) sprite.DOColor(Color.white, 0.025f);
                  });
              }
            }
            
        }
    }
}