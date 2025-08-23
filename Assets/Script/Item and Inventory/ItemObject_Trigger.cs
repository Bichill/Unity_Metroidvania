using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<Player>() != null)
        {
            if(_collision.GetComponent<CharacterStats>().isDead)return;

            Debug.Log("Picked up item");
            myItemObject.PickupItem();
        }
    }
}
