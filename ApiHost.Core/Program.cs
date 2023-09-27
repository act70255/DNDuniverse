using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Hosting;
using ApiHost.Core.Setup.Modules;
using MongoDB.Driver;
using Serilog;
using Autofac.Integration.WebApi;
using Serilog.Events;
using ApiHost.Core.MiddleWare;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ApiHost.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new ServiceModule());
                builder.RegisterModule(new DND.Model.AutoMapperModule());
                builder.RegisterModule(new DND.Repository.RepositoryModule());
                builder.RegisterModule(new DND.Domain.ServiceModule());
            });

            #region MongoDB
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var mongoDbSettings = builder.Configuration.GetSection("MongoDB");
            var mongoDbConnectionString = mongoDbSettings["ConnectionString"];
            var mongoDbDatabaseName = mongoDbSettings["DatabaseName"];
            builder.Services.AddSingleton<IMongoDatabase>(s => new MongoClient(mongoDbConnectionString).GetDatabase(mongoDbDatabaseName));
            #endregion

            //regist middleware
            builder.Services.AddTransient<ExceptionHandleMiddleware>();
            builder.Services.AddTransient<ResultMiddleware>();

            #region Log
            //builder.Services.AddLogging(builder => builder
            //    .AddSerilog(new LoggerConfiguration()
            //                .MinimumLevel.Debug()
            //                //.Filter.ByIncludingOnly(f => f.Level == LogEventLevel.Debug || f.Level == LogEventLevel.Error)
            //                .WriteTo.Console()
            //                .WriteTo.File("logs\\Debug.txt", LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
            //                .WriteTo.File("logs\\Info.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
            //                .CreateLogger()));
            #endregion
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            var app = builder.Build();
            
            app.MapControllers();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapHealthChecks("/health");
            app.UseAuthorization();
            app.MapControllers();
            
            app.UseMiddleware<ExceptionHandleMiddleware>();
            app.UseMiddleware<ResultMiddleware>();

            new StartUp();
            app.Run();
        }


        public class StartUp
        {
            #region WINAPI
            [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);
            [DllImport("USER32.DLL")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
            [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
            private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
            // Define the SetWindowPos API function.
            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint Flags);
            [DllImport("user32.dll", SetLastError = true)]
            static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
            [DllImport("user32.dll")]
            private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            // isZoomed for maximized and isIconic for minimized
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool IsIconic(IntPtr hWnd);
            [DllImport("user32.dll")]
            static extern bool IsZoomed(IntPtr hWnd);

            private const short SWP_NOMOVE = 0X2;
            private const short SWP_NOSIZE = 1;
            private const short SWP_NOZORDER = 0X4;
            private const int SWP_SHOWWINDOW = 0x0040;
            private const int SW_SHOWNORMAL = 1;
            private const int SW_SHOWMINIMIZED = 2;
            private const int SW_SHOWMAXIMIZED = 3;
            private const int SW_RESTORE = 9;
            #endregion
            #region Members
            DateTime lastUpdate = DateTime.Now;
            int refreshRate = 30;
            Process netProcess;
            Dictionary<string, DateTime> dicSpidernets = new Dictionary<string, DateTime>();
            Timer _timer;
            #endregion

            public StartUp()
            {
                _timer = new Timer(1000);
                _timer.Elapsed += _timer_Elapsed;
                _timer.AutoReset = true; // repeat forever
                _timer.Enabled = true;

                //StartServer();
                ReStartNetProcess();

                var command = Console.ReadLine();
            }
            #region Method

            async void ReStartNetProcess()
            {
                string[] execs = new string[]
                {
                    "--disable-web-security --window-size=800,600 --user-data-dir=C:\\Users\\James.lin\\Desktop\\Spider https://www.bet365.com/#/IP/B13",
                    "--disable-web-security --window-size=800,600 --user-data-dir=C:\\Users\\James.lin\\Desktop\\Spider2 https://www.bet365.com/#/IP/B92",
                    "--disable-web-security --window-size=800,600 --user-data-dir=C:\\Users\\James.lin\\Desktop\\Spider3 https://www.bet365.com/#/IP/B1",
                    "--disable-web-security --window-size=800,600 --user-data-dir=C:\\Users\\James.lin\\Desktop\\Spider4 https://www.bet365.com/#/IP/B13",
                    "--disable-web-security --window-size=800,600 --user-data-dir=C:\\Users\\James.lin\\Desktop\\Spider5 https://www.bet365.com/#/IP/B92",
                };
                foreach (var each in execs)
                {
                    var url = each.Split(' ')[each.Split(' ').Length - 1];
                    var processList = Process.GetProcessesByName("chrome");
                    if (!processList.Any(a => a.MainWindowTitle.EndsWith(url)))
                        Execute("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe", each, IsConsole: false);
                }

                while (true)
                {
                    var processList = Process.GetProcessesByName("chrome");
                    var chromeList = processList.Where(f => !string.IsNullOrEmpty(f.MainWindowTitle));
                    Debug.WriteLine($"[Refresh {chromeList.Count()}] {string.Join("##", chromeList.Select(s => s.MainWindowTitle).ToArray())}");
                    int position = 5;
                    foreach (var p in chromeList)
                    {
                        SetProcessForeground(p.MainWindowHandle);
                        SetWindowRect(p, position, position, 1200, 600, true);
                        position += 40;
                        await Task.Delay(3000);
                    }
                }
            }

            Process Execute(string exeFile, string argument = "", Action<string> outputCallback = null, Action<string> errorCallback = null, bool IsConsole = true)
            {
                ProcessStartInfo si = new ProcessStartInfo()
                {
                    FileName = exeFile,
                    Arguments = argument,
                    //必須要設定以下兩個屬性才可將輸出結果導向
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    //不顯示任何視窗
                    CreateNoWindow = true
                };
                Process p = new Process()
                {
                    StartInfo = si,
                    EnableRaisingEvents = true,
                };
                p.Start();

                //透過OutputDataReceived及ErrorDataReceived即時接收輸出內容
                p.OutputDataReceived += (o, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data) && outputCallback != null)
                    {
                        outputCallback(e.Data);
                    }
                };
                p.ErrorDataReceived += (o, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data) && errorCallback != null)
                    {
                        errorCallback(e.Data);
                    }
                };
                //呼叫Begin*ReadLine()開始接收輸出結果
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                if (IsConsole)
                    p.WaitForExit();

                return p;
            }

            private void SetProcessForeground(IntPtr handle)
            {
                if (handle == IntPtr.Zero)
                {
                    return;
                }
                UnMinimizeWindow(handle);
                SetForegroundWindow(handle);
            }
            private void UnMinimizeWindow(IntPtr handle)
            {
                if (IsIconic(handle))
                {
                    ShowWindowAsync(handle, SW_RESTORE);
                }
            }
            private void SetWindowRect(Process process, int x = -1, int y = -1, int width = -1, int height = -1, bool isForce = false)
            {
                GetWindowRect(process.MainWindowHandle, out Rectangle rect);
                if (isForce || (rect.Width - rect.X) < 1200)
                {
                    Debug.WriteLine($"{process.MainWindowTitle} [{rect}]");

                    SetWindowPos(process.MainWindowHandle, new IntPtr(-1),
                        x > 0 ? x : rect.X,
                        y > 0 ? y : rect.Y,
                        width > 0 ? width : rect.Width,
                        height > 0 ? height : rect.Height,
                        SWP_NOZORDER | SWP_SHOWWINDOW);
                }
            }


            private void _timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                var processList = Process.GetProcessesByName("chrome");

                //foreach (Process p in processList.Where(f => !string.IsNullOrEmpty(f.MainWindowTitle)))
                //{

                //}

                //foreach (var each in dicSpidernets.Where(f => f.Value < DateTime.Now.AddSeconds(-refreshRate)))
                //{
                //    foreach (Process p in processList)
                //    {
                //        if (p.MainWindowTitle != "" && p.MainWindowTitle.Contains(each.Key))
                //        {
                //            Debug.WriteLine($"[Refresh] {p.MainWindowTitle}");
                //            SetProcessForeground(p.MainWindowHandle);
                //            PostMessage(p.MainWindowHandle, 0x100, 0x74, 0);
                //            dicSpidernets[each.Key] = DateTime.Now;
                //        }
                //        else
                //        {
                //            dicSpidernets.Remove(each.Key);
                //        }
                //    }
                //}
            }

            private void ApiServer_DataReceived(object sender, Tuple<string, string, object> e)
            {
                if (e.Item3 is string strData)
                    OnLog(strData);
            }

            public void OnLog(string log)
            {
                if (log.Contains("#") && log.Split('#') is string[] arr && arr.Length == 2 && arr[1] is string url)
                {
                    if (!dicSpidernets.ContainsKey(url))
                    {
                        dicSpidernets.Add(url, DateTime.Now);
                    }
                    else if (log.Contains("data changed"))
                    {
                        dicSpidernets[url] = DateTime.Now;
                    }
                }
            }
            #endregion
        }
    }
}