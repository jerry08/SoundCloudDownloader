﻿using System;

namespace SoundCloudDownloader.Core.Utils.Extensions;

public static class GenericExtensions
{
    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform) =>
        transform(input);
}
