using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int score = 0;
    private List<PlayerAI> players = new List<PlayerAI>();

    public bool spawnZombies;
    public bool spawnLava;

    public GameObject zombiePrefab;
    public GameObject lavaPrefab;

    public Text scoreText;

    public float spawnDistanceLava = 4;
    public float unspawnDistanceLava = 4.5f;
    public float spawnDistanceZombies = 3;
    public float unspawnDistanceZombies = 3.5f;
    
	// Use this for initialization
	void Start ()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnZombies)
        {
            SpawnZombiesNearCamera();
            UnspawZombiesFarCamera();
        }

        if (spawnLava)
        {
            SpawnLavaNearCamera();
            UnspawnLavaFarCamera();
        }
    }

    private void SpawnNear(float distance, GameObject prefab, System.Func<int, int, bool> shouldSpawn)
    {
        var pos = Camera.main.transform.position;
        var xMin = (int)(pos.x - distance);
        var xMax = (int)(pos.x + distance + 1);
        var yMin = (int)(pos.y - distance);
        var yMax = (int)(pos.y + distance + 1);
        for (int x = xMin; x <= xMax; ++x)
        {
            for (int y = yMin; y <= yMax; ++y)
            {
                if (shouldSpawn(x, y))
                {
                    var spawned = Instantiate(prefab, new Vector3(x, y, prefab.transform.position.z), Quaternion.identity).GetComponent<ContactKill>();
                    spawned.spawnPosX = x;
                    spawned.spawnPosY = y;
                }
            }
        }
    }

    private void UnspawFar(float distance, IEnumerable<ContactKill> current)
    {
        var cameraPos = Camera.main.transform.position;
        var pos = new Vector2(cameraPos.x, cameraPos.y);
        foreach (var elem in current)
        {
            if (Vector2.Distance(pos, new Vector2(elem.transform.position.x, elem.transform.position.y)) > distance)
            {
                Destroy(elem.gameObject);
            }
        }
    }

    private void SpawnZombiesNearCamera()
    {
        SpawnNear(spawnDistanceZombies, zombiePrefab, ShouldSpawnZombie);
    }

    private void UnspawZombiesFarCamera()
    {
        UnspawFar(unspawnDistanceZombies, GameObject.FindGameObjectsWithTag("Zombie").Select(zombie => zombie.GetComponent<ContactKill>()));
    }

    private void SpawnLavaNearCamera()
    {
        SpawnNear(spawnDistanceLava, lavaPrefab, ShouldSpawnLava);
    }

    private void UnspawnLavaFarCamera()
    {
        UnspawFar(unspawnDistanceLava, GameObject.FindGameObjectsWithTag("Lava").Select(lava => lava.GetComponent<ContactKill>()));
    }

    private bool ShouldSpawnLava(int x, int y)
    {
        if (System.Math.Abs(x) % 4 != 1 || System.Math.Abs(y) % 4 != 1)
            return false;
        return !(GameObject.FindGameObjectsWithTag("Lava").Select(lava => lava.GetComponent<ContactKill>()).Any(lava => lava.spawnPosX == x && lava.spawnPosY == y));
    }

    private bool ShouldSpawnZombie(int x, int y)
    {
        if (System.Math.Abs(x) % 4 != 3 || System.Math.Abs(y) % 4 != 3)
            return false;
        return !(GameObject.FindGameObjectsWithTag("Zombie").Select(zombie => zombie.GetComponent<ContactKill>()).Any(zombie => zombie.spawnPosX == x && zombie.spawnPosY == y));
    }

    public void AddPlayer(PlayerAI player)
    {
        players.Add(player);
        player.onScoreChanged.AddListener(new UnityEngine.Events.UnityAction<int>(UpdateScore));
    }

    public void UpdateScore(int delta)
    {
        score += delta;
        scoreText.text = score.ToString();
    }


}
