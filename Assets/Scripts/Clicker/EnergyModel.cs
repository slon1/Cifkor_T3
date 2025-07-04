using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyModel {
	public int Value { get; private set; }
	public int MaxValue { get; }

	public event Action<int> OnChanged;

	public EnergyModel(int max) {
		MaxValue = max;
		Value = max;
	}

	public bool TrySpend(int amount) {
		if (Value >= amount) {
			Value -= amount;
			OnChanged?.Invoke(Value);
			return true;
		}
		return false;
	}

	public void Add(int amount) {
		var newValue = Math.Min(Value + amount, MaxValue);
		Value = newValue;
		OnChanged?.Invoke(Value);
	}
}

