using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyModel {
	public int Value { get; private set; }

	public event Action<int> OnChanged;

	public void Add(int amount) {
		Value += amount;
		OnChanged?.Invoke(Value);
	}
}

