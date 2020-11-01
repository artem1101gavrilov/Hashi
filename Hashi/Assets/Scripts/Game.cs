using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject NodePrefab;
    [SerializeField] private GameObject LinePrefab;

    private const int sizeHorizontal = 7;
    private const int sizeVertical = 10;
    public static int MaxLine = 3;

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
        //CreateGame()
    }

    public void CreateGame()
    {
        SetSizes();
        SetGameArray();
        SetPowers();
    }

    public void DestroyLine((int x, int y) position, DirectionNewNode direction)
    {
        switch (direction)
        {
            case DirectionNewNode.Up:
                // Забить дорогу в поле
                for (int upLine = position.x - 1; upLine >= 0; upLine--)
                {
                    if (gameArray[upLine, position.y] == -1)
                    {
                        gameArray[upLine, position.y] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case DirectionNewNode.Left:
                for (int leftLine = position.y - 1; leftLine >= 0; leftLine--)
                {
                    if (gameArray[position.x, leftLine] == -1)
                    {
                        gameArray[position.x, leftLine] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case DirectionNewNode.Down:
                for (int downLine = position.x + 1; downLine < sizeHorizontal; downLine--)
                {
                    if (gameArray[downLine, position.y] == -1)
                    {
                        gameArray[downLine, position.y] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case DirectionNewNode.Right:
                for (int rightLine = position.y + 1; rightLine < sizeVertical; rightLine--)
                {
                    if (gameArray[position.x, rightLine] == -1)
                    {
                        gameArray[position.x, rightLine] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
        }
    }

    public bool TryGetLink((int x, int y) position, DirectionNewNode direction, out Node node, out LineHashi line)
    {
        node = null;
        line = null;
        switch(direction)
        {
            case DirectionNewNode.Up:
                int up = position.x - 1;
                for (; up >= 0; up--)
                {
                    if (gameArray[up, position.y] == -1)
                    {
                        return false;
                    }
                    else if(gameArray[up, position.y] > 0)
                    {
                        break;
                    }
                }
                if (up == -1) return false;
                // Забить дорогу в поле
                for (int upLine = position.x - 1; upLine >= 0; upLine--)
                {
                    if (gameArray[upLine, position.y] == 0)
                    {
                        gameArray[upLine, position.y] = -1;
                    }
                    else 
                    {
                        break;
                    }
                }
                var thisNode = nodes[gameArray[position.x, position.y] - 1];
                node = nodes[gameArray[up, position.y] - 1];
                line = Instantiate(LinePrefab).GetComponent<LineHashi>();
                line.SetLine(thisNode.transform, node.transform);
                return true;
            case DirectionNewNode.Left:
                int i = position.y - 1;
                for (; i >= 0; i--)
                {
                    if (gameArray[position.x, i] == -1)
                    {
                        return false;
                    }
                    else if (gameArray[position.x, i] > 0)
                    {
                        break;
                    }
                }
                if (i == -1) return false;
                for (int leftLine = position.y - 1; leftLine >= 0; leftLine--)
                {
                    if (gameArray[position.x, leftLine] == 0)
                    {
                        gameArray[position.x, leftLine] = -1;
                    }
                    else
                    {
                        break;
                    }
                }
                var thisNodeLeft = nodes[gameArray[position.x, position.y] - 1];
                node = nodes[gameArray[position.x, i] - 1];
                line = Instantiate(LinePrefab).GetComponent<LineHashi>();
                line.SetLine(thisNodeLeft.transform, node.transform);
                return true;
            case DirectionNewNode.Down:
                int down = position.x + 1;
                for (; down < sizeHorizontal; down++)
                {
                    if (gameArray[down, position.y] == -1)
                    {
                        return false;
                    }
                    else if (gameArray[down, position.y] > 0)
                    {
                        break;
                    }
                }
                if (down == sizeHorizontal) return false;
                for (int downLine = position.x + 1; downLine < sizeHorizontal; downLine--)
                {
                    if (gameArray[downLine, position.y] == 0)
                    {
                        gameArray[downLine, position.y] = -1;
                    }
                    else
                    {
                        break;
                    }
                }
                var thisNodeDown = nodes[gameArray[position.x, position.y] - 1];
                node = nodes[gameArray[down, position.y] - 1];
                line = Instantiate(LinePrefab).GetComponent<LineHashi>();
                line.SetLine(thisNodeDown.transform, node.transform);
                return true;
            case DirectionNewNode.Right:
                int right = position.y + 1;
                for (; right < sizeVertical; right++)
                {
                    if (gameArray[position.x, right] == -1)
                    {
                        return false;
                    }
                    else if (gameArray[position.x, right] > 0)
                    {
                        break;
                    }
                }
                if (right == sizeVertical) return false;
                for (int rightLine = position.y + 1; rightLine < sizeVertical; rightLine--)
                {
                    if (gameArray[position.x, rightLine] == 0)
                    {
                        gameArray[position.x, rightLine] = -1;
                    }
                    else
                    {
                        break;
                    }
                }
                var thisNodeRight = nodes[gameArray[position.x, position.y] - 1];
                node = nodes[gameArray[position.x, right] - 1];
                line = Instantiate(LinePrefab).GetComponent<LineHashi>();
                line.SetLine(thisNodeRight.transform, node.transform);
                return true;
        }
        return false;
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

        // Очистка поля от дорог
        for (int i = 0; i < sizeHorizontal; i++)
        {
            for (int j = 0; j < sizeVertical; j++)
            {
                if (gameArray[i, j] == -1)
                {
                    gameArray[i, j] = 0;
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
                    node.game = this;
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
            var rnd = Random.Range(1, MaxLine + 1);
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
