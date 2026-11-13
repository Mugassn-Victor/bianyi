# 使用 GitHub 自动编译 EXE

1. 在 GitHub 新建一个空仓库。
2. 将本目录中的所有文件上传到仓库根目录，包括隐藏目录 `.github`。
3. 打开仓库的 **Actions** 标签。
4. 选择 **Build Windows EXE**。
5. 点击 **Run workflow**。
6. 构建完成后，在页面底部 **Artifacts** 下载 `H2LinuxMultiTerminal-win-x64`。
7. 解压后得到 `H2LinuxMultiTerminal.exe`。

也可以直接把文件推送到 `main` 或 `master` 分支，工作流会自动运行。
