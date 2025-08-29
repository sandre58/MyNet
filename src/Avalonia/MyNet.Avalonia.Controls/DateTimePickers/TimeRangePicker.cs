﻿// -----------------------------------------------------------------------
// <copyright file="TimeRangePicker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using MyNet.Avalonia.Controls.Primitives;
using MyNet.Avalonia.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Avalonia.Controls;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[TemplatePart(PartStartTextBox, typeof(TextBox))]
[TemplatePart(PartEndTextBox, typeof(TextBox))]
[TemplatePart(PartPopup, typeof(Popup))]
[TemplatePart(PartStartPresenter, typeof(TimePickerPresenter))]
[TemplatePart(PartEndPresenter, typeof(TimePickerPresenter))]
[TemplatePart(PartButton, typeof(Button))]
[PseudoClasses(PseudoClassName.Empty)]
public class TimeRangePicker : TimePickerBase
{
    public const string PartStartTextBox = "PART_StartTextBox";
    public const string PartEndTextBox = "PART_EndTextBox";
    public const string PartStartPresenter = "PART_StartPresenter";
    public const string PartEndPresenter = "PART_EndPresenter";
    public const string PartButton = "PART_Button";
    public const string PartPopup = "PART_Popup";

    public static readonly StyledProperty<TimeSpan?> StartTimeProperty =
        AvaloniaProperty.Register<TimeRangePicker, TimeSpan?>(
            nameof(StartTime), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<TimeSpan?> EndTimeProperty =
        AvaloniaProperty.Register<TimeRangePicker, TimeSpan?>(
            nameof(EndTime), defaultBindingMode: BindingMode.TwoWay);

    private Button? _button;
    private TimePickerPresenter? _endPresenter;
    private TextBox? _endTextBox;
    private TimePickerPresenter? _startPresenter;

    private bool _isFocused;
    private Popup? _popup;

    private TextBox? _startTextBox;
    private bool _suppressTextPresenterEvent;

    static TimeRangePicker()
    {
        FocusableProperty.OverrideDefaultValue<TimeRangePicker>(true);
        _ = StartTimeProperty.Changed.AddClassHandler<TimeRangePicker, TimeSpan?>((picker, args) =>
            picker.OnSelectionChanged(args));
        _ = EndTimeProperty.Changed.AddClassHandler<TimeRangePicker, TimeSpan?>((picker, args) =>
            picker.OnSelectionChanged(args, false));
        _ = DisplayFormatProperty.Changed.AddClassHandler<TimeRangePicker, string?>((picker, _) =>
            picker.OnDisplayFormatChanged());
    }

    #region SeparatorText

    /// <summary>
    /// Provides SeparatorText Property.
    /// </summary>
    public static readonly StyledProperty<string?> SeparatorTextProperty = AvaloniaProperty.Register<TimeRangePicker, string?>(nameof(SeparatorText), "~");

    /// <summary>
    /// Gets or sets the SeparatorText property.
    /// </summary>
    public string? SeparatorText
    {
        get => GetValue(SeparatorTextProperty);
        set => SetValue(SeparatorTextProperty, value);
    }

    #endregion

    public TimeSpan? StartTime
    {
        get => GetValue(StartTimeProperty);
        set => SetValue(StartTimeProperty, value);
    }

    public TimeSpan? EndTime
    {
        get => GetValue(EndTimeProperty);
        set => SetValue(EndTimeProperty, value);
    }

    public void Clear()
    {
        _ = Focus(NavigationMethod.Pointer);
        SetCurrentValue(StartTimeProperty, null);
        SetCurrentValue(EndTimeProperty, null);
        _startPresenter?.SyncTime(null);
        _endPresenter?.SyncTime(null);
    }

    private void OnDisplayFormatChanged()
    {
        if (_startTextBox is not null) SyncTimeToText(StartTime);
        if (_endTextBox is not null) SyncTimeToText(EndTime, false);
    }

    private void OnSelectionChanged(AvaloniaPropertyChangedEventArgs<TimeSpan?> args, bool start = true)
    {
        SyncTimeToText(args.NewValue.Value, start);
        _suppressTextPresenterEvent = true;
        var presenter = start ? _startPresenter : _endPresenter;
        presenter?.SyncTime(args.NewValue.Value);
        _suppressTextPresenterEvent = false;
    }

    private void SyncTimeToText(TimeSpan? time, bool start = true)
    {
        var textBox = start ? _startTextBox : _endTextBox;
        if (textBox is null) return;
        if (time is null)
        {
            textBox.Text = null;
            return;
        }

        var date = new DateTime(1, 1, 1, time.Value.Hours, time.Value.Minutes, time.Value.Seconds);
        var text = date.ToString(DisplayFormat, CultureInfo.CurrentCulture);
        textBox.Text = text;
        PseudoClasses.Set(PseudoClassName.Empty, StartTime is null && EndTime is null);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        GotFocusEvent.RemoveHandler(OnTextBoxGetFocus, _startTextBox, _endTextBox);
        Button.ClickEvent.RemoveHandler(OnButtonClick, _button);
        TimePickerPresenter.SelectedTimeChangedEvent.RemoveHandler(OnPresenterTimeChanged, _startPresenter, _endPresenter);
        TextBox.TextChangedEvent.RemoveHandler(OnTextChanged, _startTextBox, _endTextBox);

        _popup = e.NameScope.Find<Popup>(PartPopup);
        _startTextBox = e.NameScope.Find<TextBox>(PartStartTextBox);
        _endTextBox = e.NameScope.Find<TextBox>(PartEndTextBox);
        _startPresenter = e.NameScope.Find<TimePickerPresenter>(PartStartPresenter);
        _endPresenter = e.NameScope.Find<TimePickerPresenter>(PartEndPresenter);
        _button = e.NameScope.Find<Button>(PartButton);

        GotFocusEvent.AddHandler(OnTextBoxGetFocus, _startTextBox, _endTextBox);
        Button.ClickEvent.AddHandler(OnButtonClick, _button);
        TimePickerPresenter.SelectedTimeChangedEvent.AddHandler(OnPresenterTimeChanged, _startPresenter, _endPresenter);
        TextBox.TextChangedEvent.AddHandler(OnTextChanged, _startTextBox, _endTextBox);

        _startPresenter?.SyncTime(StartTime);
        _endPresenter?.SyncTime(EndTime);
        SyncTimeToText(StartTime);
        SyncTimeToText(EndTime, false);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (Equals(sender, _startTextBox))
            OnTextChangedInternal(_startTextBox, _startPresenter, StartTimeProperty, true);
        else if (Equals(sender, _endTextBox)) OnTextChangedInternal(_endTextBox, _endPresenter, EndTimeProperty, true);
    }

    private void OnTextChangedInternal(TextBox? textBox, TimePickerPresenter? presenter, AvaloniaProperty property, bool fromText = false)
    {
        if (textBox?.Text is null || string.IsNullOrEmpty(textBox.Text))
        {
            SetCurrentValue(property, null);
            presenter?.SyncTime(null);
        }
        else if (string.IsNullOrEmpty(DisplayFormat))
        {
            if (DateTime.TryParse(textBox.Text, out var defaultTime))
            {
                SetCurrentValue(property, defaultTime.TimeOfDay);
                presenter?.SyncTime(defaultTime.TimeOfDay);
            }
        }
        else
        {
            if (DateTime.TryParseExact(textBox.Text, DisplayFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
            {
                SetCurrentValue(property, date.TimeOfDay);
                presenter?.SyncTime(date.TimeOfDay);
            }
            else
            {
                if (!fromText)
                {
                    SetCurrentValue(property, null);
                    _ = textBox.SetValue(TextBox.TextProperty, null);
                    presenter?.SyncTime(null);
                }
            }
        }
    }

    private void OnPresenterTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (!IsInitialized) return;
        if (_suppressTextPresenterEvent) return;
        SetCurrentValue(Equals(sender, _startPresenter) ? StartTimeProperty : EndTimeProperty, e.NewTime);
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (IsFocused)
        {
            SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
        }
    }

    private void OnTextBoxGetFocus(object? sender, GotFocusEventArgs e) => SetCurrentValue(IsDropDownOpenProperty, true);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Down)
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Tab)
        {
            if (Equals(e.Source, _endTextBox)) SetCurrentValue(IsDropDownOpenProperty, false);
            return;
        }

        if (e.Key == Key.Enter)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            CommitInput(true);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    public void Confirm()
    {
        _startPresenter?.Confirm();
        _endPresenter?.Confirm();
        SetCurrentValue(IsDropDownOpenProperty, false);
        _ = Focus();
    }

    public void Dismiss()
    {
        SetCurrentValue(IsDropDownOpenProperty, false);
        _ = Focus();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        FocusChanged(IsKeyboardFocusWithin);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        FocusChanged(IsKeyboardFocusWithin);
        var top = TopLevel.GetTopLevel(this);
        var element = top?.FocusManager?.GetFocusedElement();
        if (element is Visual v && _popup?.IsInsidePopup(v) == true) return;
        if (element == _startTextBox || element == _endTextBox) return;
        CommitInput(true);
        SetCurrentValue(IsDropDownOpenProperty, false);
    }

    private void FocusChanged(bool hasFocus)
    {
        var wasFocused = _isFocused;
        _isFocused = hasFocus;
        if (hasFocus)
        {
            if (!wasFocused && _startTextBox != null)
                _ = _startTextBox.Focus();
        }
    }

    private void CommitInput(bool clearWhenInvalid)
    {
        if (DateTime.TryParseExact(_startTextBox?.Text, DisplayFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out var start))
        {
            _startPresenter?.SyncTime(start.TimeOfDay);
            SetCurrentValue(StartTimeProperty, start.TimeOfDay);
        }
        else
        {
            if (clearWhenInvalid)
            {
                _ = _startTextBox?.SetValue(TextBox.TextProperty, null);
                _startPresenter?.SyncTime(null);
            }
        }

        if (DateTime.TryParseExact(_endTextBox?.Text, DisplayFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out var end))
        {
            _endPresenter?.SyncTime(end.TimeOfDay);
            SetCurrentValue(EndTimeProperty, end.TimeOfDay);
        }
        else
        {
            if (clearWhenInvalid)
            {
                _ = _endTextBox?.SetValue(TextBox.TextProperty, null);
                _endPresenter?.SyncTime(null);
            }
        }
    }
}
