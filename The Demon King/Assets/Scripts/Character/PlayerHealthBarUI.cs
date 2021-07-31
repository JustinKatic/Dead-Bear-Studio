using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    public List<Image> healthBars = new List<Image>(); 
    // Start is called before the first frame update
    void Start()
    {
        healthBars = GetComponentsInChildren<Image>().ToList();
    }
    
}
