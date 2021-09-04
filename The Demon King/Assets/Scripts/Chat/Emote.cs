using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emote : MonoBehaviour
{
      public GameObject EmoteObject;
      private Sprite image;
        private void Start()
        {
            if (image == null)
            {
                image = EmoteObject.gameObject.GetComponentInChildren<Image>().sprite;
                GetComponent<Image>().sprite = image;
            }
        }
}
