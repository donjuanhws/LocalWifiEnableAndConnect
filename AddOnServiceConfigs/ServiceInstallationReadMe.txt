cd C:\Software\WifiMonitor\AddOnServiceConfigs\NSSM\win64
nssm install WifiMonitorService "C:\Software\WifiMonitor\LocalWifiEnableAndConnect.exe"
nssm set WifiMonitorService AppDirectory "C:\Software\WifiMonitor"
nssm set WifiMonitorService Start SERVICE_AUTO_START
nssm set WifiMonitorService AppRestartDelay 5000
nssm set WifiMonitorService AppExit Default Restart
nssm set WifiMonitorService AppExit 1 Restart
nssm set WifiMonitorService AppExit 2 Restart
nssm start WifiMonitorService