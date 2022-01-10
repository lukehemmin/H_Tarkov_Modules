# Packet Sniffer

References are based on version 0.12.12.1.16266

## Deobfuscation

```cs
// Token: 0x0600D578 RID: 54648 RVA: 0x00127273 File Offset: 0x00125473
Class1085.smethod_0()
{
    return (string)((Hashtable)AppDomain.CurrentDomain.GetData(Class2085.string_0))[int_0];
}
```

```bash
de4dot-x64.exe --un-name "!^<>[a-z0-9]$&!^<>[a-z0-9]__.*$&![A-Z][A-Z]\$<>.*$&^[a-zA-Z_<{$][a-zA-Z_0-9<>{}$.`-]*$" "Assembly-CSharp-cleaned.dll" --strtyp delegate --strtok 0x0600D578
pause
```

## Assembly-CSharp.dll

### Save requests

```cs
// Token: 0x06001EB2 RID: 7858 RVA: 0x001A58EC File Offset: 0x001A3AEC
[postfix]
Class180.method_2()
{
    var uri = new Uri(url);
    var path = (System.IO.Directory.GetCurrentDirectory() + "\\HTTP_DATA\\").Replace("\\\\", "\\");
    var file = uri.LocalPath.Replace('/', '.').Remove(0, 1);
    var time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    if (System.IO.Directory.CreateDirectory(path).Exists && obj != null)
    {
        System.IO.File.WriteAllText($@"{path}req.{file}_{time}.json", text);
    }
}
```

### Save responses

```cs
// Token: 0x06001EBE RID: 7870 RVA: 0x001A5FD8 File Offset: 0x001A41D8
[postfix]
Class180.method_12()
{
    // add this at the end, before "return text3;"
    var uri = new Uri(url);
    var path = (System.IO.Directory.GetCurrentDirectory() + "\\HTTP_DATA\\").Replace("\\\\", "\\");
    var file = uri.LocalPath.Replace('/', '.').Remove(0, 1);
    var time = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    if (System.IO.Directory.CreateDirectory(path).Exists)
    {
        // in case you turn this into a patch, text3 = __result
        System.IO.File.WriteAllText($@"{path}resp.{file}_{time}.json", text3);
    }
}
```

### Disable SSL certification

```cs
// Token: 0x06005039 RID: 20537 RVA: 0x0027D4AC File Offset: 0x0027B6AC
[prefix]
Class519.ValidateCertificate()
{
    return true;
}
```

### Battleye

```cs
// Token: 0x06006ADF RID: 27359 RVA: 0x002D3AA8 File Offset: 0x002D1CA8
[prefix]
Class803.RunValidation()
{
    this.Succeed = true;
}
```

## FilesChecker.dll

### Consistency multi

```cs
// Token: 0x06000054 RID: 84 RVA: 0x00002A38 File Offset: 0x00000C38
// target with return type: Task<ICheckResult>
[prefix]
ConsistencyController.EnsureConsistency()
{
    return Task.FromResult<ICheckResult>(ConsistencyController.CheckResult.Succeed(new TimeSpan()));
}
```

### Consistency single

```cs
// Token: 0x06000053 RID: 83 RVA: 0x000028D4 File Offset: 0x00000AD4
[prefix]
ConsistencyController.EnsureConsistencySingle()
{
    return Task.FromResult<ICheckResult>(ConsistencyController.CheckResult.Succeed(new TimeSpan()));
}
```
