using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class InfoScr : ScrAbs
{
	[SerializeField]
	private Text textInfo;
	[SerializeField]
	private Text textDescription;
	[SerializeField]
	private Button closeBtn;
	
	
	public override void Show() {
		base.Show();
		closeBtn.onClick.AddListener(()=>RaiseEvent(ButtonId.CloseInfo));
	
	}
	

	public override void Hide() {
		base.Hide();
		closeBtn.onClick.RemoveAllListeners();
	}
	public override void Execute<T>(PageActionId action, T param) {
		base.Execute(action, param);
		if(action== PageActionId.SetTitleInfo && param is string str) {
			textInfo.text = str;
		}
		if (action == PageActionId.SetTextInfo && param is string txt) {
			textDescription.text = txt;
		}
	}


}
