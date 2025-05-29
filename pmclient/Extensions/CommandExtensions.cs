using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace pmclient.Extensions;

public static class CommandExtensions
{
    public static TViewModel SetCommand<TViewModel>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, ICommand>> commandSelector,
        ICommand command)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(commandSelector);

        if (commandSelector.Body is MemberExpression { Member: PropertyInfo { CanWrite: true } propInfo })
            propInfo.SetValue(viewModel, command);
        else throw new ArgumentException("Expression must be a writable property.");

        return viewModel;
    }
}