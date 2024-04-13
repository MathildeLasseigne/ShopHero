using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject parentObject;
    private TapGameController controller;


    public void SetController(TapGameController controller)
    {
        this.controller = controller;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("TappingSword"))
        {
            controller.TriggerTileInCollisionZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("TappingSword"))
        {
            Debug.Log("Trigger exit");
            controller.TriggerTileInCollisionZone(false);
        }
    }
}
