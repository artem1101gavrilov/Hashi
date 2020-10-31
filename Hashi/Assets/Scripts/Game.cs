using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject NodePrefab;
    [SerializeField] private GameObject LinePrefab;

    private const int sizeHorizontal = 7;
    private const int sizeVertical = 10;

    /// <summary>
    /// Игровое поле.
    /// 0 - пустота
    /// -1 - линия
    /// 1..inf - номера узлов
    /// </summary>
    private int[,] gameArray = new int[sizeHorizontal, sizeVertical];

    /// <summary>
    /// Точка на канвасе для размеров игрового поля.
    /// </summary>
    [SerializeField] private Transform startPoint;
    private float leftSize;
    private float rightSize;
    private float upSize;
    private float downSize;
    private float stepHorizontal;
    private float stepVertical;

    private List<(int x, int y)> openPoint = new List<(int x, int y)>();
    private List<(int first, int second)> links = new List<(int first, int second)>();

    /// <summary>
    /// Список всех узлов в игре
    /// </summary>
    private List<Node> nodes = new List<Node>();
    private static int idNode = 1;
    private static int currentId = 0;

    private void Start()
    {
        SetSizes();
        SetGameArray();
        SetPowers();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            SetGameArray();
            SetPowers();
        }
    }

    private void SetSizes()
    {
        leftSize = startPoint.position.x;
        rightSize = -startPoint.position.x;
        upSize = startPoint.position.y;
        downSize = -startPoint.position.y;
        stepHorizontal = (rightSize - leftSize) / (sizeVertical - 1);
        stepVertical = (upSize - downSize) / (sizeHorizontal - 1);
    }

    private void SetGameArray()
    {
        idNode = 1;
        currentId = 0;
        gameArray = new int[sizeHorizontal, sizeVertical];
        nodes = new List<Node>();
        links = new List<(int first, int second)>();

        int x = Random.Range(0, sizeHorizontal);
        int y = Random.Range(0, sizeVertical);
        gameArray[x, y] = idNode;
        openPoint.Add((x, y));
        GenerationGameArray();
        CreateAllNodes();
    }

    private void GenerationGameArray()
    {
        var possibleDirections = new List<DirectionNewNode>() { DirectionNewNode.Down, DirectionNewNode.Left, DirectionNewNode.Right, DirectionNewNode.Up };
        while (openPoint.Count > 0)
        {
            var currentPoint = openPoint[0];
            ++currentId;
            openPoint.Remove(currentPoint);

            // Проверяем где могут быть новые узлы от данного
            var possibleDirection = new List<DirectionNewNode>(possibleDirections);
            if (CheckUp(currentPoint) == false) possibleDirection.Remove(DirectionNewNode.Up);
            if (CheckLeft(currentPoint) == false) possibleDirection.Remove(DirectionNewNode.Left);
            if (CheckDown(currentPoint) == false) possibleDirection.Remove(DirectionNewNode.Down);
            if (CheckRight(currentPoint) == false) possibleDirection.Remove(DirectionNewNode.Right);

            // Убираем возможный 1 путь движения
            if(possibleDirection.Count > 0)
            {
                var countDirection = Random.Range(possibleDirection.Count - 1, possibleDirection.Count + 1);
                if(countDirection != possibleDirection.Count)
                {
                    possibleDirection.RemoveAt(Random.Range(0, possibleDirection.Count));
                }
            }

            // Создаем новые узлы
            foreach (var direction in possibleDirection)
            {
                var lengthDirection = GetRandomLength();
                switch (direction)
                {
                    case DirectionNewNode.Up:
                        int up = currentPoint.x - 1;
                        for (; up >= 0 && up > currentPoint.x - lengthDirection; up--)
                        {
                            if (gameArray[up, currentPoint.y] == 0)
                            {
                                gameArray[up, currentPoint.y] = -1;
                            }
                            else
                            {
                                up++;
                                break;
                            }
                        }
                        if (up < 0) up = 0;
                        gameArray[up, currentPoint.y] = ++idNode;
                        openPoint.Add((up, currentPoint.y));
                        links.Add((currentId, idNode));
                        break;
                    case DirectionNewNode.Left:
                        int i = currentPoint.y - 1;
                        for (; i >= 0 && i > currentPoint.y - lengthDirection; i--)
                        {
                            if (gameArray[currentPoint.x, i] == 0)
                            {
                                gameArray[currentPoint.x, i] = -1;
                            }
                            else
                            {
                                i++;
                                break;
                            }
                        }
                        if (i < 0) i = 0;
                        gameArray[currentPoint.x, i] = ++idNode;
                        openPoint.Add((currentPoint.x, i));
                        links.Add((currentId, idNode));
                        break;
                    case DirectionNewNode.Down:
                        int down = currentPoint.x + 1;
                        for (; down < sizeHorizontal && down < currentPoint.x + lengthDirection; down++)
                        {
                            if (gameArray[down, currentPoint.y] == 0)
                            {
                                gameArray[down, currentPoint.y] = -1;
                            }
                            else
                            {
                                down--;
                                break;
                            }
                        }
                        if (down >= sizeHorizontal) down = sizeHorizontal - 1;
                        gameArray[down, currentPoint.y] = ++idNode;
                        openPoint.Add((down, currentPoint.y));
                        links.Add((currentId, idNode));
                        break;
                    case DirectionNewNode.Right:
                        int right = currentPoint.y + 1;
                        for (; right < sizeVertical && right < currentPoint.y + lengthDirection; right++)
                        {
                            if (gameArray[currentPoint.x, right] == 0)
                            {
                                gameArray[currentPoint.x, right] = -1;
                            }
                            else
                            {
                                right--;
                                break;
                            }
                        }
                        if (right >= sizeVertical) right = sizeVertical - 1;
                        gameArray[currentPoint.x, right] = ++idNode;
                        openPoint.Add((currentPoint.x, right));
                        links.Add((currentId, idNode));
                        break;
                }
            }
        }
    }

    private bool CheckLeft((int x, int y) currentPoint)
    {
        if (currentPoint.y > 1 && gameArray[currentPoint.x, currentPoint.y - 1] == 0 &&
            gameArray[currentPoint.x, currentPoint.y - 2] == 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckUp((int x, int y) currentPoint)
    {
        if (currentPoint.x > 1 && gameArray[currentPoint.x - 1, currentPoint.y] == 0 &&
            gameArray[currentPoint.x - 2, currentPoint.y] == 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckRight((int x, int y) currentPoint)
    {
        if (currentPoint.y < (sizeVertical - 2) && gameArray[currentPoint.x, currentPoint.y + 1] == 0 && gameArray[currentPoint.x, currentPoint.y + 2] == 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckDown((int x, int y) currentPoint)
    {
        if (currentPoint.x < (sizeHorizontal - 2) && gameArray[currentPoint.x + 1, currentPoint.y] == 0 && gameArray[currentPoint.x + 2, currentPoint.y] == 0)
        {
            return true;
        }
        return false;
    }

    private void CreateAllNodes()
    {
        for (int i = 0; i < sizeHorizontal; i++)
        {
            for (int j = 0; j < sizeVertical; j++)
            {
                if(gameArray[i, j] > 0)
                {
                    var node = Instantiate(NodePrefab, new Vector3(leftSize + j * stepHorizontal, upSize - i * stepVertical), Quaternion.identity, transform).GetComponent<Node>();
                    node.Id = gameArray[i, j];
                    node.gamePosition = (i, j);
                    nodes.Add(node);
                }
            }
        }

        nodes = nodes.OrderBy(n => n.Id).ToList();
    }

    private void SetPowers()
    {
        foreach (var link in links)
        {
            var rnd = Random.Range(1, 4);
            nodes[link.first - 1].Power += rnd;
            nodes[link.second - 1].Power += rnd;
        }
        foreach (var node in nodes)
        {
            node.PrintPower();
        }
    }

    private int GetRandomLength()
    {
        var a = Random.Range(1, 101);
        if (a <= 15) return 0;
        if (a <= 50) return 1;
        if (a <= 85) return 2;
        if (a <= 98) return 3;
        return 4;
    }
}

public enum DirectionNewNode
{
    Right,
    Left,
    Up, 
    Down
}
