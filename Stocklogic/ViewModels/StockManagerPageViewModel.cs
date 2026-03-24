using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Models;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class StockManagerPageViewModel : ViewModelBase
{
    // ─── Tab ─────────────────────────────────────────────────────────────────

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTabParts))]
    [NotifyPropertyChangedFor(nameof(IsTabOrders))]
    private string _activeTab = "Parts";

    public bool IsTabParts  => ActiveTab == "Parts";
    public bool IsTabOrders => ActiveTab == "Orders";

    [RelayCommand]
    private void SetTab(string tab)
    {
        ActiveTab = tab;
        if (tab == "Orders" && _allOrders.Count == 0)
            LoadOrders();
    }

    // ─── Parts ───────────────────────────────────────────────────────────────

    private List<Part> _allParts = new();

    public ObservableCollection<Part>     FilteredParts { get; } = new();
    public ObservableCollection<Supplier> Suppliers     { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedPart))]
    [NotifyPropertyChangedFor(nameof(RecommendedSupplier))]
    private Part? _selectedPart;

    public bool      HasSelectedPart    => SelectedPart != null;
    public Supplier? RecommendedSupplier => Suppliers.FirstOrDefault();

    partial void OnSelectedPartChanged(Part? value)
    {
        if (value == null) return;
        EditStock    = value.InStock;
        EditMinStock = value.MinStock;
        SaveMessage  = "";
        LoadSuppliers(value.Id);
    }

    [ObservableProperty] private int    _editStock;
    [ObservableProperty] private int    _editMinStock;
    [ObservableProperty] private string _saveMessage = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFilterAll))]
    [NotifyPropertyChangedFor(nameof(IsFilterLow))]
    [NotifyPropertyChangedFor(nameof(IsFilterOut))]
    private string _activeFilter = "All";

    public bool IsFilterAll => ActiveFilter == "All";
    public bool IsFilterLow => ActiveFilter == "Low";
    public bool IsFilterOut => ActiveFilter == "Out";

    // ─── Orders ──────────────────────────────────────────────────────────────

    private List<CustomerOrder> _allOrders = new();

    public ObservableCollection<CustomerOrder> FilteredOrders { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedOrder))]
    [NotifyPropertyChangedFor(nameof(CanMarkReady))]
    [NotifyPropertyChangedFor(nameof(CanMarkCompleted))]
    private CustomerOrder? _selectedOrder;

    public bool HasSelectedOrder  => SelectedOrder != null;
    public bool CanMarkReady      => SelectedOrder?.Status == OrderStatus.Pending;
    public bool CanMarkCompleted  => SelectedOrder?.Status == OrderStatus.Ready;

    [ObservableProperty] private string _orderSaveMessage = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOrderFilterAll))]
    [NotifyPropertyChangedFor(nameof(IsOrderFilterPending))]
    [NotifyPropertyChangedFor(nameof(IsOrderFilterReady))]
    [NotifyPropertyChangedFor(nameof(IsOrderFilterCompleted))]
    private string _activeOrderFilter = "All";

    public bool IsOrderFilterAll       => ActiveOrderFilter == "All";
    public bool IsOrderFilterPending   => ActiveOrderFilter == "Pending";
    public bool IsOrderFilterReady     => ActiveOrderFilter == "Ready";
    public bool IsOrderFilterCompleted => ActiveOrderFilter == "Completed";

    // ─── Constructor ─────────────────────────────────────────────────────────

    public StockManagerPageViewModel()
    {
        _allParts = DataBaseService.GetAllParts();
        ApplyPartFilter();
    }

    // ─── Part commands ───────────────────────────────────────────────────────

    [RelayCommand]
    private void SetFilter(string filter)
    {
        ActiveFilter = filter;
        ApplyPartFilter();
    }

    [RelayCommand]
    private void SaveStock()
    {
        if (SelectedPart == null) return;
        if (DataBaseService.UpdatePartStock(SelectedPart.Id, EditStock))
        {
            SelectedPart.InStock = EditStock;
            SaveMessage = "Stock updated.";
            ApplyPartFilter();
        }
        else
        {
            SaveMessage = "Error saving stock.";
        }
    }

    [RelayCommand]
    private void SaveMinStock()
    {
        if (SelectedPart == null) return;
        if (DataBaseService.UpdatePartMinStock(SelectedPart.Id, EditMinStock))
        {
            SelectedPart.MinStock = EditMinStock;
            SaveMessage = "Minimum stock updated.";
            ApplyPartFilter();
        }
        else
        {
            SaveMessage = "Error saving minimum stock.";
        }
    }

    // ─── Order commands ──────────────────────────────────────────────────────

    [RelayCommand]
    private void SetOrderFilter(string filter)
    {
        ActiveOrderFilter = filter;
        ApplyOrderFilter();
    }

    [RelayCommand]
    private void MarkReady()
    {
        if (SelectedOrder == null) return;
        if (DataBaseService.UpdateOrderStatus(SelectedOrder.Id, "Ready"))
        {
            SelectedOrder.Status = OrderStatus.Ready;
            OrderSaveMessage = "Order marked as Ready.";
            RefreshSelectedOrder();
        }
        else
        {
            OrderSaveMessage = "Error updating order.";
        }
    }

    [RelayCommand]
    private void MarkCompleted()
    {
        if (SelectedOrder == null) return;
        if (DataBaseService.UpdateOrderStatus(SelectedOrder.Id, "Completed"))
        {
            SelectedOrder.Status = OrderStatus.Completed;
            OrderSaveMessage = "Order marked as Completed.";
            RefreshSelectedOrder();
        }
        else
        {
            OrderSaveMessage = "Error updating order.";
        }
    }

    // ─── Navigation ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void Back() => NavigationService.Navigate(new StartPage());

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private void ApplyPartFilter()
    {
        var filtered = ActiveFilter switch
        {
            "Low" => _allParts.Where(p => p.Status == StockStatus.LowStock),
            "Out" => _allParts.Where(p => p.Status == StockStatus.OutOfStock),
            _     => _allParts.AsEnumerable()
        };
        FilteredParts.Clear();
        foreach (var p in filtered)
            FilteredParts.Add(p);
    }

    private void LoadSuppliers(int partId)
    {
        Suppliers.Clear();
        foreach (var s in DataBaseService.GetSuppliersForPart(partId))
            Suppliers.Add(s);
        OnPropertyChanged(nameof(RecommendedSupplier));
    }

    private void LoadOrders()
    {
        _allOrders = DataBaseService.GetAllOrders();
        ApplyOrderFilter();
    }

    private void ApplyOrderFilter()
    {
        var filtered = ActiveOrderFilter switch
        {
            "Pending"   => _allOrders.Where(o => o.Status == OrderStatus.Pending),
            "Ready"     => _allOrders.Where(o => o.Status == OrderStatus.Ready),
            "Completed" => _allOrders.Where(o => o.Status == OrderStatus.Completed),
            _           => _allOrders.AsEnumerable()
        };
        FilteredOrders.Clear();
        foreach (var o in filtered)
            FilteredOrders.Add(o);
    }

    private void RefreshSelectedOrder()
    {
        OnPropertyChanged(nameof(CanMarkReady));
        OnPropertyChanged(nameof(CanMarkCompleted));
        ApplyOrderFilter();
    }
}
