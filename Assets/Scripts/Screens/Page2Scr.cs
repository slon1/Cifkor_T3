using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Page2Scr : ScrAbs {
	private const string ServerUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
	[SerializeField]
	private Text text;
	private IHttpGetJsonService jsonService;
	private AsyncTimer timer;

	[Inject]
	private void Construct(IHttpGetJsonService jsonService) {
		this.jsonService = jsonService;

	}
	public override void Init() {
		base.Init();
		timer = new AsyncTimer();
	}
	public override void Show() {
		base.Show();
		timer.Start(UpdateWeatherAsync, TimeSpan.FromSeconds(5));

	}


	public override void Hide() {
		base.Hide();
		timer?.Stop();
		jsonService.CancelRequest(ServerUrl);

	}

	private async UniTask UpdateWeatherAsync() {
		var result = await jsonService.GetJsonAsync<WeatherRoot>(
			ServerUrl,
			() => RaiseEvent(ButtonId.ShowTimer),
			() => RaiseEvent(ButtonId.HideTimer)
		);
		text.text = GetCurrentForecast(result.Value.properties.periods).temperature + "F";
	}

	private Period GetCurrentForecast(List<Period> periods) {
		var now = DateTime.UtcNow;
		return periods.FirstOrDefault(p => now >= p.startTime.ToUniversalTime() && now <= p.endTime.ToUniversalTime());
	}
	public override void Execute(PageActionId action) {
		base.Execute(action);
	}
	private void OnDestroy() {
		timer.Stop();
		timer = null;
	}
}
