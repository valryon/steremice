using DG.Tweening;
using UnityEngine;

namespace Steremice
{
  public class Cheese : MonoBehaviour
  {
    public int points;
    public float ttl = 3;

    private void Awake()
    {
      Destroy(gameObject, ttl);
      transform.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
      transform.DOPunchRotation(new Vector3(0, 0, 360f), 1f);
      transform.DOScale(Vector3.one, 1f);
    }

    private void OnDestroy()
    {
      transform.DOKill();
    }
  }
}