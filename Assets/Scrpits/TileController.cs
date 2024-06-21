using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    public TileState state { get; set; }
    public TileCell cell { get; set; }
    public int number { get; set; }
    public bool locked { get; set; }

    private Image background;
    private TextMeshProUGUI text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.numberColor;
        text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tileController = null;
        }
        this.cell = cell;
        this.cell.tileController = this;
        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tileController = null;
        }
        this.cell = cell;
        this.cell.tileController = this;
        Vector2 newPosition = cell.transform.position;
        StartCoroutine(Animate(newPosition, false));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tileController = null;
        }
        this.cell = null;
        cell.tileController.locked = true;
        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float timer = 0f;
        float duration = 0.1f;
        Vector3 from = transform.position;
        while (timer < duration)
        {
            transform.position = Vector3.Lerp(from, to, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
        if (merging)
        {
            Destroy(gameObject);
        }
    }

}
