# Hosting Unity3D Skeleton Project

## 生成过程

- 开启 `/WPFApp.sln`
- 生成 WPFApp 工程
- 找到生成目录（debug 模式下是 `/WPFApp/WPFApp/bin/Debug/net8.0-windows`）
- 添加 Unity 工程（在 `/UnityApp`）
- 生成 Unity 工程到 `/UnityApp_Build`
- 将生成的 Unity 应用所在的文件夹整个移动到 WPF 应用的文件夹下，最终目录形如：

```
net8.0-windows
    |   WPFApp.exe
    |   WPFApp.dll
    |   (other...)
    |
    \---UnityApp_Build
        |    UnityApp.exe
        |    (other...)
        |
        +---UnityApp_Data
        |   (other...)
```

- 运行生成的 `WPFApp.exe`，一切正常的话 WPFApp 的主窗体内会显示 UnityApp 的内容。

## 做了什么？

- 从 WPF 应用启动一个 Unity 应用，Unity 应用的视口作为 WPF 应用中的控件。
- 使用命名管道从 WPF 应用发送消息到 Unity 应用，并立即得到输出，整个过程是阻塞的。

## 已知问题

点击 WPF 应用中的按钮后 Unity 部分不再响应键盘输入，直到拖拽改变 WPF 窗体尺寸。
WPF 界面中的 `reset` 按钮会在点击的稍后清除 WPF 的键盘焦点，这并不能使 Unity 部分重新响应键盘输入。