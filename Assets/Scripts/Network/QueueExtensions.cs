using System;
using System.Collections.Generic;
/// <summary>
/// Расширение для удаления элемента из очереди по условию
/// </summary>
public static class QueueExtensions {
	public static void RemoveWhere<T>(this Queue<T> queue, Predicate<T> predicate) {
		var items = queue.ToArray();
		queue.Clear();
		foreach (var item in items) {
			if (!predicate(item))
				queue.Enqueue(item);
		}
	}
}
