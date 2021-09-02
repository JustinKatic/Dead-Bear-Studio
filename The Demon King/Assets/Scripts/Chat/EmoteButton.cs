using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteButton : MonoBehaviour
{
      public GameObject Emote;
      private Sprite image;
        private void Start()
        {
            if (image == null)
            {
                image = Emote.gameObject.GetComponentInChildren<Image>().sprite;
                GetComponent<Image>().sprite = image;
            }
        }
}
