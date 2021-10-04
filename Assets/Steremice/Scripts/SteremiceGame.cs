using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Steremice
{
  public class SteremiceGame : MonoBehaviour
  {
    public enum State
    {
      Title,
      Game
    }

    public static SteremiceGame Instance { get; private set; }
    public static Transform Root;

    public int Score { get; set; }

    public int BestScore
    {
      get => PlayerPrefs.GetInt("BestScore");
      set => PlayerPrefs.SetInt("BestScore", value);
    }

    [Header("Bindings")]
    public Camera gameCamera;

    public GameObject explosion;
    
    [Header("Data")]
    public Player playerPrefab;

    public Cheese cheesePrefab;
    public float minCheeseDelay = 2f;
    public float maxCheeseDelay = 5f;

    public GameObject[] rocksPrefabs;
    public GameObject[] enemiesPrefabs;


    [Header("UI")]
    public GameObject startPanel;
    public Text instructions;
    public Text title;
    public Text subtitle;
    public Text bestScore;

    public GameObject gamePanel;
    public Text lives;
    public Text score;

    private Rect cameraRect;
    public Rect Rect => cameraRect;
    
    private Player player;
    public Player Player => player;
    
    private Health playerHealth;
    private State state;
    private float cheeseCooldown;
    private float rocksCooldown;
    private float fightersCooldown;

    private void Start()
    {
      Instance = this;

      if (Application.targetFrameRate < 60) Application.targetFrameRate = 60;
      
      instructions.DOFade(0.25f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetDelay(0.5f);
      title.DOFade(0, 0);
      subtitle.DOFade(0, 0);
      title.DOFade(1f, 0.5f).SetEase(Ease.OutCubic).SetDelay(1f);
      subtitle.DOFade(1f, 0.5f).SetEase(Ease.OutCubic).SetDelay(1.42f);
      StartTitle();
    }

    private void OnDestroy()
    {
      instructions.DOKill();
    }

    private void Update()
    {
      switch (state)
      {
        case State.Title:
          UpdateTitleScreen();
          break;

        case State.Game:
          UpdateGame();
          break;
      }
    }

    private void UpdateTitleScreen()
    {
      if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
      {
        StartGame();
      }
    }

    private void StartGame()
    {
      state = State.Game;
      startPanel.gameObject.SetActive(false);
      gamePanel.gameObject.SetActive(true);

      if (Root != null)
      {
        Destroy(Root.gameObject);
      }

      Root = new GameObject("Steremice").transform;
      Root.SetParent(transform, false);

      // Create player
      player = Instantiate(playerPrefab.gameObject).GetComponent<Player>();
      player.transform.SetParent(Root, true);
      playerHealth = player.GetComponent<Health>();

      cheeseCooldown = maxCheeseDelay;
      rocksCooldown = 0.01f;
      fightersCooldown = 1f;
      
      UpdateCameraRect();
    }

    private void UpdateGame()
    {
      // Game Over?
      if (player == null)
      {
        if (Score > BestScore)
        {
          BestScore = Score;
        }

        StartTitle();
        return;
      }

      // Spawn cheeses
      if (cheeseCooldown > 0)
      {
        cheeseCooldown -= Time.deltaTime;
        if (cheeseCooldown <= 0)
        {
          cheeseCooldown = Random.Range(minCheeseDelay, maxCheeseDelay);
          var cheese = Instantiate(cheesePrefab.gameObject, Root, false);
          cheese.transform.position = new Vector3(Random.Range(cameraRect.min.x + 2, cameraRect.max.x - 2),
            Random.Range(cameraRect.min.y + 2, cameraRect.max.y - 2), 0);
        }
      }
      
      // Spawn rocks
      if (rocksCooldown > 0)
      {
        rocksCooldown -= Time.deltaTime;
        if (rocksCooldown <= 0)
        {
          rocksCooldown = Random.Range(0.5f, 4f);
          int rocksToSpawn = Random.Range(1, 5);
          for (int i = 0; i < rocksToSpawn; i++)
          {
            GameObject prefab = rocksPrefabs[Random.Range(0, rocksPrefabs.Length)];
            GameObject rock = Instantiate(prefab, Root, false);
            rock.transform.position = new Vector3(cameraRect.max.x + 2, cameraRect.center.y + Random.Range(-5, 5));

            AutoMove move = rock.GetComponent<AutoMove>();
            move.direction = new Vector2(-1f, Random.Range(-0.2f, 0.2f));
            move.speed *= Random.Range(0.75f, 2f);
          }
        }
        
        // Enemies
        if (fightersCooldown > 0)
        {
          fightersCooldown -= Time.deltaTime;
          if (fightersCooldown <= 0)
          {
            fightersCooldown = Random.Range(0.5f, 4f);
            int fightersToSpawn = Random.Range(1, 4);
            for (int i = 0; i < fightersToSpawn; i++)
            {
              GameObject prefab = enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)];
              GameObject enemy = Instantiate(prefab, Root, false);
              enemy.transform.position = new Vector3(cameraRect.max.x + 2, cameraRect.center.y + Random.Range(-5, 5));
            }
          }
        }
      }

      UpdateUI();
      UpdateCameraRect();
    }

    /// <summary>
    /// Entry point of the game. Reset to a normal state. Use this if you need to reinit the game.
    /// </summary>
    public void StartTitle()
    {
      state = State.Title;
      startPanel.gameObject.SetActive(true);
      gamePanel.gameObject.SetActive(false);

      bestScore.text = BestScore.ToString("000000000");

      if (Root != null)
      {
        Destroy(Root.gameObject);
      }
      
      if (player != null)
      {
        Destroy(player.gameObject);
      }
    }

    private void UpdateUI()
    {
      score.text = Score.ToString("000000000");
      lives.text = playerHealth != null ? playerHealth.hp.ToString() : "0";
    }

    private void UpdateCameraRect()
    {
      var dist = (Vector3.zero - gameCamera.transform.position).z;
      var leftBorder = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
      var rightBorder = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
      var topBorder = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
      var bottomBorder = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;

      cameraRect = new Rect(leftBorder, topBorder, rightBorder - leftBorder, bottomBorder - topBorder);
    }

    public bool IsOnScreen(Vector2 worldPosition)
    {
      return cameraRect.Contains(worldPosition);
    }

    public void PlayExplosion(Vector3 position)
    {
      var e = Instantiate(explosion, position, quaternion.identity);
      e.transform.SetParent(Root, true);
    }
  }
}