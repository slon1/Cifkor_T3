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
}

public class GameView : MonoBehaviour, IGameView {
	[SerializeField] private Text _currencyText;
	[SerializeField] private Text _energyText;
	[SerializeField] private Button _collectButton;

	public event Action OnButtonClick;

	private void Awake() {
		_collectButton.onClick.AddListener(() => OnButtonClick?.Invoke());
	}

	public void SetCurrency(int value) {
		_currencyText.text = $"{value}";
	}

	public void SetEnergy(int value) {
		_energyText.text = $"{value}";
	}

	public void PlayCollectEffect() {
		Debug.Log("FX: collect");
	}

	public void PlayNoEnergyEffect() {
		Debug.Log("FX: no energy");
	}
}
