﻿using Caliburn.Micro;
using Microsoft.Win32;
using MinFiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UI.ViewModels
{
    public class MainViewModel : Conductor<IScreen>, IShell
    {
        private IWindowManager windowManager;
        private string fileName;
        private long fileSize;
        private double fileEntropy;
        private int countBlocks;
        private double blockEntropy;
        private int progressBar;
        private string timer;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private Stopwatch stopwatch = new Stopwatch();
        private string currentTime = "00:00:00";

        public MainViewModel(IWindowManager windowManager)
        {
            DisplayName = "MinFiler";
            this.windowManager = windowManager;
            BlockEntropy = 4;
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
        }

        #region Property
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }
        public long FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                NotifyOfPropertyChange(() => FileSize);
            }
        }
        public double FileEntropy
        {
            get { return fileEntropy; }
            set
            {
                fileEntropy = value;
                NotifyOfPropertyChange(() => FileEntropy);
            }
        }
        public int CountBlocks
        {
            get { return countBlocks; }
            set
            {
                countBlocks = value;
                NotifyOfPropertyChange(() => CountBlocks);
            }
        }
        public double BlockEntropy
        {
            get { return blockEntropy; }
            set
            {
                blockEntropy = value;
                NotifyOfPropertyChange(() => BlockEntropy);
            }
        }
        public int ProgressBar
        {
            get { return progressBar; }
            set
            {
                if (progressBar >= 100)
                    progressBar = 0;
                else
                    progressBar = value;
                NotifyOfPropertyChange(() => ProgressBar);
            }
        }
        public string Timer
        {
            get { return timer; }
            set
            {
                timer = value;
                NotifyOfPropertyChange(() => Timer);
            }
        }
        #endregion

        #region Button
        public void SelectFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
                FileSize = new FileInfo(FileName).Length;
            }



        }
        public void GenerateFile()
        {
            var createFileDialog = new FileCreaterViewModel();
            var result = windowManager.ShowDialog(createFileDialog);
            if (result == true)
            {
                //generate file
            }
        }
        public async void Bloking()
        {
            var blockFiler = new BlockFiler(FileName, BlockEntropy);
            blockFiler.AddProgress += AddProgress;
            blockFiler.Finish += Finish;
            ResetTimer();
            StartTimer();
            await blockFiler.BlokingAsync();
            StopTimer();
            FileEntropy = blockFiler.FileEntropy;
            CountBlocks = blockFiler.Blocks.Count;

        }
        public async void BlokingVer2()
        {
            var blockFiler = new BlockFiler(FileName, BlockEntropy);
            blockFiler.AddProgress += AddProgress;
            blockFiler.Finish += Finish;
            ResetTimer();
            StartTimer();
            await blockFiler.Bloking_Ver2Async();
            StopTimer();
            FileEntropy = blockFiler.FileEntropy;
            CountBlocks = blockFiler.Blocks.Count;
        }
        public void AddProgress()
        {
            ProgressBar++;
        }
        public void Finish()
        {
            ProgressBar = 0;
        }
        #endregion

        #region Methods
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                TimeSpan timeSpan = stopwatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                    timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
                Timer = currentTime;
            }
        }
        private void StartTimer()
        {
            stopwatch.Start();
            dispatcherTimer.Start();
        }
        private void ResetTimer()
        {
            stopwatch.Reset();
            Timer = "00:00:00";
        }
        private void StopTimer()
        {
            stopwatch.Stop();
            dispatcherTimer.Stop();
        }
        #endregion
    }
}
