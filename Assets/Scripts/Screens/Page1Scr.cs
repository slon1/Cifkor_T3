using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Page1Scr : ScrAbs
{
	private GamePresenter presenter;
	private bool started;
	[Inject]
	private void Construct(GamePresenter presenter) {
		this.presenter = presenter;

	}
	public override void Show() {
		if (!started) {
			started = true;
			base.Show();
			presenter.Start();
		}
	}
	public override void Hide() {
		base.Hide();
		presenter.Stop();
		started = false;
	}
}
