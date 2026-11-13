using System.Windows;
using System.Windows.Controls;
namespace H2LinuxMultiTerminal;

public partial class MainWindow : Window
{
    private readonly List<TerminalPane> panes=[];
    private bool busy;
    public MainWindow(){InitializeComponent(); Loaded += async (_,__)=>{ await Task.Yield(); Build(4); };}
    private void Build_Click(object s,RoutedEventArgs e){ var item=(ComboBoxItem)InstanceCount.SelectedItem; Build(int.Parse(item.Content.ToString()!)); }
    private void Build(int count){ TerminalItems.Items.Clear(); panes.Clear(); for(int i=1;i<=count;i++){var p=new TerminalPane(i);panes.Add(p);TerminalItems.Items.Add(p);} StatusText.Text=$"已创建 {count} 个实例"; }
    private void SelectAll_Click(object s,RoutedEventArgs e)=>panes.ForEach(p=>p.IsSelected=true);
    private void SelectNone_Click(object s,RoutedEventArgs e)=>panes.ForEach(p=>p.IsSelected=false);
    private void Reload_Click(object s,RoutedEventArgs e)=>panes.Where(p=>p.IsSelected).ToList().ForEach(p=>p.Reload());
    private async void Send_Click(object s,RoutedEventArgs e)
    {
        if(busy)return; var selected=panes.Where(p=>p.IsSelected).ToList(); if(selected.Count==0){MessageBox.Show("请至少选择一个实例");return;}
        var lines=CommandBox.Text.Replace("\r","").Split('\n',StringSplitOptions.RemoveEmptyEntries); if(lines.Length==0)return;
        int instanceDelay=ReadDelay(InstanceDelayBox,200), lineDelay=ReadDelay(LineDelayBox,500);
        busy=true; SendButton.IsEnabled=false;
        try{ foreach(var line in lines){ foreach(var p in selected){ StatusText.Text=$"正在发送到 Linux {p.Id}"; await p.SendTextAsync(line); await Task.Delay(instanceDelay); } await Task.Delay(lineDelay); } StatusText.Text="发送完成"; }
        catch(Exception ex){ StatusText.Text="发送失败"; MessageBox.Show(ex.Message,"错误"); }
        finally{busy=false;SendButton.IsEnabled=true;}
    }
    private static int ReadDelay(TextBox box,int fallback)=>int.TryParse(box.Text,out var n)?Math.Clamp(n,0,10000):fallback;
    private async void CtrlC_Click(object s,RoutedEventArgs e)=>await ShortcutAll("c","KeyC",67);
    private async void CtrlL_Click(object s,RoutedEventArgs e)=>await ShortcutAll("l","KeyL",76);
    private async Task ShortcutAll(string key,string code,int vk){ foreach(var p in panes.Where(p=>p.IsSelected)){try{await p.SendShortcutAsync(key,code,vk,true);await Task.Delay(100);}catch(Exception ex){StatusText.Text=ex.Message;}} }
}
