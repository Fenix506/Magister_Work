﻿using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using MinFiler;

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
        private bool autoSaving;
        private object locker = new object();
        private BlockFiler blockFiler;
        public MainViewModel(IWindowManager windowManager)
        {
            DisplayName = "MinFiler";
            this.windowManager = windowManager;
            BlockEntropy = 7.95;
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
        public bool AutoSaving
        {
            get { return autoSaving; }
            set
            {
                autoSaving = value;
                NotifyOfPropertyChange(() => AutoSaving);
            }
        }
        #endregion

        #region Button
        public void SelectFile()
        {
            var openFileDialog = new CommonOpenFileDialog();
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
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
        public async void BlokingOneThread()
        {
            blockFiler = new BlockFiler(FileName, AddProgress, BlockEntropy);

            StartTimer();
            await blockFiler.BlokingAsync(AutoSaving);
            StopTimer();
            FileEntropy = blockFiler.FileEntropy;
            CountBlocks = blockFiler.BlockList.CountBlocks;

        }
        public async void BlokingMultiThread()
        {
            blockFiler = new BlockFiler(FileName, AddProgress, BlockEntropy);

            StartTimer();

            await blockFiler.ParallelBlokingAsync(AutoSaving);

            StopTimer();
            FileEntropy = blockFiler.FileEntropy;
            CountBlocks = blockFiler.BlockList.CountBlocks;

        }
        public async void BlokingWithCompression()
        {
            blockFiler = new BlockFiler(FileName, AddProgress, BlockEntropy);

            StartTimer();

            await blockFiler.BlokingWithCompressAsync(AutoSaving);

            StopTimer();
            FileEntropy = blockFiler.FileEntropy;
            CountBlocks = blockFiler.BlockList.CountBlocks;

        }
        public void SaveBlocks()
        {
            var folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var saver = new BlockSaver(folderDialog.FileName, blockFiler.FullFileName);
                saver.SaveBlockedFile(blockFiler.BlockList);
            }
        }
        public void CompressFile()
        {
            var folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = Path.Combine(folderDialog.FileName, "compressed");
                var saver = new BlockSaver(path, FileName);
                saver.CompressFile();
            }

        }
        public void DecompressFile()
        {
            var folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = Path.Combine(folderDialog.FileName, "decompressed");
                var saver = new BlockSaver(path, FileName);
                saver.DecompressFile();
            }
        }
        public void CompressBlocks()
        {
            var folderDialogWhereBlocks = new CommonOpenFileDialog();
            folderDialogWhereBlocks.Title = "Folder with blocks";
            folderDialogWhereBlocks.IsFolderPicker = true;
            if (folderDialogWhereBlocks.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folderDialogWhereSave = new CommonOpenFileDialog();
                folderDialogWhereSave.Title = "Folder where save blocks";
                folderDialogWhereSave.IsFolderPicker = true;
                if (folderDialogWhereSave.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var whereSavePath = Path.Combine(folderDialogWhereSave.FileName, "compressed");
                    var saver = new BlockSaver(whereSavePath, folderDialogWhereBlocks.FileName);
                    saver.CompressBlocksFile();
                }
            }
        }

        public void DecompressBlocks() 
        {
            var folderDialogWhereBlocks = new CommonOpenFileDialog();
            folderDialogWhereBlocks.Title = "Folder with blocks";
            folderDialogWhereBlocks.IsFolderPicker = true;
            if (folderDialogWhereBlocks.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folderDialogWhereSave = new CommonOpenFileDialog();
                folderDialogWhereSave.Title = "Folder where save blocks";
                folderDialogWhereSave.IsFolderPicker = true;
                if (folderDialogWhereSave.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var whereSavePath = Path.Combine(folderDialogWhereSave.FileName, "decompressed");
                    var saver = new BlockSaver(whereSavePath, folderDialogWhereBlocks.FileName);
                    saver.DecompressBloksFile();
                }
            }
        }
        public void Deblocking()
        {
            var folderDialogWhereBlocks = new CommonOpenFileDialog();
            folderDialogWhereBlocks.Title = "Folder with blocks";
            folderDialogWhereBlocks.IsFolderPicker = true;
            if (folderDialogWhereBlocks.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folderDialogWhereSave = new CommonOpenFileDialog();
                folderDialogWhereSave.Title = "Folder where save blocks";
                folderDialogWhereSave.IsFolderPicker = true;
                if (folderDialogWhereSave.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    
                    var saver = new DeblockFiler(folderDialogWhereBlocks.FileName, folderDialogWhereSave.FileName);
                    saver.Deblocking();
                }
            }
        }
        #endregion

        #region Methods
        private void AddProgress()
        {
            lock (locker) { ProgressBar++; }
        }
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
            ResetTimer();
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
