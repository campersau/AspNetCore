// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace Microsoft.AspNetCore.Components.Rendering;

internal class RendererSynchronizationContextDispatcher : Dispatcher
{
    private readonly RendererSynchronizationContext _context;
    private readonly CultureInfo _cultureInfo;
    private readonly CultureInfo _uiCultureInfo;

    public RendererSynchronizationContextDispatcher()
    {
        _context = new RendererSynchronizationContext();
        _context.UnhandledException += (sender, e) =>
        {
            OnUnhandledException(e);
        };

        _cultureInfo = CultureInfo.CurrentCulture;
        _uiCultureInfo = CultureInfo.CurrentUICulture;
    }

    public override bool CheckAccess() => SynchronizationContext.Current == _context;

    public override Task InvokeAsync(Action workItem)
    {
        CultureInfo.CurrentCulture = _cultureInfo;
        CultureInfo.CurrentUICulture = _uiCultureInfo;

        if (CheckAccess())
        {
            workItem();
            return Task.CompletedTask;
        }

        return _context.InvokeAsync(workItem);
    }

    public override Task InvokeAsync(Func<Task> workItem)
    {
        CultureInfo.CurrentCulture = _cultureInfo;
        CultureInfo.CurrentUICulture = _uiCultureInfo;

        if (CheckAccess())
        {
            return workItem();
        }

        return _context.InvokeAsync(workItem);
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
    {
        CultureInfo.CurrentCulture = _cultureInfo;
        CultureInfo.CurrentUICulture = _uiCultureInfo;

        if (CheckAccess())
        {
            return Task.FromResult(workItem());
        }

        return _context.InvokeAsync<TResult>(workItem);
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
    {
        CultureInfo.CurrentCulture = _cultureInfo;
        CultureInfo.CurrentUICulture = _uiCultureInfo;

        if (CheckAccess())
        {
            return workItem();
        }

        return _context.InvokeAsync<TResult>(workItem);
    }
}
