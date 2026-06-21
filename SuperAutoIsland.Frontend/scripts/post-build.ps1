New-Item -ItemType Directory -Force ../SuperAutoIsland/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/Assets/wwwroot

New-Item -ItemType Directory -Force ../SuperAutoIsland/bin/Debug/net8.0-windows/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/bin/Debug/net8.0-windows/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/bin/Debug/net8.0-windows/Assets/wwwroot
