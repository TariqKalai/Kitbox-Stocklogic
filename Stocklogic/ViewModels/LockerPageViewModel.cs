using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Models;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class LockerPageViewModel : ViewModelBase
{
    public Cabinet Cabinet { get; }

    public ObservableCollection<Locker> Lockers { get; }

    // Listes d'options statiques depuis le service
    public IReadOnlyList<int>    AvailableHeights    { get; } = CabinetComposerService.ValidLockerHeights;
    public IReadOnlyList<string> AvailableColors     { get; } = CabinetComposerService.PanelColors;
    public IReadOnlyList<string> AvailableDoorColors { get; } = CabinetComposerService.DoorColors;
    public IReadOnlyList<string> AngleIronColors     { get; } = CabinetComposerService.AngleIronColors;

    public bool DoorAvailable => CabinetComposerService.ValidWidthsWithDoor.Contains(Cabinet.Width);

    // Champs du formulaire
    [ObservableProperty] private int    _selectedHeight;
    [ObservableProperty] private string _selectedColor     = "White";
    [ObservableProperty] private bool   _hasDoors;
    [ObservableProperty] private string _selectedDoorColor = "White";

    [ObservableProperty]
    private string _selectedAngleIronColor = "White";
    partial void OnSelectedAngleIronColorChanged(string value) => Cabinet.AngleIronColor = value;

    [ObservableProperty] private decimal _totalPrice;
    [ObservableProperty] private string  _statusMessage = string.Empty;

    // État de sélection / mode édition
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEditMode))]
    [NotifyPropertyChangedFor(nameof(IsAddMode))]
    [NotifyPropertyChangedFor(nameof(PanelTitle))]
    [NotifyPropertyChangedFor(nameof(CanAddLocker))]
    private Locker? _selectedLocker;

    public bool   IsEditMode  => SelectedLocker is not null;
    public bool   IsAddMode   => SelectedLocker is null;
    public string PanelTitle  => IsEditMode
        ? $"Edit Locker #{Lockers.IndexOf(SelectedLocker!) + 1}"
        : "Add New Locker";
    public bool   CanAddLocker => Cabinet.CanAddLocker && IsAddMode;

    // Résumé (notifiés depuis UpdateOverview)
    public int TotalItemsCount => Lockers.Count;
    public int TotalHeight     => Lockers.Sum(l => l.Height);
    public int AngleIronHeight => Lockers.Sum(l => l.Height + 4);
    public bool HasLockers     => Lockers.Count > 0;
    public bool IsEmpty        => Lockers.Count == 0;

    public LockerPageViewModel(Cabinet cabinet)
    {
        Cabinet         = cabinet;
        Lockers         = new ObservableCollection<Locker>(cabinet.Lockers);
        _selectedHeight = AvailableHeights[0];
    }

    // Quand on clique un casier → on remplit le formulaire
    partial void OnSelectedLockerChanged(Locker? value)
    {
        AddLockerCommand.NotifyCanExecuteChanged();
        if (value is null) return;
        SelectedHeight    = value.Height;
        SelectedColor     = value.Color;
        HasDoors          = value.HasDoors;
        SelectedDoorColor = value.DoorColor;
    }

    [RelayCommand(CanExecute = nameof(CanAddLockerExec))]
    private void AddLocker()
    {
        var locker = new Locker
        {
            Height    = SelectedHeight,
            Depth     = Cabinet.Depth,
            Width     = Cabinet.Width,
            Color     = SelectedColor,
            HasDoors  = HasDoors && DoorAvailable,
            DoorColor = SelectedDoorColor
        };
        Cabinet.Lockers.Add(locker);
        Lockers.Add(locker);
        OnPropertyChanged(nameof(CanAddLocker));
        AddLockerCommand.NotifyCanExecuteChanged();
        UpdateOverview();
    }

    private bool CanAddLockerExec() => Cabinet.CanAddLocker && IsAddMode;

    [RelayCommand]
    private void UpdateLocker()
    {
        if (SelectedLocker is null) return;

        int idx = Lockers.IndexOf(SelectedLocker);
        SelectedLocker.Height    = SelectedHeight;
        SelectedLocker.Color     = SelectedColor;
        SelectedLocker.HasDoors  = HasDoors && DoorAvailable;
        SelectedLocker.DoorColor = SelectedDoorColor;

        // Remplacement en place pour déclencher CollectionChanged → UI se rafraîchit
        var updated = SelectedLocker;
        SelectedLocker = null;
        Lockers[idx] = updated;

        ResetForm();
        UpdateOverview();
    }

    [RelayCommand]
    private void DeleteSelectedLocker()
    {
        if (SelectedLocker is null) return;
        var toDelete = SelectedLocker;
        SelectedLocker = null;
        Cabinet.Lockers.Remove(toDelete);
        Lockers.Remove(toDelete);
        OnPropertyChanged(nameof(CanAddLocker));
        AddLockerCommand.NotifyCanExecuteChanged();
        UpdateOverview();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        SelectedLocker = null;
        ResetForm();
    }

    private void ResetForm()
    {
        SelectedHeight    = AvailableHeights[0];
        SelectedColor     = "White";
        HasDoors          = false;
        SelectedDoorColor = "White";
    }

    private void UpdateOverview()
    {
        OnPropertyChanged(nameof(TotalItemsCount));
        OnPropertyChanged(nameof(TotalHeight));
        OnPropertyChanged(nameof(AngleIronHeight));
        OnPropertyChanged(nameof(HasLockers));
        OnPropertyChanged(nameof(IsEmpty));
    }

    [RelayCommand]
    private void Back()
    {
        NavigationService.Navigate(new DimensionPage());
    }

    [RelayCommand]
    private void Continue()
    {
        if (Lockers.Count == 0)
        {
            StatusMessage = "Add at least one locker before continuing.";
            return;
        }
        StatusMessage = string.Empty;
        Cabinet.AngleIronColor = SelectedAngleIronColor;
        var summaryVM   = new SummaryPageViewModel(Cabinet.Lockers.ToList(), Cabinet);
        var summaryPage = new SummaryPage { DataContext = summaryVM };
        NavigationService.Navigate(summaryPage);
    }
}
