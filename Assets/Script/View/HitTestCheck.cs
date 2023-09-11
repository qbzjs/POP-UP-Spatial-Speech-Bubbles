using System;
using SpatialSpeechBubble;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.View
{
    public class HitTestCheck : MonoBehaviour, IPointerClickHandler
    {
        public Action<string> ClickLetter;
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("HitTestCheck.OnPointerClick() Letter");
            var letterData = eventData.pointerCurrentRaycast.gameObject.GetComponent<TextWriter>().text;
            ClickLetter?.Invoke(letterData);
        }
    }
}
