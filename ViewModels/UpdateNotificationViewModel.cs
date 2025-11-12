using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Allva.Desktop.Services;
using Velopack;

namespace Allva.Desktop.ViewModels
{
    public partial class UpdateNotificationViewModel : ObservableObject
    {
        private readonly UpdateInfo _updateInfo;
        private readonly UpdateService _updateService;

        [ObservableProperty]
        private string _versionActual = "1.2.0";

        [ObservableProperty]
        private string _versionNueva = "1.2.1";

        [ObservableProperty]
        private bool _descargando = false;

        [ObservableProperty]
        private int _progreso = 0;

        [ObservableProperty]
        private string _mensajeEstado = "Actualización disponible";

        public UpdateNotificationViewModel(UpdateInfo updateInfo, UpdateService updateService)
        {
            _updateInfo = updateInfo;
            _updateService = updateService;
            VersionActual = _updateService.CurrentVersion;
            VersionNueva = _updateInfo.TargetFullRelease.Version.ToString();
        }

        [RelayCommand]
        private async Task ActualizarAhora()
        {
            Descargando = true;
            MensajeEstado = "Descargando actualización...";

            try
            {
                await _updateService.DownloadUpdatesAsync(_updateInfo, progress =>
                {
                    Progreso = progress;
                });

                MensajeEstado = "✅ Descarga completada. La actualización se aplicará al cerrar la aplicación.";
                await Task.Delay(2000);
                
                CerrarDialogo?.Invoke();
            }
            catch (Exception ex)
            {
                MensajeEstado = $"❌ Error: {ex.Message}";
                Descargando = false;
            }
        }

        [RelayCommand]
        private void ActualizarDespues()
        {
            CerrarDialogo?.Invoke();
        }

        public Action? CerrarDialogo { get; set; }
    }
}