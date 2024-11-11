using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace BUT.TTOR.Core
{
    public class CalPuckSelectionBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject puckTogglePrefab;
        [SerializeField] private GameObject selectAPuckTextGO;
        private ToggleGroup _toggleGroup;
        private Toggle[] _toggles;

        private Toggle _activePuckToggle;

        public UnityEvent<string> OnActivePuckToggleChanged;

        private void Awake()
        {
            _toggleGroup = GetComponent<ToggleGroup>();
            _toggles = _toggleGroup.GetComponentsInChildren<Toggle>(true);

            CalUI calcUI = GetComponentInParent<CalUI>();
        }

        private void Start()
        {
            CreatePuckToggleButtons();
        }

        private void CreatePuckToggleButtons()
        {
            PuckData[] puckData = PuckDataManager.GetPuckData();

            ToggleGroup group = GetComponent<ToggleGroup>();

            for (int i = 0; i < puckData.Length; i++)
            {
                PuckToggle newPuckToggle = Instantiate(puckTogglePrefab, transform).GetComponent<PuckToggle>();
                newPuckToggle.Initialize(puckData[i]);
                Toggle toggle = newPuckToggle.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(Submit);
                toggle.group = group;
            }


            DeselectAll();
        }

        private void OnEnable()
        {
            //DeselectAll();
        }

        public void Submit(bool value)
        {
            _activePuckToggle = _toggleGroup.ActiveToggles().FirstOrDefault();
            if (_activePuckToggle != null)
            {
                OnActivePuckToggleChanged?.Invoke(_activePuckToggle.GetComponent<PuckToggle>().PuckData.name);

                selectAPuckTextGO.SetActive(false);
            }
            else
            {
                selectAPuckTextGO.SetActive(true);
                OnActivePuckToggleChanged?.Invoke("");
            }
        }

        public void DeselectAll()
        {
            _toggleGroup.SetAllTogglesOff();
            OnActivePuckToggleChanged?.Invoke("");
            selectAPuckTextGO.SetActive(true);
        }
    }
}
