using System;
using Stylet;

namespace SoundCloudDownloader.ViewModels.Framework;

public abstract class DialogScreen<T> : PropertyChangedBase
{
    public T? DialogResult { get; private set; }

    public event EventHandler? Closed;

    public virtual void Close(T? dialogResult = default)
    {
        DialogResult = dialogResult;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}

public abstract class DialogScreen : DialogScreen<bool?>
{
}