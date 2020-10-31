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

    /// <summary>
    /// Список всех узлов в игре
    /// </summary>
    private List<Node> nodes;
    private static int idNode = 1;

    private void Start()
    {
        SetSizes();
        SetGameArray();
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
        int x = Random.Range(0, sizeHorizontal);
        int y = Random.Range(0, sizeVertical);
        gameArray[x, y] = idNode++;
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
                var lengthDirection = Random.Range(0, 3);
                switch (direction)
                {
                    case DirectionNewNode.Up:
                        
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
                        gameArray[currentPoint.x, i] = idNode++;
                        openPoint.Add((currentPoint.x, i));
                        break;
                    case DirectionNewNode.Down:
                        break;
                    case DirectionNewNode.Right:

                        break;
                }
            }
        }
    }

    private bool CheckUp((int x, int y) currentPoint)
    {
        if (currentPoint.y > 0 && gameArray[currentPoint.x, currentPoint.y - 1] == 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckLeft((int x, int y) currentPoint)
    {
        if (currentPoint.x > 0 && gameArray[currentPoint.x - 1, currentPoint.y] == 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckDown((int x, int y) currentPoint)
    {
        if (currentPoint.y < (sizeVertical - 1) && gameArray[currentPoint.x, currentPoint.y + 1] == 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckRight((int x, int y) currentPoint)
    {
        if (currentPoint.x < (sizeHorizontal - 1) && gameArray[currentPoint.x + 1, currentPoint.y] == 0)
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
                    Instantiate(NodePrefab, new Vector3(leftSize + j * stepHorizontal, upSize - i * stepVertical), Quaternion.identity, transform);
                }
            }
        }
    }
}

public enum DirectionNewNode
{
    Right,
    Left,
    Up, 
    Down
}
