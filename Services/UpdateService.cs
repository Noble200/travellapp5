using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace Allva.Desktop.Services
{
    public class UpdateService
    {
        private readonly UpdateManager? _updateManager;
        private readonly bool _isUpdateAvailable;

        public UpdateService()
        {
            try
            {
                _updateManager = new UpdateManager(
                    new GithubSource(
                        "https://github.com/Allva-soft/AllvaSystem",
                        null, // Token si fuera necesario para repo privado
                        false // false = solo releases estables
                    )
                );
                _isUpdateAvailable = true;
            }
            catch
            {
                _updateManager = null;
                _isUpdateAvailable = false;
            }
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            if (!_isUpdateAvailable || _updateManager == null)
                return null;

            try
            {
                var updateInfo = await _updateManager.CheckForUpdatesAsync();
                return updateInfo;
            }
            catch
            {
                return null;
            }
        }

        public async Task DownloadUpdatesAsync(UpdateInfo updateInfo, Action<int>? progressCallback = null)
        {
            if (_updateManager == null || updateInfo == null)
                return;

            await _updateManager.DownloadUpdatesAsync(updateInfo, progressCallback);
        }

        public void ApplyUpdatesAndRestart(UpdateInfo updateInfo)
        {
            if (_updateManager == null || updateInfo == null)
                return;

            _updateManager.ApplyUpdatesAndRestart(updateInfo);
        }

        public string CurrentVersion => _updateManager?.CurrentVersion?.ToString() ?? "1.0.0";

        public bool IsUpdateSystemAvailable => _isUpdateAvailable;
    }
}