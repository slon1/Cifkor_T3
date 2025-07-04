using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public interface IGameView {
	event Action OnButtonClick;

	void SetCurrency(int value);
	void SetEnergy(int value);
	void PlayCollectEffect();
	void PlayNoEnergyEffect();

	void Clear();
}

public class GameView : MonoBehaviour, IGameView {
	[SerializeField] private Text _currencyText;
	[SerializeField] private Text _energyText;
	[SerializeField] private Button _collectButton;

	[SerializeField] private GameObject vfx_PlayCollectEffect;
	[SerializeField] private GameObject vfx_PlayCoinUp;

	public event Action OnButtonClick;

	private AnimatorManager animatorCollectEffect;
	private AnimatorManager animatorCoinUp;
	private void Awake() {
		_collectButton.onClick.AddListener(() => OnButtonClick?.Invoke());
		animatorCoinUp = new AnimatorManager(vfx_PlayCoinUp);
		animatorCollectEffect = new AnimatorManager(vfx_PlayCollectEffect);

	}

	public void SetCurrency(int value) {
		_currencyText.text = $"{value}";
	}

	public void SetEnergy(int value) {
		_energyText.text = $"{value}";
	}

	public async void PlayCollectEffect() {
		canceled = false;
		var stratPosition = (Vector2)_collectButton.transform.position;
		Vector2 offset = new Vector2(0,0.2f);
		EventBus.Bus.Invoke(EventId.OnSound, "earn");
		await animatorCoinUp.PlayAsync(stratPosition + offset,0.8f);
		if (canceled) { return; };
		EventBus.Bus.Invoke(EventId.OnSound, "fire");
		await animatorCollectEffect.PlayAsync(stratPosition + Vector2.up + offset, 1);		
		
	}
	private bool canceled;
	public void Clear() {
		animatorCoinUp.StopAll();
		animatorCollectEffect.StopAll();
		canceled = true;
	}

	

	public void PlayNoEnergyEffect() {
		Debug.Log("FX: no energy");
	}
	private void OnDestroy() {		
		animatorCoinUp.Dispose();
		animatorCollectEffect.Dispose();
	}
}
