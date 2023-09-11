using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    public class LetterSentenceView : MonoBehaviour
    {
        [SerializeField] private HitTestCheck _hitTestCheck;
        [SerializeField] private DismissButton _dismissButton;
        [SerializeField] private RectTransform panel;
        [SerializeField] private GameObject parentPanel;
        [SerializeField] private Text letterPanelText;
        private bool IsMovePanel = false;

        private void Start()
        {
            _hitTestCheck.ClickLetter += ShowPanel;
            _dismissButton.ClickDismissButton += HiddenPanel;
        }

        private void Update()
        {
            if (!IsMovePanel) return;
            PanelMove();
        }

        public void ShowPanel(string message)
        {
            Debug.Log("ShowPanel");
            parentPanel.gameObject.SetActive(true);
            letterPanelText.text = message;
            IsMovePanel = true;
        }

        public void HiddenPanel()
        {
            Debug.Log("HiddenPanel");
            parentPanel.gameObject.SetActive(false);
            letterPanelText.text = "";
            IsMovePanel = false;
            panel.offsetMax = new Vector2(panel.offsetMax.x, -2000);
        }
    
        private void PanelMove()
        {
            if (panel.offsetMax.y >= -400 && IsMovePanel)
            {
                panel.offsetMax = new Vector2(panel.offsetMax.x, -400);;
                IsMovePanel = false;
                return;
            }
            panel.offsetMax += new Vector2(0, 40);
        }

        private void OnDestroy()
        {
            _hitTestCheck.ClickLetter -= ShowPanel;
            _dismissButton.ClickDismissButton -= HiddenPanel;
        }
    }
}