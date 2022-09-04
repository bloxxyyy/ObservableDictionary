using System.Collections;

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
public class KeyValuePairEventArgs<TKey, TValue, TParent> : EventArgs {
	public TParent Parent { get; }
	public KeyValuePair<TKey, TValue> KeyValuePair { get; }
	public KeyValueChangedAction Action { get; }

	public KeyValuePairEventArgs(KeyValuePair<TKey, TValue> keyValuePair, KeyValueChangedAction action, TParent parent) {
		KeyValuePair = keyValuePair;
		Action = action;
		Parent = parent;
	}

}

/// <summary>
/// The objective is to have a safe place to alter other dictionaries when altering the base observable dictionary.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ObservableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
	private Dictionary<TKey, TValue> _PrivateDictionary = new();
	public event EventHandler<KeyValuePairEventArgs<TKey, TValue>> KeyValuePairChanged;

	public bool TryGetValue(TKey key, out TValue value) {
		return _PrivateDictionary.TryGetValue(key, out value);
	}

	public bool ContainsKey(TKey key) {
		return _PrivateDictionary.ContainsKey(key);
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

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		return _PrivateDictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return _PrivateDictionary.GetEnumerator();
	}
}
public class ObservableDictionary<TKey, TValue, TParent> : IEnumerable<KeyValuePair<TKey, TValue>> {
	private Dictionary<TKey, TValue> _PrivateDictionary = new();
	public event EventHandler<KeyValuePairEventArgs<TKey, TValue, TParent>> KeyValuePairChanged;
	private TParent _Parent;

	public bool TryGetValue(TKey key, out TValue value) {
		return _PrivateDictionary.TryGetValue(key, out value);
	}

	public bool ContainsKey(TKey key) {
		return _PrivateDictionary.ContainsKey(key);
	}

	public void Add(TKey key, TValue value, TParent parent) {
		if (_PrivateDictionary.ContainsKey(key))
			return;

		_Parent = parent;

		_PrivateDictionary.Add(key, value);
		KeyValuePairChanged?.Invoke(this, new KeyValuePairEventArgs<TKey, TValue, TParent>(new KeyValuePair<TKey, TValue>(key, value), KeyValueChangedAction.Add, _Parent));
	}

	public void Remove(TKey key) {
		if (!_PrivateDictionary.TryGetValue(key, out var value))
			return;

		_PrivateDictionary.Remove(key);
		KeyValuePairChanged?.Invoke(this, new KeyValuePairEventArgs<TKey, TValue, TParent>(new KeyValuePair<TKey, TValue>(key, value), KeyValueChangedAction.Remove, _Parent));
	}

	public void Clear() {
		KeyValuePairChanged?.Invoke(this, new KeyValuePairEventArgs<TKey, TValue, TParent>(new KeyValuePair<TKey, TValue>(default, default), KeyValueChangedAction.Clear, _Parent));

		// The invoke goes first so the user of our Clear method has time to react before the entire dict is emptied.
		_PrivateDictionary.Clear();
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
		return _PrivateDictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return _PrivateDictionary.GetEnumerator();
	}
}