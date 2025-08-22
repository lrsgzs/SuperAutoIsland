mkdir -Force ../SuperAutoIsland/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/Assets/wwwroot
