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
    
    /// <summary>
    /// Список всех узлов в игре
    /// </summary>
    private List<Node> nodes;

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
        gameArray[x, y] = 1;
        CreateAllNodes();
    }

    private void CreateAllNodes()
    {
        for (int i = 0; i < sizeHorizontal; i++)
        {
            for (int j = 0; j < sizeVertical; j++)
            {
                //if(gameArray[i, j] > 0)
                {
                    Instantiate(NodePrefab, new Vector3(leftSize + j * stepHorizontal, upSize - i * stepVertical), Quaternion.identity, transform);
                }
            }
        }
    }
}
