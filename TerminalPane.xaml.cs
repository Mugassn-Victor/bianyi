using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace H2LinuxMultiTerminal;

public partial class TerminalPane : UserControl
{
    public int Id { get; }
    public bool IsSelected { get => SelectedBox.IsChecked == true; set => SelectedBox.IsChecked = value; }
    private double? relativeX, relativeY;
    private bool armed;
    private string ConfigPath => Path.Combine(AppFolder, $"target-{Id}.json");
    private static string AppFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "H2LinuxMultiTerminal");

    public TerminalPane(int id)
    {
        InitializeComponent(); Id=id; TitleText.Text=$"Linux {id}";
        Loaded += async (_,__) => await InitializeAsync();
        Browser.PreviewMouseLeftButtonDown += Browser_PreviewMouseLeftButtonDown;
        LoadTarget();
    }

    private async Task InitializeAsync()
    {
        Directory.CreateDirectory(AppFolder);
        var profile=Path.Combine(AppFolder,"Profiles",$"linux-{Id}");
        var env=await CoreWebView2Environment.CreateAsync(userDataFolder:profile);
        await Browser.EnsureCoreWebView2Async(env);
        Browser.CoreWebView2.Settings.AreDevToolsEnabled=true;
        Browser.CoreWebView2.Navigate("https://cli.h2.nexus");
    }

    private void ArmTarget_Click(object sender, RoutedEventArgs e)
    { armed=true; TargetText.Text="现在点击终端输入区域"; }

    private void Browser_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(!armed || Browser.ActualWidth<=0 || Browser.ActualHeight<=0) return;
        var p=e.GetPosition(Browser);
        relativeX=Math.Clamp(p.X/Browser.ActualWidth,0,1);
        relativeY=Math.Clamp(p.Y/Browser.ActualHeight,0,1);
        File.WriteAllText(ConfigPath,JsonSerializer.Serialize(new Target(relativeX.Value,relativeY.Value)));
        armed=false; TargetText.Text="已定位";
    }

    private void LoadTarget()
    {
        try { if(!File.Exists(ConfigPath)) return; var t=JsonSerializer.Deserialize<Target>(File.ReadAllText(ConfigPath)); if(t is null)return; relativeX=t.X; relativeY=t.Y; TargetText.Text="已定位"; } catch { }
    }

    public async Task SendTextAsync(string text)
    {
        if(Browser.CoreWebView2 is null) return;
        await FocusTerminalAsync();
        await Browser.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.insertText",JsonSerializer.Serialize(new { text }));
        await SendKeyAsync("Enter","Enter",13);
    }

    public async Task SendShortcutAsync(string key,string code,int vk,bool ctrl)
    {
        if(Browser.CoreWebView2 is null) return;
        await FocusTerminalAsync();
        await SendKeyAsync(key,code,vk,ctrl ? 2 : 0);
    }

    private async Task FocusTerminalAsync()
    {
        if(relativeX is null || relativeY is null) throw new InvalidOperationException($"Linux {Id} 尚未定位终端");
        double x=relativeX.Value*Browser.ActualWidth, y=relativeY.Value*Browser.ActualHeight;
        foreach(var type in new[]{"mousePressed","mouseReleased"})
            await Browser.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchMouseEvent",JsonSerializer.Serialize(new { type,x,y,button="left",clickCount=1 }));
        await Task.Delay(100);
    }

    private async Task SendKeyAsync(string key,string code,int vk,int modifiers=0)
    {
        foreach(var type in new[]{"keyDown","keyUp"})
            await Browser.CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchKeyEvent",JsonSerializer.Serialize(new { type,key,code,windowsVirtualKeyCode=vk,nativeVirtualKeyCode=vk,modifiers }));
    }

    public void Reload()=>Browser.CoreWebView2?.Reload();
    private void Reload_Click(object sender,RoutedEventArgs e)=>Reload();
    private sealed record Target(double X,double Y);
}
