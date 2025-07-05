using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIManager : MonoBehaviour, IGUIManager {
	

	private Dictionary<PanelId, IPage> panels;
	[SerializeField]
	private List<ScrAbs> screens;
	private PanelId lastOpen;

	private void Start() {
		Initialize();
	}

	public void Initialize() {
		panels = screens.ToDictionary(panel => panel.PanelID, panel => (IPage)panel);
		screens.ForEach(x => x.Init());
		EventBus.Bus.AddListener<ButtonId>(EventId.MenuEvent, OnMenuEvent);				
		ShowPanel(PanelId.page1);
	}

	private void OnMenuEvent(ButtonId id) {
		
		switch (id) {
			case ButtonId.GotoPage1:
				ShowPanel(PanelId.page1);				
				break;
			case ButtonId.GotoPage2:
				ShowPanel(PanelId.page2);				
				break;
			case ButtonId.GotoPage3:
				ShowPanel(PanelId.page3);							
				break;
			case ButtonId.ShowTimer:
				Execute(PanelId._static, PageActionId.ShowTimer);
				break;
			case ButtonId.HideTimer:
				Execute(PanelId._static, PageActionId.HideTimer);
				break;
			case ButtonId.CloseInfo:
				ShowPanelModal(PanelId.info, false);
				break;
			default:
				break;
		}
	}

	public void Back() {
		//Installer.GetService<IGameManager>().ShowPanel(lastOpen[lastOpen.Count-2]);

	}
	public void ShowPanelModal(PanelId panelId, bool show) {
		if (show) {			
			panels[panelId].Show();
		}
		else {
			panels[panelId].Hide();
		}
		


	}
	public void ShowPanel(PanelId panelId) {
		if (lastOpen == panelId) 
			{ return; }

		foreach (var panel in panels.Values) {
			if (panel.IsStatic()) {
				continue;
			}
			if (panel.PanelID == panelId) {

				panel.Show();				
			}
			else {
				panel.Hide();
			}
		}

		lastOpen = panelId;

	}

	private void OnDestroy() {
		
		panels = null;
		EventBus.Bus.RemoveListener<ButtonId>(EventId.MenuEvent, OnMenuEvent);
	}


	public void Execute<T>(PanelId panelId, PageActionId action, T param) {
		panels[panelId].Execute(action, param);
	}


	public void Execute(PanelId panelId, PageActionId action) {
		panels[panelId].Execute(action);
	}

	
}
