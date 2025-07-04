using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel {
	public CurrencyModel Currency { get; }
	public EnergyModel Energy { get; }

	public GameStateModel(int maxEnergy) {
		Currency = new CurrencyModel();
		Energy = new EnergyModel(maxEnergy);
	}
}
