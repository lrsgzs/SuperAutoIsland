# 日常构建：在 SuperAutoIsland/cipx/ 下生成插件包（SuperAutoIsland.cipx）与校验文件。
# 只负责构建，不包含任何发版/上传逻辑。
Write-Host '开始构建 SuperAutoIsland'

dotnet publish -p:CreateCipx=true -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "构建失败，dotnet 退出码：$LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host '构建完成'
