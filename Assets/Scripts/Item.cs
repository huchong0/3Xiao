using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Item : MonoBehaviour,IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData)
    {
        PanelController.instance.clickItem(this);
    }

    public void appearBorder()
    {
        Image border = GetComponent<Image>();
        border.enabled = true;
    }
    public void disappearBorder()
    {
        Image border = GetComponent<Image>();
        border.enabled = false;
    }

    IEnumerator  moveToGrid(GameObject Grid)
    {
        float time = 0.3f;
        float countTime = 0f;
        while(countTime<time)
        {
            countTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, Grid.transform.position, countTime / time);
            yield return null;
        }

    }
    IEnumerator fallToGrid(GameObject Grid)
    {
        float time = 1f;
        float countTime = 0f;
        while (countTime < time)
        {
            countTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, Grid.transform.position, countTime / time);
            yield return null;
        }

    }


    public void ExchangeToItem(Item Item)
    {
        var PreGrid = transform.parent.gameObject;
        StartCoroutine(moveToGrid(Item.transform.parent.gameObject));
        StartCoroutine(Item.moveToGrid(PreGrid));
        transform.SetParent(Item.transform.parent);
        Item.transform.SetParent(PreGrid.transform);
    }

    public void FallToGrid(GameObject Grid)
    {
        StartCoroutine(fallToGrid(Grid));
        transform.SetParent(Grid.transform);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
