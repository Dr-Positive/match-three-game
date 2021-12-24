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

    public PlayerGroup(List<GameObject> group, Color color, PlayerController.Fraction fraction = PlayerController.Fraction.None)
    {
        this.group = group;
        this.color = color;
        this.fraction = fraction;
    }

    public void ResetGroup()
    {
        attackEnemu(PlayerController.Team.Player2);
        foreach (var player in group)
        {
            player.GetComponent<PlayerController>().Reset();
        }
    }

    public void attackEnemu(PlayerController.Team enemyTeam)
    {
        var colums = group
            .Select(item => item.GetComponent<PlayerController>().mapPosition.x)
            .Distinct()
            .ToList();
        
        foreach(var currentX in colums)
        {
            var players = GameObject.FindGameObjectsWithTag("Player")
                .Where(item => item.GetComponent<PlayerController>().team == enemyTeam)
                .Where(item => item.GetComponent<PlayerController>().mapPosition.x == currentX)
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
                if (currentPlayer.canKill(currentEnemy.fraction))
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
    public PlayerController selectedPlayer;
    public List<PlayerGroup> playerGroups = new List<PlayerGroup>();
    public GameObject GroupList;
    void Start()   
    {
        GroupList = GameObject.Find("GroupList");
        var a = GameObject.FindGameObjectsWithTag("Place");
        foreach(var b in a)
        {
            b.GetComponent<PlaceController>().RenderModel();
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

    private PlayerGroup findGroup(GameObject currentPlayer, List<GameObject> players, List<MapPosition> passedCells, PlayerGroup result)
    {
        var currentPlayerController = currentPlayer.GetComponent<PlayerController>();
        //var ind = findInExistGroup(currentPlayerController);
        //if(ind >= 0)
        //{
        //    var group = playerGroups[ind];
        //    result.group.AddRange(group.group);
        //    result.color = group.color == Color.white ? new Color(Random.value, Random.value, Random.value) : group.color;
        //    playerGroups.Remove(group);
        //}

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
        //var neighborGroups = new List<GameObject>();
        foreach (var neighbor in neighbors)
        {
            result = findGroup(neighbor, players, passedCells, result);
        }
        //neighbors.AddRange(neighborGroups);
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
        var newGroups = new List<PlayerGroup>();
        var allPlayers = GameObject.FindGameObjectsWithTag("Player");
        var players = allPlayers
            .Where(item => item.GetComponent<PlayerController>().team == PlayerController.Team.Player1)
            .OrderBy(item => item.GetComponent<PlayerController>().mapPosition.x)
            .ThenBy(item => item.GetComponent<PlayerController>().mapPosition.y)
            .ToList();
        var passedCells = new List<MapPosition>();
        foreach (var player in players)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (passedCells.Contains(playerController.mapPosition)) continue;
            
            var group = findGroup(player, players, passedCells, new PlayerGroup(new List<GameObject> { player }, Color.white));
            //Debug.Log($@"Группа {group.group.Count}");
            var isValidGroup = group.group.Count >= 3;
            if (isValidGroup)
            {
                group.color = new Color(Random.value, Random.value, Random.value);
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

    IEnumerator RestartAll()
    {
        yield return new WaitForSeconds(0.1f);
        FillEmptyPlaces(PlayerController.Team.Player1);
        //FillEmptyPlaces(PlayerController.Team.Player2);
        UpdateList();
        FindMatches();
    }

    IEnumerator UpdateList()
    {
        yield return null;
        //Debug.Log("123");
        var view = Resources.Load("PlayerGroup") as GameObject;
        var GroupList = GameObject.Find("GroupList");
        for (int i = 0; i < GroupList.transform.childCount; i++)
        {
            Destroy(GroupList.transform.GetChild(i).gameObject);
        }
        var index = 0;
        foreach (var group in playerGroups)
        {
            var obj = Instantiate(view, GroupList.transform.position - new Vector3(0, 30 + 65 * index, 0), Quaternion.identity);
            obj.transform.GetComponent<Image>().color = group.color;
            obj.transform.Find("Name").GetComponent<Text>().color = group.color;
            obj.transform.Find("Name").GetComponent<Text>().text = $@"Группа длинны {group.group.Count}";
            obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                //Debug.Log("delete");
                group.ResetGroup();
                playerGroups.Remove(group);
                StartCoroutine(RestartAll());
            });
            index++;
            obj.transform.SetParent(GroupList.transform);
        }
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
            //Destroy(placeController.viewModel);
            //placeController.renderer.material.color = Color.white;
            placeController.RenderModel();
        }
        StartCoroutine(RestartAll());
    }
    public void doSwith(PlayerController player)
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
        //Dictionary<int, List<GameObject>> places;
        var allPlaces = GameObject.FindGameObjectsWithTag("Place")
            .Where(item =>
            {
                var controller = item.GetComponent<PlaceController>();
                return controller.PlaceTeam == team;
            })
            .ToList();
        var xToEmptyPlaces = allPlaces
            .Where(item =>
            {
                var controller = item.GetComponent<PlaceController>();
                return controller.isEmpty;
            })
            .GroupBy(item => item.GetComponent<PlaceController>().PlaceX)
            .ToDictionary(item => item.Key, item => item.ToList());

        foreach (var emptyPlaces in xToEmptyPlaces)
        {
            Debug.Log("Х: " + emptyPlaces.Key);
            Debug.Log("Пустых элементов: " + emptyPlaces.Value.Count);
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
            //if (team == PlayerController.Team.Player2) offsetY *= -1;
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
        Debug.Log(bottomPlaces);
        foreach (var bottomPlace in bottomPlaces)
        {
            Debug.Log("123");
            var oldPlaceController = bottomPlace.GetComponent<PlaceController>();
            var playerController = oldPlaceController.viewModel.GetComponent<PlayerController>();
            Debug.Log($@"Ищем кЛетку с Y = " + (oldPlaceController.PlaceY + offsetY));
            var targetYPlaces = allPlaces
                .Select(x => x.GetComponent<PlaceController>())
                .Where(x => x.PlaceX == emptyPlaces.Key && x.PlaceY == (oldPlaceController.PlaceY + offsetY))
                .ToList();
            //Debug.Log("Найдено клеток " + newPlaceController.Count);
            if (targetYPlaces.Count < 1)
            {
                Debug.Log("Ne Нашли");
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
