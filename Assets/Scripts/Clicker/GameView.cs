using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
	[SerializeField] private Transform vfxRoot;

	public event Action OnButtonClick;

	private AnimatorManager animatorCollectEffect;
	private AnimatorManager animatorCoinUp;

	private bool canceled;
	CancellationTokenSource cts = new();
	private void Awake() {
		_collectButton.onClick.AddListener(() => OnButtonClick?.Invoke());
		animatorCoinUp = new AnimatorManager(vfx_PlayCoinUp,vfxRoot);
		animatorCollectEffect = new AnimatorManager(vfx_PlayCollectEffect, vfxRoot);

	}

	public void SetCurrency(int value) {
		_currencyText.text = $"{value}";
	}

	public void SetEnergy(int value) {
		_energyText.text = $"{value}";
	}

	public async void PlayCollectEffect() {
		cts = null;
		cts = new();
		canceled = false;
		var stratPosition = (Vector2)_collectButton.transform.position;
		Vector2 offset = new Vector2(0,0.2f);
		try {
			EventBus.Bus.Invoke(EventId.OnSound, "earn");
			await animatorCoinUp.PlayAsync(stratPosition + offset, 0.8f).AttachExternalCancellation(cts.Token);			
			EventBus.Bus.Invoke(EventId.OnSound, "fire");
			if (!cts.IsCancellationRequested) {
				await animatorCollectEffect.PlayAsync(stratPosition + Vector2.up*2 + offset, 1).AttachExternalCancellation(cts.Token);
			}
		}
		catch (OperationCanceledException) { }
		
	}
	
	public void Clear() {
		animatorCoinUp.StopAll();
		animatorCollectEffect.StopAll();
		canceled = true;
		cts.Cancel();
	}

	

	public void PlayNoEnergyEffect() {
		Debug.Log("FX: no energy");
	}
	private void OnDestroy() {		
		animatorCoinUp.Dispose();
		animatorCollectEffect.Dispose();
		canceled = true;
		cts.Cancel();
	}
}
