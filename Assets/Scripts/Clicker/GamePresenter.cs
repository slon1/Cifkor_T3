using Cysharp.Threading.Tasks;
using System;

public class GamePresenter {
	private readonly GameStateModel model;
	private readonly IGameView view;
	private readonly IAsyncTimer autoCollectTimer;
	private readonly IAsyncTimer energyTimer;
	

	public GamePresenter(GameStateModel model, IGameView view) {
		this.model = model;
		this.view = view;
		autoCollectTimer = new AsyncTimer();
		energyTimer = new AsyncTimer();
	}
	public void Start() {
	
		model.Currency.OnChanged += OnCurrencyChanged;
		model.Energy.OnChanged += OnEnergyChanged;

		view.SetCurrency(model.Currency.Value);
		view.SetEnergy(model.Energy.Value);
				
		view.OnButtonClick += OnButtonClicked;
		
		autoCollectTimer.Start(async () => {
			TryCollect();
			await UniTask.CompletedTask;
		}, TimeSpan.FromSeconds(3));

		energyTimer.Start(async () => {
			model.Energy.Add(10);
			await UniTask.CompletedTask;
		}, TimeSpan.FromSeconds(10));
	}


	private void OnCurrencyChanged(int value) {
		view.SetCurrency(value);
	}

	private void OnEnergyChanged(int value) {
		view.SetEnergy(value);
	}

	private void OnButtonClicked() {
		EventBus.Bus.Invoke(EventId.OnSound, "tap");
		TryCollect();
	}

	private void TryCollect() {
		if (model.Energy.TrySpend(1)) {
			model.Currency.Add(1);
			view.PlayCollectEffect(); // анимация FX
		}
		else {
			view.PlayNoEnergyEffect(); // вибрация, мигание и т.д.
		}
	}
	
	public void Stop() {
		//_cts.Cancel();

		autoCollectTimer.Stop();
		energyTimer.Stop();

		view.OnButtonClick -= OnButtonClicked;
		model.Currency.OnChanged -= OnCurrencyChanged;
		model.Energy.OnChanged -= OnEnergyChanged;
		view.Clear();
	
	}
}
