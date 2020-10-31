using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void PrintPower()
    {
        CurrentPower = Power;
        Text.text = Power.ToString();
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
        Debug.Log(angle);
    }
    #endregion
}
