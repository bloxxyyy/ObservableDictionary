namespace Koko;

public enum KeyValueChangedAction { Add, Remove, Clear }

/// <summary>
/// Event Arguments.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class KeyValuePairEventArgs<TKey, TValue> : EventArgs {
	public KeyValuePair<TKey, TValue> KeyValuePair { get; }
	public KeyValueChangedAction Action { get; }

	public KeyValuePairEventArgs(KeyValuePair<TKey, TValue> keyValuePair, KeyValueChangedAction action) {
		KeyValuePair = keyValuePair;
		Action = action;
	}

}

/// <summary>
/// The objective is to have a safe place to alter other dictionaries when altering the base observable dictionary.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ObservableDictionary<TKey, TValue> {
	private Dictionary<TKey, TValue> _PrivateDictionary = new();
	public event EventHandler<KeyValuePairEventArgs<TKey, TValue>> KeyValuePairChanged;

	public bool TryGetValue(TKey key, out TValue value) {
		return _PrivateDictionary.TryGetValue(key, out value);
	}

	public void Add(TKey key, TValue value) {
		if (_PrivateDictionary.ContainsKey(key))
			return;

		_PrivateDictionary.Add(key, value);
		KeyValuePairChanged?.Invoke(this, new KeyValuePairEventArgs<TKey, TValue>(new KeyValuePair<TKey, TValue>(key, value), KeyValueChangedAction.Add));
	}

	public void Remove(TKey key) {
		if (!_PrivateDictionary.TryGetValue(key, out var value))
			return;

		_PrivateDictionary.Remove(key);
		KeyValuePairChanged?.Invoke(this, new KeyValuePairEventArgs<TKey, TValue>(new KeyValuePair<TKey, TValue>(key, value), KeyValueChangedAction.Remove));
	}

	public void Clear() {
		KeyValuePairChanged?.Invoke(this, new KeyValuePairEventArgs<TKey, TValue>(new KeyValuePair<TKey, TValue>(default, default), KeyValueChangedAction.Clear));

		// The invoke goes first so the user of our Clear method has time to react before the entire dict is emptied.
		_PrivateDictionary.Clear();
	}
}
