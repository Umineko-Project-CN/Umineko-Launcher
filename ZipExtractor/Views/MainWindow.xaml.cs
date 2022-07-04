using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using ZipExtractor.Views;
using ZipExtractor.Localization;

namespace ZipExtractor
{
    public partial class MainWindow : AnimatedWindow
    {
        private const int MaxRetries = 2;
        private readonly string[] _args;
        private readonly BackgroundWorker _backgroundWorker;
        private readonly string _executableArgs;
        private readonly string _executablePath;
        private readonly string _extractPath;
        private readonly bool _hasExecutable;
        private readonly StringBuilder _logBuilder = new StringBuilder();
        private readonly string _zipFilePath;

        public MainWindow()
        {
            InitializeComponent();
            _args = Environment.GetCommandLineArgs();
            // args[0] 为自身完整路径。
            if (_args.Length < 3)
            {
                // 参数不足。
                return;
            }
            // 读入命令行参数。
            _zipFilePath = _args[1];
            _extractPath = _args[2];
            if (_hasExecutable = _args.Length >= 4)
            {
                _executablePath = _args[3];
                _executableArgs = _args.Length > 4 ? _args[4] : string.Empty;
            }
            _backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _backgroundWorker.DoWork += DoWork;
            _backgroundWorker.ProgressChanged += ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (_hasExecutable)
            {
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(_executablePath));
                foreach (var process in processes)
                {
                    try
                    {
                        if (process.MainModule != null && process.MainModule.FileName.Equals(_executablePath))
                        {
                            _logBuilder.AppendLine(Lang.Wait);
                            _backgroundWorker.ReportProgress(0, Lang.Wait1);
                            process.WaitForExit();
                        }
                    }
                    catch (Exception exception)
                    {
                        _logBuilder.AppendLine($"{Lang.Exception}\n{exception.Message}");
                    }
                }
            }
            _logBuilder.AppendLine($"BackgroundWorker {Lang.Worker_Started}");
            // 保证解压路径的最后一个字符是目录分隔符。否则，恶意 zip 文件可能会遍历到期望的解压路径之外。
            string path = _extractPath;
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                path += Path.DirectorySeparatorChar;
            }
            ZipArchive archive = ZipFile.Open(_zipFilePath, ZipArchiveMode.Read, Encoding.GetEncoding("GBK"));
            ReadOnlyCollection<ZipArchiveEntry> entries = archive.Entries;
            _logBuilder.AppendLine(Lang.Count.Replace("{0}", entries.Count.ToString()));
            try
            {
                int progress = 0;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (_backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    ZipArchiveEntry entry = entries[i];
                    string currentInfo = $"{Lang.Extracting} {entry.FullName}";
                    _backgroundWorker.ReportProgress(progress, currentInfo);
                    int retries = 0;
                    bool extracted = false;
                    while (!extracted)
                    {
                        string filePath = string.Empty;
                        try
                        {
                            filePath = Path.Combine(path, entry.FullName);
                            if (entry.Name != "")
                            {
                                string parentDirectory = Path.GetDirectoryName(filePath);
                                if (!Directory.Exists(parentDirectory))
                                {
                                    Directory.CreateDirectory(parentDirectory);
                                }
                                entry.ExtractToFile(filePath, true);
                            }
                            extracted = true;
                        }
                        catch (IOException exception)
                        {
                            const int errorSharingViolation = 0x20;
                            const int errorLockViolation = 0x21;
                            int errorCode = Marshal.GetHRForException(exception) & 0x0000FFFF;
                            if (errorCode != errorSharingViolation && errorCode != errorLockViolation)
                            {
                                throw;
                            }
                            else
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
                                    catch
                                    {
                                        // 忽略。
                                    }
                                }
                                if (lockingProcesses != null)
                                {
                                    foreach (var lockingProcess in lockingProcesses)
                                    {
                                        MessageBoxResult dialogResult =
                                            MessageBox.Show(Lang.Error_Occupied.Replace("{0}", lockingProcess.ProcessName).Replace("{1}", filePath),
                                                            Lang.Cannot_Update, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                                        if (dialogResult == MessageBoxResult.Cancel)
                                        {
                                            throw;
                                        }
                                    }
                                }
                                else
                                {
                                    // 等待五秒钟。
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                    }
                    progress = (i + 1) * 100 / entries.Count;
                    _backgroundWorker.ReportProgress(progress, currentInfo);
                    _logBuilder.AppendLine($"{currentInfo} [{progress}%]");
                }
            }
            finally
            {
                archive.Dispose();
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 更新 UI 界面。
            pbProgress.Value = e.ProgressPercentage;
            textStatus.Text = e.UserState.ToString();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }
                if (e.Cancelled)
                {
                    return;
                }
                textStatus.Text = Lang.Completed;
                if (_hasExecutable)
                {
                    try
                    {
                        var processStartInfo = new ProcessStartInfo(_executablePath)
                        {
                            Arguments = _executableArgs
                        };
                        Process.Start(processStartInfo);
                        _logBuilder.AppendLine(Lang.Launch_Success);
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
                _logBuilder.AppendLine($"{Lang.Exception}\n{exception.Message}");
                MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _logBuilder.AppendLine();
                Application.Current.MainWindow.Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // 取消当前任务。
            _backgroundWorker?.CancelAsync();
            // 写入日志。
            _logBuilder.AppendLine();
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipExtractor.log"),
                               _logBuilder.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _logBuilder.AppendLine(DateTime.Now.ToString("F"));
            _logBuilder.AppendLine();
            _logBuilder.AppendLine($"ZipExtractor {Lang.Started}");
            for (int i = 0; i < _args.Length; i++)
            {
                _logBuilder.AppendLine($"[{i}] {_args[i]}");
            }
            _logBuilder.AppendLine();
            // 解压所有文件。
            _backgroundWorker?.RunWorkerAsync();
        }
    }
}