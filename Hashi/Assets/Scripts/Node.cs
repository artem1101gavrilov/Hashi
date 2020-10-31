using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public int Id { get; set; }
    public int Power { get; set; }
    [SerializeField] private TextMeshProUGUI Text;
    private List<NodeLink> links = new List<NodeLink>();
    public (int x, int y) gamePosition { get; set; }

    public void PrintPower()
    {
        Text.text = Power.ToString();
    }
}
