using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject parentObject;
    private TapGameController controller;

    public GameObject wingStart;
    public GameObject wingEnd;

    public bool isLong { get; private set; } = false;
    public bool isStart { get; private set; } = false;
    public bool isEnd { get; private set; } = false;



    public void SetController(TapGameController controller)
    {
        this.controller = controller;
    }


    public void SetIsLongStart()
    {
        isLong = true;
        isStart = true;
        wingStart?.SetActive(true);
    }

    public void SetIsLongEnd()
    {
        isLong = true;
        isEnd = true;
        wingEnd?.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("TappingSword"))
        {
            controller.TriggerTileInCollisionZone(this, true, wait: isLong);
        } else if(collision.gameObject.layer == LayerMask.NameToLayer("GarbageCollector"))
        {
            controller.RegisterToDestroy(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("TappingSword"))
        {
            //Debug.Log("Trigger exit");
            controller.TriggerTileInCollisionZone(this, false);
        }
    }

}
