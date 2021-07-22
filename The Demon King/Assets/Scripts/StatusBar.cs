using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider _slider;
   
       public void SetMaxHealth(int health)
       {
           _slider.maxValue = health;
           _slider.value = health;
       }
       

}
