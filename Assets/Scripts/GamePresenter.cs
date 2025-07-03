using Cysharp.Threading.Tasks;
using System;

public class GamePresenter {
	private readonly GameStateModel _model;
	private readonly IGameView _view;
	private readonly IAsyncTimer _autoCollectTimer;
	private readonly IAsyncTimer _energyTimer;
	

	public GamePresenter(GameStateModel model, IGameView view) {
		_model = model;
		_view = view;
		_autoCollectTimer = new AsyncTimer();
		_energyTimer = new AsyncTimer();
	}
	public void Start() {	

		_model.Currency.OnChanged += OnCurrencyChanged;
		_model.Energy.OnChanged += OnEnergyChanged;

		_view.SetCurrency(_model.Currency.Value);
		_view.SetEnergy(_model.Energy.Value);
				
		_view.OnButtonClick += OnButtonClicked;
		
		_autoCollectTimer.Start(async () => {
			TryCollect();
			await UniTask.CompletedTask;
		}, TimeSpan.FromSeconds(3));

		_energyTimer.Start(async () => {
			_model.Energy.Add(10);
			await UniTask.CompletedTask;
		}, TimeSpan.FromSeconds(10));
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
	
	public void Stop() {
		//_cts.Cancel();

		_autoCollectTimer.Stop();
		_energyTimer.Stop();

		_view.OnButtonClick -= OnButtonClicked;
		_model.Currency.OnChanged -= OnCurrencyChanged;
		_model.Energy.OnChanged -= OnEnergyChanged;
	}
}
