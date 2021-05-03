using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace ZipExtractor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxRetries = 2;
        private BackgroundWorker _backgroundWorker;
        private readonly StringBuilder _logBuilder = new StringBuilder();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _logBuilder.AppendLine(DateTime.Now.ToString("F"));
            _logBuilder.AppendLine();
            _logBuilder.AppendLine("ZipExtractor 以下列命令行参数启动。");

            string[] args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                _logBuilder.AppendLine($"[{index}] {arg}");
            }

            _logBuilder.AppendLine();

            if (args.Length >= 4)
            {
                string executablePath = args[3];

                // Extract all the files.
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };

                _backgroundWorker.DoWork += (o, eventArgs) =>
                {
                    foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(executablePath)))
                    {
                        try
                        {
                            if (process.MainModule != null && process.MainModule.FileName.Equals(executablePath))
                            {
                                _logBuilder.AppendLine("等待应用进程退出……");

                                _backgroundWorker.ReportProgress(0, "等待应用退出……");
                                process.WaitForExit();
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception.Message);
                        }
                    }

                    _logBuilder.AppendLine("BackgroundWorker 成功启动");

                    var path = args[2];

                    // Ensures that the last character on the extraction path
                    // is the directory separator char.
                    // Without this, a malicious zip file could try to traverse outside of the expected
                    // extraction path.
                    if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                        path += Path.DirectorySeparatorChar;
                    var archive = ZipFile.Open(args[1], ZipArchiveMode.Read, Encoding.GetEncoding("GBK"));
                    var entries = archive.Entries;
                    _logBuilder.AppendLine($"在此 zip 文件中找到总共 {entries.Count} 个文件和文件夹。");

                    try
                    {
                        int progress = 0;
                        for (var index = 0; index < entries.Count; index++)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                eventArgs.Cancel = true;
                                break;
                            }
                            var entry = entries[index];
                            string currentFile = string.Format("正在解压 {0}", entry.FullName);
                            _backgroundWorker.ReportProgress(progress, currentFile);
                            int retries = 0;
                            bool notCopied = true;
                            while (notCopied)
                            {
                                string filePath = string.Empty;
                                try
                                {
                                    filePath = Path.Combine(path, entry.FullName);
                                    if (entry.Name != "")
                                    {
                                        var parentDirectory = Path.GetDirectoryName(filePath);
                                        if (!Directory.Exists(parentDirectory))
                                        {
                                            Directory.CreateDirectory(parentDirectory);
                                        }
                                        entry.ExtractToFile(filePath, true);
                                    }
                                    notCopied = false;
                                }
                                catch (IOException exception)
                                {
                                    const int errorSharingViolation = 0x20;
                                    const int errorLockViolation = 0x21;
                                    var errorCode = Marshal.GetHRForException(exception) & 0x0000FFFF;
                                    if (errorCode == errorSharingViolation || errorCode == errorLockViolation)
                                    {
                                        retries++;
                                        if (retries > MaxRetries)
                                        {
                                            throw;
                                        }

                                        List<Process> lockingProcesses = null;
                                        if (Environment.OSVersion.Version.Major >= 6 && retries >= 2)
                                        {
                                            try
                                            {
                                                lockingProcesses = FileUtil.WhoIsLocking(filePath);
                                            }
                                            catch (Exception)
                                            {
                                                // ignored
                                            }
                                        }

                                        if (lockingProcesses == null)
                                        {
                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            foreach (var lockingProcess in lockingProcesses)
                                            {
                                                var dialogResult = MessageBox.Show(
                                                    string.Format("{0} 仍处于打开状态并且其正在使用“{1}”。请手动关闭此进程并重试。", lockingProcess.ProcessName, filePath), "无法更新该文件！",
                                                    MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                                if (dialogResult == MessageBoxResult.Cancel)
                                                {
                                                    throw;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                            }

                            progress = (index + 1) * 100 / entries.Count;
                            _backgroundWorker.ReportProgress(progress, currentFile);

                            _logBuilder.AppendLine($"{currentFile} [{progress}%]");
                        }
                    }
                    finally
                    {
                        archive.Dispose();
                    }
                };

                _backgroundWorker.ProgressChanged += (o, eventArgs) =>
                {
                    pbExtract.Value = eventArgs.ProgressPercentage;
                    textInformation.Text = eventArgs.UserState.ToString();
                };

                _backgroundWorker.RunWorkerCompleted += (o, eventArgs) =>
                {
                    try
                    {
                        if (eventArgs.Error != null)
                        {
                            throw eventArgs.Error;
                        }
                        if (!eventArgs.Cancelled)
                        {
                            textInformation.Text = @"Finished";
                            try
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo(executablePath);
                                if (args.Length > 4)
                                {
                                    processStartInfo.Arguments = args[4];
                                }
                                Process.Start(processStartInfo);
                                _logBuilder.AppendLine("已成功启动更新后的应用。");
                            }
                            catch (Win32Exception exception)
                            {
                                if (exception.NativeErrorCode != 1223)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        _logBuilder.AppendLine();
                        _logBuilder.AppendLine(exception.ToString());

                        MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        _logBuilder.AppendLine();
                        Application.Current.Shutdown();
                    }
                };
                _backgroundWorker.RunWorkerAsync();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _backgroundWorker?.CancelAsync();

            _logBuilder.AppendLine();
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipExtractor.log"),
                _logBuilder.ToString());
        }
    }
}
