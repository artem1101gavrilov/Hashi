using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Id { get; set; }
    public int Power { get; set; }
    public int CurrentPower { get; set; }
    [SerializeField] private TextMeshProUGUI Text;
    private List<NodeLink> links = new List<NodeLink>();
    public (int x, int y) gamePosition { get; set; }
    public Game game;
    private Image colorNode;

    private void Start()
    {
        colorNode = GetComponent<Image>();
    }

    public void PrintPower()
    {
        CurrentPower = Power;
        Text.text = Power.ToString();
    }

    public void AddLink(NodeLink link)
    {
        var direction = ((int)link.direction) % 2 == 0 ? (DirectionNewNode)(((int)link.direction) + 1) : (DirectionNewNode)(((int)link.direction) - 1);
        var newLink = new NodeLink() { FirstNode = this, SecondNode = link.FirstNode, direction = direction, Line = link.Line };
        links.Add(newLink);
    }

    public void RemoveLink(NodeLink link)
    {
        var removedLink = links.First(l => l.SecondNode == link.FirstNode);
        links.Remove(removedLink);
    }

    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    #endregion

    #region IDragHandler implementation
    public void OnDrag(PointerEventData eventData)
    {
    }
    #endregion

    #region IEndDragHandler implementation
    public void OnEndDrag(PointerEventData eventData)
    {
        var direction = Camera.main.ScreenToWorldPoint(eventData.position) - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (Mathf.Abs(angle) <= 45)
        {
            CreateOrUpdateLine(DirectionNewNode.Right);
        }
        else if (angle > 45 && angle <= 135)
        {
            CreateOrUpdateLine(DirectionNewNode.Up);
        }
        else if (angle < -45 && angle >= -135)
        {
            CreateOrUpdateLine(DirectionNewNode.Down);
        }
        else
        {
            CreateOrUpdateLine(DirectionNewNode.Left);
        }
    }
    #endregion

    private void CreateOrUpdateLine(DirectionNewNode direction)
    {
        var link = links.FirstOrDefault(l => l.direction == direction);
        if (link != null)
        {
            if (link.Line.Rank < Game.MaxLine)
            {
                link.Line.SetNewRank();
                DecreasePowerNodesLink(link);
            }
            else
            {
                game.DestroyLine(gamePosition, direction);
                link.Line.DestroyLine();
                links.Remove(link);
                link.SecondNode.RemoveLink(link);
                IncreasePowerNodesLink(link);
            }
        }
        else if (game.TryGetLink(gamePosition, direction, out var node, out var line))
        {
            var newLink = new NodeLink() { FirstNode = this, SecondNode = node, direction = direction, Line = line };
            links.Add(newLink);
            node.AddLink(newLink);
            DecreasePowerNodesLink(newLink);
        }
    }

    

    private void IncreasePowerNodesLink(NodeLink nodeLink)
    {
        nodeLink.FirstNode.CurrentPower += Game.MaxLine;
        nodeLink.FirstNode.ChangeColor();
        nodeLink.SecondNode.CurrentPower += Game.MaxLine;
        nodeLink.SecondNode.ChangeColor();
    }

    private void DecreasePowerNodesLink(NodeLink nodeLink)
    {
        nodeLink.FirstNode.CurrentPower--;
        nodeLink.FirstNode.ChangeColor();
        nodeLink.SecondNode.CurrentPower--;
        nodeLink.SecondNode.ChangeColor();
    }

    protected void ChangeColor()
    {
        if(CurrentPower == 0)
        {
            SetFinishColor();
        }
        else if(CurrentPower > 0)
        {
            SetNormalColor();
        }
        else
        {
            SetErrorColor();
        }
    }

    protected void SetErrorColor()
    {
        colorNode.color = new Color(1, 0, 0);
    }

    protected void SetFinishColor()
    {
        colorNode.color = new Color(0, 1, 1);
    }

    protected void SetNormalColor()
    {
        colorNode.color = new Color(1, 1, 1);
    }
}
