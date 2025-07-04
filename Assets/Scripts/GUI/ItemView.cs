using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class ItemView : MonoBehaviour
{
    private Text text;
    private Button button;
    public Action<string> OnClick;
    public void Init(GameObject content) {
        text = GetComponentInChildren<Text>();
        button = GetComponentInChildren<Button>();
        transform.SetParent(content.transform, false);
		button.onClick.AddListener((() => OnClick?.Invoke(text.text)));
	}
    public void SetText(string str) {
        text.text = str;
    }    

	public class Factory : PlaceholderFactory<ItemView> { }
	private void OnDestroy() {
        button.onClick.RemoveAllListeners();
		text =null;
	}

	
}
