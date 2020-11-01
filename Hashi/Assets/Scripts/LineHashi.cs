using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineHashi : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private LineRenderer lineRenderer;
    public int Rank { get; private set; }

    public void SetLine(Transform Start, Transform End)
    {
        lineRenderer = GetComponent<LineRenderer>();
        startPosition = Start.position;
        endPosition = End.position;
        Rank = 1;
        DrawLine();
    }

    public void SetNewRank()
    {
        Rank++;
        if (Rank <= 3)
        {
            DrawLine();
        }
        else
        {
            DestroyLine();
        }
    }

    public void DestroyLine()
    {
        Destroy(gameObject);
    }

    private void DrawLine()
    {
        switch(Rank)
        {
            case 1:
                DrawOneLine();
                break;
            case 2:
                DrawTwoLines();
                break;
            case 3:
                DrawThreeLines();
                break;
        }
    }

    private void DrawOneLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void DrawTwoLines()
    {
        lineRenderer.positionCount = 4;
        if((Mathf.Abs(startPosition.y - endPosition.y) < 0.1f) && (Mathf.Abs(startPosition.x - endPosition.x) > 0.1f))
        {
            lineRenderer.SetPosition(0, startPosition + new Vector3(0, 0.2f));
            lineRenderer.SetPosition(1, endPosition + new Vector3(0, 0.2f));
            lineRenderer.SetPosition(2, endPosition + new Vector3(0, -0.2f));
            lineRenderer.SetPosition(3, startPosition + new Vector3(0, -0.2f));
        }
        else
        {
            lineRenderer.SetPosition(0, startPosition + new Vector3(0.2f, 0));
            lineRenderer.SetPosition(1, endPosition + new Vector3(0.2f, 0));
            lineRenderer.SetPosition(2, endPosition + new Vector3(-0.2f, 0));
            lineRenderer.SetPosition(3, startPosition + new Vector3(-0.2f, 0));
        }
    }

    private void DrawThreeLines()
    {
        lineRenderer.positionCount = 6;
        if ((Mathf.Abs(startPosition.y - endPosition.y) < 0.1f) && (Mathf.Abs(startPosition.x - endPosition.x) > 0.1f))
        {
            lineRenderer.SetPosition(0, startPosition + new Vector3(0, 0.2f));
            lineRenderer.SetPosition(1, endPosition + new Vector3(0, 0.2f));
            lineRenderer.SetPosition(2, endPosition);
            lineRenderer.SetPosition(3, startPosition);
            lineRenderer.SetPosition(4, startPosition + new Vector3(0, -0.2f));
            lineRenderer.SetPosition(5, endPosition + new Vector3(0, -0.2f));
        }
        else
        {
            lineRenderer.SetPosition(0, startPosition + new Vector3(0.2f, 0));
            lineRenderer.SetPosition(1, endPosition + new Vector3(0.2f, 0));
            lineRenderer.SetPosition(2, endPosition);
            lineRenderer.SetPosition(3, startPosition);
            lineRenderer.SetPosition(4, startPosition + new Vector3(-0.2f, 0));
            lineRenderer.SetPosition(5, endPosition + new Vector3(-0.2f, 0));
        }
    }
}
