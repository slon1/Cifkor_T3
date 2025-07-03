using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class GamePresenter : IDisposable {
	private readonly GameStateModel _model;
	private readonly IGameView _view;

	private CancellationTokenSource _cts;

	public GamePresenter(GameStateModel model, IGameView view) {
		_model = model;
		_view = view;		

	}
	public void Start() {
		_cts = new();
		_model.Currency.OnChanged += OnCurrencyChanged;
		_model.Energy.OnChanged += OnEnergyChanged;

		// сразу отрисовать начальные данные
		_view.SetCurrency(_model.Currency.Value);
		_view.SetEnergy(_model.Energy.Value);

		// подписываемся на клики
		_view.OnButtonClick += OnButtonClicked;
		StartAutoCollect(_cts.Token).Forget();
		StartEnergyReplenish(_cts.Token).Forget();
	}
	private void OnCurrencyChanged(int value) {
		_view.SetCurrency(value);
	}

	private void OnEnergyChanged(int value) {
		_view.SetEnergy(value);
	}

	private void OnButtonClicked() {
		TryCollect();
	}

	private void TryCollect() {
		if (_model.Energy.TrySpend(1)) {
			_model.Currency.Add(1);
			_view.PlayCollectEffect(); // анимация FX
		}
		else {
			_view.PlayNoEnergyEffect(); // вибрация, мигание и т.д.
		}
	}

	private async UniTaskVoid StartAutoCollect(CancellationToken token) {
		while (!token.IsCancellationRequested) {
			await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
			TryCollect();
		}
	}

	private async UniTaskVoid StartEnergyReplenish(CancellationToken token) {
		while (!token.IsCancellationRequested) {
			await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: token);
			_model.Energy.Add(10);
		}
	}

	public void Dispose() {
		_cts.Cancel();

		_view.OnButtonClick -= OnButtonClicked;

		_model.Currency.OnChanged -= OnCurrencyChanged;
		_model.Energy.OnChanged -= OnEnergyChanged;
	}
}
