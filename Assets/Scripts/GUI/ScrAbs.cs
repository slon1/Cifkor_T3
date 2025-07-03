using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public abstract class ScrAbs : MonoBehaviour, IPage {
	[SerializeField]
	protected List<EventButton> EventButtons;
	[SerializeField]
	protected GameObject root;
	[SerializeField]
	protected PanelId panelId;
	private CanvasGroup canvas;

	public GameObject Root => root;

	public PanelId PanelID => panelId;




	public bool Static;
	public bool IsStatic() {
		return Static;
	}
	private Dictionary<ButtonId, List<EventButton>> dict;

	private void Awake() {
		canvas = GetComponent<CanvasGroup>();
	}

	public virtual void Init() {
		EventButtons.ForEach(button => button.InitEvent());
		dict = EventButtons.GroupBy(x => x.ButtonId).ToDictionary(g => g.Key, g => g.ToList());

	}
	public Button GetButton(ButtonId buttonId, string tag = null) {
		if (dict.TryGetValue(buttonId, out var list)) {
			if (!string.IsNullOrEmpty(tag)) {
				var found = list.FirstOrDefault(b => b.Tag == tag);
				if (found != null)
					return found.Button;
			}
			return list[0].Button;
		}
		return null;
	}

	protected void RaiseEvent(ButtonId buttonId) {
		EventBus.Bus.Invoke(EventId.MenuEvent, buttonId);
	}
	



	public virtual void Hide() {
		if (root != null) {
			canvas.blocksRaycasts = false;
			canvas.DOFade(0, 0.5f).OnComplete(()=> 
			canvas.alpha=0);
		}
		
	}

	public virtual void Show() {
		if (root != null) {
	
			canvas.DOFade(1, 0.5f).OnComplete(() => {
				canvas.blocksRaycasts = true;
	
			});
		}
	}

	public bool IsVisible() {
		return root.activeInHierarchy;
	}

	public virtual void Execute<T>(PageActionId action, T param) {

	}
	private void OnDestroy() {
		DOTween.Kill(canvas);
		canvas = null;
	}

	public virtual void Execute(PageActionId action) {
		
	}

	
}


