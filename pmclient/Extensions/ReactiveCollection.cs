using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace pmclient.Extensions;

[Serializable]
public class ReactiveCollection<T> : IDisposable where T : notnull
{
    private readonly SourceList<T> _source = new();
    private readonly CompositeDisposable _disposables = new();
    private readonly ReadOnlyObservableCollection<T> _items;

    public ReadOnlyObservableCollection<T> Items => _items;

    public ReactiveCollection()
    {
        _source
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe()
            .DisposeWith(_disposables);
    }

    public new void Add(T item) => _source.Add(item);

    public void AddAt(T item, int index) => _source.Insert(index, item);

    public void AddRange(IEnumerable<T> items) => _source.AddRange(items);

    public void AddRangeAt(IEnumerable<T> item, int index) => _source.InsertRange(item, index);

    public void Remove(T item) => _source.Remove(item);

    public void RemoveAt(int index) => _source.RemoveAt(index);

    public void RemoveRange(int index, int count) => _source.RemoveRange(index, count);

    public void RemoveMany(IEnumerable<T> items) => _source.RemoveMany(items);

    public void Clear() => _source.Clear();

    public void Replace(IEnumerable<T> items)
    {
        _source.Edit(inner =>
        {
            inner.Clear();
            inner.AddRange(items);
        });
    }

    public void Edit(Action<IExtendedList<T>> editAction) => _source.Edit(editAction);

    public void Dispose() => _disposables.Dispose();
}