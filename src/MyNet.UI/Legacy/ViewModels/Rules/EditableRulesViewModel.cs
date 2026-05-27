// -----------------------------------------------------------------------
// <copyright file="EditableRulesViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using MyNet.UI.Commands;
using MyNet.UI.Resources;

namespace MyNet.UI.Legacy.ViewModels.Rules;

public class EditableRulesViewModel<T> : EditableObject
    where T : IEditableRule
{
    public EditableRulesViewModel(bool addValidationRuleForBlockEmptyRules = false)
        : this([], addValidationRuleForBlockEmptyRules) { }

    public EditableRulesViewModel(IEnumerable<IAvailableRule<T>> availableRules, bool addValidationRuleForBlockEmptyRules = false)
    {
        AvailableRules.AddRange(availableRules);

        AddCommand = RelayCommandFactory.Default.CreateRequired<IAvailableRule<T>>(AddRule, x => x.IsEnabled);
        RemoveCommand = RelayCommandFactory.Default.CreateRequired<T>(RemoveDateRule, x => x.CanRemove);
        MoveUpCommand = RelayCommandFactory.Default.CreateRequired<T>(MoveUp, CanMoveUp);
        MoveDownCommand = RelayCommandFactory.Default.CreateRequired<T>(MoveDown, CanMoveDown);

        if (addValidationRuleForBlockEmptyRules)
            ValidationRules.Add<EditableRulesViewModel<T>, UiObservableCollection<T>>(x => x.Rules, ValidationResources.CollectionRequiresAtLeastOneItem.FormatWith(UiResources.Rules), x => x?.Count > 0);
    }

    [CanSetIsModified(false)]
    [CanBeValidated(false)]
    public UiObservableCollection<IAvailableRule<T>> AvailableRules { get; } = [];

    [Display(Name = nameof(Rules), ResourceType = typeof(UiResources))]
    public UiObservableCollection<T> Rules { get; } = [];

    public ICommand AddCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand MoveDownCommand { get; }

    public ICommand MoveUpCommand { get; }

    protected virtual void AddRule(IAvailableRule<T> rule) => Rules.Add(rule.Create());

    protected virtual void RemoveDateRule(T rule) => Rules.Remove(rule);

    protected virtual void MoveUp(T rule)
    {
        var index = Rules.IndexOf(rule);

        if (index > 0)
            Rules.Swap(index, index - 1);
    }

    protected virtual bool CanMoveUp(T rule) => rule.CanMove && Rules.IndexOf(rule) > 0;

    protected virtual void MoveDown(T rule)
    {
        var index = Rules.IndexOf(rule);

        if (index > -1 && index < Rules.Count - 1)
            Rules.Swap(index, index + 1);
    }

    protected virtual bool CanMoveDown(T rule) => rule.CanMove && Rules.IndexOf(rule) < Rules.Count - 1;
}
