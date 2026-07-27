New-Item -ItemType Directory -Force ../SuperAutoIsland/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/Assets/wwwroot

New-Item -ItemType Directory -Force ../SuperAutoIsland/bin/Debug/net10.0/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/bin/Debug/net10.0/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/bin/Debug/net10.0/Assets/wwwroot
