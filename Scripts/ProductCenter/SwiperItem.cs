using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ProductCenter
{
    public class SwiperItem : MonoBehaviour, IPointerClickHandler
    {
        public LevelLoader LevelLoader;
        public int id;
        private TextMeshProUGUI textName;

        void Start()
        {
            textName = transform.Find("TextName").gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void LoadDetail()
        {
            //DetailStore.ActiveDetailText = textName.text.Replace("注射液", "");

            LevelLoader.LoadNewScene("DetailScene");
        }

        /// <summary>
        /// 点击事件回调
        /// </summary>
        /// <param name="eventData">事件参数</param>
        public void OnPointerClick(PointerEventData eventData) => LoadDetail();

        /// <summary>
        /// 滑动到指定位置并改变缩放
        /// </summary>
        /// <param name="pos">新位置</param>
        /// <param name="scale">新缩放</param>
        public void SlidePosAndScale(Vector2 pos, Vector2 scale)
        {
            transform.DOScale(scale, 0.5f);
            transform.DOLocalMove(pos, 0.5f);
        }

        /// <summary>
        /// 消失并滑动到指定位置以及改变缩放，动作结束后出现
        /// </summary>
        /// <param name="pos">新位置</param>
        /// <param name="scale">新缩放</param>
        public void SlidHide(Vector2 pos, Vector2 scale)
        {
            gameObject.SetActive(false);
            transform.DOScale(scale, 1);
            transform.DOLocalMove(pos, 1).OnComplete(() =>
            {
                gameObject.SetActive(true);
            });
        }

        /// <summary>
        /// 立刻改变位置和缩放
        /// </summary>
        /// <param name="pos">新位置</param>
        /// <param name="scale">新缩放</param>
        public void SetPosAndScale(Vector2 pos, Vector2 scale)
        {
            transform.localScale = scale;
            transform.localPosition = pos;
        }
    }
}
