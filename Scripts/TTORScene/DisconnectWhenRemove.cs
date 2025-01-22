using System.Collections.Generic;
using UnityEngine;

public class DisconnectWhenRemove : MonoBehaviour
{
    public List<ScreenCastBtn> screenCastBtns;

    private bool isOnScreen = false;

    // Update is called once per frame
    void Update()
    {
        isOnScreen = false;
        ScreenCastBtn onScreenItem = null;
        foreach (var item in screenCastBtns)
        {
         if(item.IsOnScreen)
            {
                isOnScreen = true;
                onScreenItem = item;
                break;
            }   
        }
        if (isOnScreen && Input.touchCount == 0 && !TTORStore.IsShow)
        {
            onScreenItem.ChangeScreenCast(string.Empty);
        }
    }
}
