using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGroup
{
    public Color color;
    public List<GameObject> group;
    public PlayerController.Fraction fraction;
    public PlayerController.Team groupTeam;

    public PlayerGroup(List<GameObject> group, Color color, PlayerController.Fraction fraction = PlayerController.Fraction.None, PlayerController.Team groupTeam = PlayerController.Team.None)
    {
        this.group = group;
        this.color = color;
        this.fraction = fraction;
        this.groupTeam = groupTeam;
    }

    public void ResetGroup()
    {
        //AttackEnemu();
        foreach (var player in group)
        {
            var playerController = player.GetComponent<PlayerController>();
            playerController.DoShot();
        }
    }

    public void AttackEnemu()
    {
        var enemyTeam = groupTeam == PlayerController.Team.Player1 ? PlayerController.Team.Player2 : PlayerController.Team.Player1;
        var colums = group
            .Select(item => item.GetComponent<PlayerController>().mapPosition.x)
            .Distinct()
            .ToList();
        
        foreach(var currentX in colums)
        {
            var players = GameObject.FindGameObjectsWithTag("Player")
                .Where(item => item.GetComponent<PlayerController>().team == enemyTeam)
                .Where(item => item.GetComponent<PlayerController>().mapPosition.x == Mathf.Abs(6 - currentX))
                .OrderBy(item => item.GetComponent<PlayerController>().mapPosition.x)
                .ThenBy(item => item.GetComponent<PlayerController>().mapPosition.y)
                .ToList();

            var currentXGroup = group
                .Where(item => item.GetComponent<PlayerController>().mapPosition.x == currentX)
                .OrderBy(item => item.GetComponent<PlayerController>().mapPosition.y)
                .ToList();
            var maxItemCount = Mathf.Max(players.Count, currentXGroup.Count);
            for(var i = 0; i < maxItemCount; i++)
            {
                if (currentXGroup.Count == 0 || players.Count == 0) break;
                var currentPlayerObject = currentXGroup.Last();
                var currentPlayer = currentPlayerObject.GetComponent<PlayerController>();
                var currentEnemyObject = players.Last();
                var currentEnemy = currentEnemyObject.GetComponent<PlayerController>();
                if (currentPlayer.CanKill(currentEnemy.fraction))
                {
                    players.Remove(currentEnemyObject);
                    currentEnemy.Reset();
                }
                else
                {
                    currentXGroup.Remove(currentPlayerObject);
                }
            }


            foreach (var currentGroupItem in currentXGroup)
            {
                var currentPlayer = currentGroupItem.GetComponent<PlayerController>();
                foreach(var currentEnemyObj in players)
                {
                    var currentEnemyObject = players.Last();
                    var currentEnemy = currentEnemyObject.GetComponent<PlayerController>();
                    
                }
            }
        }
    }

}

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Players
    {
        Player1,
        Player2,
    };
    public enum CameraPositionNames
    {
        Player1,
        Player2,
        Default,
    };
    public Dictionary<CameraPositionNames, Quaternion> CameraPositions = new Dictionary<CameraPositionNames, Quaternion>()
    {
        {CameraPositionNames.Default, new Quaternion(0,90,0,1f)},
        {CameraPositionNames.Player1, new Quaternion(0,0,0,1f)},
        {CameraPositionNames.Player2, new Quaternion(0,180,0,1f)}


    };
    public PlayerController selectedPlayer;
    public List<PlayerGroup> playerGroups = new List<PlayerGroup>();
    public GameObject GroupList;
    public GameObject CameraObject;
    public GameObject SquadTab; 
    private List<GameObject> Models;
    public List<AudioClip> musicAudioClips = new List<AudioClip>();
    public Players CurrentPlayer;

    void Start()
    {
        var cavalryman = Resources.Load("cavalryman") as GameObject;
        var halberdiers = Resources.Load("halberdiers") as GameObject;
        var knight = Resources.Load("knight") as GameObject;
        var maceman = Resources.Load("maceman") as GameObject;
        var spearman = Resources.Load("spearman") as GameObject;
        Models = new List<GameObject>() { cavalryman, halberdiers, knight, maceman, spearman };
        var a = GameObject.FindGameObjectsWithTag("Place");
        foreach (var b in a)
        {
            b.GetComponent<PlaceController>().RenderModel(Models);
        }
        StartCoroutine(RotateCamera(CameraPositions[(CameraPositionNames)CurrentPlayer]));
        StartCoroutine(PlayBackgroudMusic());
        //ScoreController.TargetScoreWidthP1 = 100;
        //ScoreController.TargetScoreWidthP2 = 890;
        //Debug.Log(Score.transform.GetChild(0).GetComponent<RectTransform>().rect.width);
    }
    IEnumerator PlayBackgroudMusic()
    {
        // Текущий индекс трека
        int musicIndex = 0;
        // Проигрываем музыку, если она есть
        while (musicAudioClips.Count > 0)
        {
            // Время для запуска следующего трека + задержка
            float waitTime = musicAudioClips[musicIndex].length + 2;
            // Проигрываем мелодию один раз
            transform.GetComponent<AudioSource>().PlayOneShot(musicAudioClips[musicIndex]);

            // Работа с текущим индексом трека
            musicIndex++;
            if (musicIndex >= musicAudioClips.Count)
            {
                musicIndex = 0;
            }

            // Задержка для включения следующего трека
            yield return new WaitForSeconds(waitTime);
        }
    }

   

    public void SquadButton()
    {
        Debug.Log(SquadTab.active);
        SquadTab.SetActive(!SquadTab.active);
    }


    private int GetCameraDelta(int currentValue) => currentValue < 0 ? 1 : currentValue > 0 ? -1 : 0;

    IEnumerator RotateCamera(Quaternion newRotation, float delay=1f)
    {
        yield return new WaitForSeconds(delay);
        var currentRotation = CameraObject.transform.rotation.eulerAngles;
        var offsetX = (int)(currentRotation.x - newRotation.x);
        var offsetY = (int)(currentRotation.y - newRotation.y);
        var offsetZ = (int)(currentRotation.z - newRotation.z);
        while (offsetY != 0 || offsetX != 0 || offsetZ != 0)
        {
            var dx = GetCameraDelta(offsetX);
            var dy = GetCameraDelta(offsetY);
            var dz = GetCameraDelta(offsetZ);

            CameraObject.transform.Rotate(new Vector3(dx, dy, dz));
            currentRotation = CameraObject.transform.rotation.eulerAngles;
            offsetX = (int)(currentRotation.x - newRotation.x);
            offsetY = (int)(currentRotation.y - newRotation.y);
            offsetZ = (int)(currentRotation.z - newRotation.z);
            yield return new WaitForSeconds(0.01f);
        }
        FindMatches();
    }

    private List<PlayerGroup> checkByPlayerGroups(List<PlayerGroup> existGroups, PlayerController player)
    {
        var a = existGroups
            .Where(item => item.group.Any(x =>
            {
                var mp1 = x.GetComponent<PlayerController>().mapPosition;
                var mp2 = player.mapPosition;
                if (mp1.Equals(mp2))
                {
                    Debug.Log("mp1: " + mp1.ToString());
                    Debug.Log("mp2: " + mp2.ToString());
                }

                return mp1.Equals(mp2);
            }))
            .ToList();
        Debug.Log(a.Count);
        return new List<PlayerGroup>();


    }

    private Color GetGroupColor(PlayerGroup targetGroup)
    {
        return new Color(Random.value, Random.value, Random.value);
        //var a = playerGroups
        //    .Where(playerGroup => playerGroup.group
        //        .Where(playerObj => targetGroup.group
        //            .Where(item => playerObj.GetComponent<PlayerController>().mapPosition.Equals(item.GetComponent<PlayerController>().mapPosition))
        //            .ToList()
        //            .Count > 0)
        //        .ToList()
        //        .Count > 0)
        //    .ToList();
        //Debug.Log("Grops count >> " + a.Count);
        //if (a.Count > 0) return a[0].color;
        //return new Color(Random.value, Random.value, Random.value);
    }
    
    private PlayerGroup FindGroup(GameObject currentPlayer, List<GameObject> players, List<MapPosition> passedCells, PlayerGroup result)
    {
        var currentPlayerController = currentPlayer.GetComponent<PlayerController>();
        var neighbors = players
            .Where(item => item.GetComponent<PlayerController>().fraction == currentPlayerController.fraction 
                && !result.group.Contains(item) && !passedCells.Contains(item.GetComponent<PlayerController>().mapPosition))
            .Where(item =>
            {
                var neighborPosition = item.GetComponent<PlayerController>().mapPosition;
                var currentPosition = currentPlayerController.mapPosition;
                var offsetX = Mathf.Abs(neighborPosition.x - currentPosition.x);
                var offsetY = Mathf.Abs(neighborPosition.y - currentPosition.y);
                //Debug.Log($@"offsetX {offsetX}, offsetY {offsetY}");
                if (offsetX > 1 || offsetY > 1) return false;
                return (offsetX == 1 && offsetY == 0) || (offsetY == 1 && offsetX == 0);
            })
            .OrderBy(item => item.GetComponent<PlayerController>().mapPosition.x)
            .ThenBy(item => item.GetComponent<PlayerController>().mapPosition.y)
            .ToList();
        result.group.AddRange(neighbors);
        if (result.group.Count >= 5)
        {
            result.group = result.group.Take(5).ToList();
            return result;
        }
        foreach (var neighbor in neighbors)
        {
            result = FindGroup(neighbor, players, passedCells, result);
        }
        return result;

    }

    private int findInExistGroup(PlayerController currentPlayerController)
    {
        var res = -1;
        var index = 0;
        var mapPosition1 = currentPlayerController.mapPosition;
        //Debug.Log(@$"existGroups " + playerGroups.Count);
        foreach (var group in playerGroups)
        {
            var list = group.group;
            foreach(var item in list)
            {
                var mapPosition2 = item.GetComponent<PlayerController>().mapPosition;
                if (mapPosition1.Equals(mapPosition2))
                {
                    Debug.Log(mapPosition1.ToString());
                    Debug.Log(mapPosition2.ToString());
                    res = index;
                    break;
                }
            }
            if (res >= 0) break;
            index++;
        }
        return res;
    }

    public void FindMatches()
    {
        //FillEmptyPlaces(PlayerController.Team.Player1);
        //FillEmptyPlaces(PlayerController.Team.Player2);
        var allPlayers = GameObject.FindGameObjectsWithTag("Player");
        var currentTeam = (PlayerController.Team)CurrentPlayer;
        var enemyPlayers = allPlayers
            .Where(item => item.GetComponent<PlayerController>().team != currentTeam);
        foreach(var enemy in enemyPlayers)
            enemy.GetComponent<PlayerController>().SwithPlaceColor(Color.white);
        var newGroups = new List<PlayerGroup>();
        var players = allPlayers
            .Where(item => item.GetComponent<PlayerController>().team == currentTeam)
            .OrderBy(item => item.GetComponent<PlayerController>().mapPosition.x)
            .ThenBy(item => item.GetComponent<PlayerController>().mapPosition.y)
            .ToList();
        //Debug.Log(players.Count);
        var passedCells = new List<MapPosition>();
        foreach (var player in players)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (passedCells.Contains(playerController.mapPosition)) continue;
            var group = FindGroup(player, players, passedCells, new PlayerGroup(new List<GameObject> { player }, Color.white, playerController.fraction));
            var isValidGroup = group.group.Count >= 3;
            if (isValidGroup)
            {
                //Debug.Log($@"Группа {group.group.Count}, фракция {group.fraction
                group.color = GetGroupColor(group);
                group.groupTeam = currentTeam;
                newGroups.Add(group);
            }
            foreach (var mem in group.group)
            {
                var b = mem.GetComponent<PlayerController>();
                passedCells.Add(b.mapPosition);
                if(!b.placeController.isEmpty)
                    mem.GetComponent<PlayerController>().SwithPlaceColor(group.color);
            }
        }
        playerGroups = newGroups;
        StartCoroutine(UpdateList());
    }

    public void SwichMove(float delay=1f)
    {
        CurrentPlayer = CurrentPlayer == Players.Player1 ? Players.Player2 : Players.Player1;
        StartCoroutine(RotateCamera(CameraPositions[(CameraPositionNames)CurrentPlayer], delay));
    }

    IEnumerator UpdateList()
    {
        yield return null;
        //Debug.Log("123");
        var view = Resources.Load("PlayerGroup") as GameObject;
        for (int i = 0; i < GroupList.transform.childCount; i++)
            Destroy(GroupList.transform.GetChild(i).gameObject);
        var index = 0;
        foreach (var group in playerGroups)
        {
            var obj = Instantiate(view, GroupList.transform.position - new Vector3(0, 30 + 65 * index, 0), Quaternion.identity);
            //obj.transform.GetComponent<Image>().color = group.color;
            obj.transform.Find("Name").GetComponent<Text>().color = group.color;
            obj.transform.Find("Name").GetComponent<Text>().text = $@"Группа ${group.fraction} длинны {group.group.Count}";
            obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                //Debug.Log("attack");
                for (int i = 0; i < GroupList.transform.childCount; i++)
                    Destroy(GroupList.transform.GetChild(i).gameObject);
                UpdateList();
                group.ResetGroup();
                SwichMove(3f);
            });
            index++;
            obj.transform.SetParent(GroupList.transform);
        };
    }

    public void RestartGame()
    {
        var places = GameObject.FindGameObjectsWithTag("Place");
        //Убрать
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            Destroy(player);
        }
        foreach (var place in places)
        {
            var placeController = place.GetComponent<PlaceController>();
            placeController.RenderModel(Models);
        }
    }
    public void DoSwith(PlayerController player)
    {
        if (!selectedPlayer)
        {
            selectedPlayer = player;
        }
        else
        {
            if (!selectedPlayer.mapPosition.canSwith(player.mapPosition))
            {
                //selectedPlayer.SwithPlaceColor(Color.green);
                //player.SwithPlaceColor(Color.green);
                selectedPlayer = null;
                return;
            }
            var pos1 = selectedPlayer.GetComponent<Transform>().position;
            var pos2 = player.GetComponent<Transform>().position;
            var mapPos1 = selectedPlayer.mapPosition;
            var mapPos2 = player.mapPosition;
            var placeControl1 = selectedPlayer.placeController;
            var placeControl2 = player.placeController;
            selectedPlayer.placeController = placeControl2;
            player.placeController = placeControl1;
            player.moveTo(pos1);
            selectedPlayer.moveTo(pos2);
            player.mapPosition = mapPos1;
            selectedPlayer.mapPosition = mapPos2;
            //Debug.Log(pos1);
            //Debug.Log(pos2);
            //selectedPlayer.SwithPlaceColor(Color.green);
            //player.SwithPlaceColor(Color.green);
            selectedPlayer = null;

        }
    }

    public void FillEmptyPlaces(PlayerController.Team team)
    {
        //Продолжить тут
        //Dictionary<int, List<GameObject>> places;
        Debug.Log(team);
        var allPlaces = GameObject.FindGameObjectsWithTag("Place")
            .Where(item => item.GetComponent<PlaceController>().PlaceTeam == team)
            .ToList();
        var xToEmptyPlaces = allPlaces
            .Where(item => item.GetComponent<PlaceController>().isEmpty)
            .GroupBy(item => item.GetComponent<PlaceController>().PlaceX)
            .ToDictionary(item => item.Key, item => item.ToList());

        foreach (var emptyPlaces in xToEmptyPlaces)
        {
            Debug.Log("Х: " + emptyPlaces.Key);
            //Debug.Log("Пустых элементов: " + emptyPlaces.Value.Count);
            var maxEmptyY = emptyPlaces.Value.Max(item => item.GetComponent<PlaceController>().PlaceY);
            var bottomPlaces = allPlaces
                .Where(x =>
                {
                    var controller = x.GetComponent<PlaceController>();
                    var a = team == PlayerController.Team.Player1 ? controller.PlaceY < maxEmptyY : controller.PlaceY > maxEmptyY;
                    return controller.PlaceX == emptyPlaces.Key && !controller.isEmpty && a;

                })
                .ToList();
            if (bottomPlaces.Count < 1) continue;
            bottomPlaces.Reverse();
            var maxY = bottomPlaces.Max(item => item.GetComponent<PlaceController>().PlaceY);
            var offsetY = maxEmptyY - maxY;
            if (team == PlayerController.Team.Player2) offsetY *= -1;
            Debug.Log("Нужно передвинуть на " + offsetY);
            Debug.Log("Пустых клеток " + bottomPlaces.Count);
            //Проблема в обмене плейсами тут снизу найти потом
            MoveUp(allPlaces, bottomPlaces, emptyPlaces, offsetY);
        }

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void MoveUp(List<GameObject> allPlaces, List<GameObject> bottomPlaces, KeyValuePair<int, List<GameObject>> emptyPlaces, int offsetY)
    {
        //Debug.Log(bottomPlaces);
        foreach (var bottomPlace in bottomPlaces)
        {
            //Debug.Log("123");
            var oldPlaceController = bottomPlace.GetComponent<PlaceController>();
            var playerController = oldPlaceController.viewModel.GetComponent<PlayerController>();
            //Debug.Log($@"Ищем кЛетку с Y = " + (oldPlaceController.PlaceY + offsetY));
            var targetYPlaces = allPlaces
                .Select(x => x.GetComponent<PlaceController>())
                .Where(x => x.PlaceX == emptyPlaces.Key && x.PlaceY == (oldPlaceController.PlaceY + offsetY))
                .ToList();
            //Debug.Log("Найдено клеток " + newPlaceController.Count);
            if (targetYPlaces.Count < 1)
            {
                //Debug.Log("Ne Нашли");
                continue;
            }
            var newPlaceController = targetYPlaces[0];
            playerController.moveTo(newPlaceController.getPositionForModel(true));
            playerController.mapPosition = newPlaceController.GetCurrentMapPosition();
            playerController.placeController = newPlaceController;
            var t = newPlaceController.viewModel;
            newPlaceController.viewModel = oldPlaceController.viewModel;
            oldPlaceController.viewModel = null;
            newPlaceController.isEmpty = false;
            oldPlaceController.isEmpty = true;
            //oldPlaceController.viewModel = colX;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
