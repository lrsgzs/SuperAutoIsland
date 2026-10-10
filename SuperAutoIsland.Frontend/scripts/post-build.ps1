# 应用内 JS 编辑器「格式化」用的 prettier（standalone + estree/babel 插件），随 wwwroot 一起发出去。
# 从 node_modules 里拿，保证和前端给 Blockly 生成代码时用的是同一份、同一个版本。
New-Item -ItemType Directory -Force ./dist/prettier | Out-Null
Copy-Item ./node_modules/prettier/standalone.js ./dist/prettier/prettier-standalone.js -Force
Copy-Item ./node_modules/prettier/plugins/estree.js ./dist/prettier/prettier-estree.js -Force
Copy-Item ./node_modules/prettier/plugins/babel.js ./dist/prettier/prettier-babel.js -Force

New-Item -ItemType Directory -Force ../SuperAutoIsland/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/Assets/wwwroot

New-Item -ItemType Directory -Force ../SuperAutoIsland/bin/Debug/net10.0/Assets/wwwroot
Remove-Item -Recurse -Force ../SuperAutoIsland/bin/Debug/net10.0/Assets/wwwroot
Copy-Item -Recurse ./dist ../SuperAutoIsland/bin/Debug/net10.0/Assets/wwwroot
