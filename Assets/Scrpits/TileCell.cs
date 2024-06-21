using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileCell : MonoBehaviour
{
    public Vector2Int address { get; set; }

    public TileController tileController { get; set; }

    public bool empty => tileController == null;

    public bool occupied => tileController != null;

    //public void changeColor()
    //{
    //    Image img = GetComponent<Image>();
    //    img.color = Color.red;
    //}
    //public void changeWhiteColor()
    //{
    //    Image img = GetComponent<Image>();
    //    img.color = Color.white;
    //}
}
