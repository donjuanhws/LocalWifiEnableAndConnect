nssm install WifiMonitorService "C:\Software\WifiMonitor\LocalWifiEnableAndConnect.exe"
nssm set WifiMonitorService AppDirectory "C:\Software\WifiMonitor"
nssm set WifiMonitorService Start SERVICE_AUTO_START
nssm set WifiMonitorService AppRestartDelay 5000
nssm start WifiMonitorService