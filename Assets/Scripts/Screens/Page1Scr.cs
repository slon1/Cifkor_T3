using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Page1Scr : ScrAbs
{
	private GamePresenter presenter;

	[Inject]
	private void Construct(GamePresenter presenter) {
		this.presenter = presenter;

	}
	public override void Show() {
		base.Show();
		presenter.Start();
	}
	public override void Hide() {
		base.Hide();
		presenter.Stop();
	}
}
