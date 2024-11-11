using TMPro;
using UnityEngine;

namespace BUT.TTOR.Examples
{
    public class InfoText : MonoBehaviour
    {
        public TMP_Text TextInfo;
        public TMP_Text PlacePuckText;

        public GlobeController GlobeController;

        private void Start()
        {
            GlobeController.OnActivePinChange.AddListener(ChangeText);
        }

        public void ChangeText(PinInfo pin)
        {
            if (pin != null)
            {
                TextInfo.text =
                "Country: " + "<color=#EF1F79>" + "<b>" +  pin.Name + "</b>"  + "</color >" + "<br>" +
                "Capitol: " + pin.Capitol + "<br>" +
                "Population: " + pin.Population + "<br>" +
                "Currency: " + pin.Currency + "<br>";   
            }

            if(pin == null)
            {
                TextInfo.text = "";
            }
        }
    }
}
