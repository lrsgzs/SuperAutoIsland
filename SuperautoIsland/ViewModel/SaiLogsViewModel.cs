using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.ViewModel;

public partial class SaiLogsViewModel : ObservableRecipient
{
    private ReadOnlyObservableCollection<LogData> _logs = null!;
    public ReadOnlyObservableCollection<LogData> Logs => _logs;
    private IDisposable? _prevSubscription;
    public RootLogger RootLogger = Logger.Root;
    
    [ObservableProperty] private bool _isFilteredError = true;
    [ObservableProperty] private bool _isFilteredWarning = true;
    [ObservableProperty] private bool _isFilteredInfo = true;
    [ObservableProperty] private bool _isFilteredDebug = false;
    [ObservableProperty] private bool _isFilteredOthers = false;
    [ObservableProperty] private string _filterText = "";

    public SaiLogsViewModel()
    {
        RefreshSource();

        this.ObservableForProperty(x => x.IsFilteredError).Subscribe(_ => RefreshSource());
        this.ObservableForProperty(x => x.IsFilteredWarning).Subscribe(_ => RefreshSource());
        this.ObservableForProperty(x => x.IsFilteredInfo).Subscribe(_ => RefreshSource());
        this.ObservableForProperty(x => x.IsFilteredDebug).Subscribe(_ => RefreshSource());
        this.ObservableForProperty(x => x.IsFilteredOthers).Subscribe(_ => RefreshSource());
        this.ObservableForProperty(x => x.FilterText).Subscribe(_ => RefreshSource());
    }

    private void RefreshSource()
    {
        _prevSubscription?.Dispose();
        var observable = RootLogger.LogDataList
            .Connect()
            .Filter(x =>
            {
                var isFiltered = x.Level switch
                {
                    "ERROR" => IsFilteredError,
                    "WARN" => IsFilteredWarning,
                    "DEBUG" => IsFilteredDebug,
                    "INFO" => IsFilteredInfo,
                    _ => IsFilteredOthers
                };

                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    return isFiltered;
                }
                return isFiltered && (x.Message.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                                      x.Scope.Contains(FilterText));
            })
            .Bind(out _logs);
        OnPropertyChanged(nameof(Logs));
        _prevSubscription = observable.Subscribe();
    }
}