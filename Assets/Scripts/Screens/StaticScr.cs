using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticScr : ScrAbs {
	[SerializeField] Image timer;
	public override void Init() {
		base.Init();
		timer.enabled = false;
	}
	public override void Execute(PageActionId action) {
		base.Execute(action);
		if (action == PageActionId.ShowTimer) {
			timer.enabled = true;
		}
		if (action == PageActionId.HideTimer) {
			timer.enabled = false;
		}
	}
}

