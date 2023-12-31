﻿using ConsoleApp.Models;
using ConsoleApp.Windows.Generic;
using NetCurses;
using NetCurses.Components;
using NetCurses.Models;
using System.Diagnostics;

namespace ConsoleApp.Windows.Filesystem;
internal class FilesystemView : ScrollMenu
{
    private string _currentPath = string.Empty;
    private List<string> _files = new();
    private List<string> _dirs = new();
    private List<int> _selectedItems = new();
    private Dictionary<string, int> _cursorPositionHistory = new();

    public bool HasActivePopup => Children.Any();

    // ScrollMenu overrides

    protected override IEnumerable<string> Items
        => _dirs
            .Select(x => "/ " + Path.GetFileName(x))
            .Concat(_files.Select(x => Path.GetFileName(x)));

    protected override void PreDisplayRow(int itemIndex, int lineIndex)
    {
        base.PreDisplayRow(itemIndex, lineIndex);
        if (itemIndex < _dirs.Count)
        {
            this.ToggleColorPair(Colors.BlueOnBlack, true);
        }
        if (_selectedItems.Contains(itemIndex))
        {
            this.ToggleColorPair(Colors.RedOnBlack, true);
        }
    }

    protected override void PostDisplayRow(int itemIndex, int lineIndex)
    {
        base.PostDisplayRow(itemIndex, lineIndex);
        this.ToggleColorPair(Colors.BlueOnBlack, false);
        this.ToggleColorPair(Colors.RedOnBlack, false);
    }

    // NcWindow overrides

    protected override void InitializeInner()
    {
        var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        SelectFolder(startupPath);
    }

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        if (IsActive && !HasActivePopup)
        {
            Footer.Text = "(ENT)Open (BSP)Back (SPC)Select (DEL)ete (C)opy (M)ove (N)ew";

            // cursor movement
            if (keypressed == "up" || keypressed == "down")
                StepCursor(keypressed);

            // item selection
            if (keypressed == "space")
            {
                if (_selectedItems.Contains(CursorItemIndex))
                    _selectedItems.Remove(CursorItemIndex);
                else
                    _selectedItems.Add(CursorItemIndex);
            }

            // item manipulation
            if (keypressed == "enter")
                OpenItem();
            if (keypressed == "backspace")
                CloseItem();
            if (keypressed == "del")
                DeleteItems();
            if (keypressed == "n")
                CreateItem();
        }

        this.ToggleBox(_currentPath);
        DisplayMenu();
        return new UpdateResult();
    }

    private void SelectFolder(string path)
    {
        if (path is null || path == string.Empty) throw new ArgumentException("Path must not be empty");

        var contents = GetFolderContents(path);
        _dirs = contents.dirs;
        _files = contents.files;

        if (_currentPath != string.Empty)
            _cursorPositionHistory[_currentPath] = CursorItemIndex;
        ResetMenu(); // this will reset cursor position
        if (_cursorPositionHistory.TryGetValue(path, out var cursorPosition))
            StepCursor(cursorPosition);

        _currentPath = path;
    }

    // custom methods

    private (List<string> dirs, List<string> files) GetFolderContents(string folder)
    {
        var dirs = Directory.GetDirectories(folder).ToList();
        var files = Directory.GetFiles(folder).ToList();
        return (dirs, files);
    }

    private void OpenItem()
    {
        if (CursorItemIndex < _dirs.Count)
        {
            var dir = _dirs[CursorItemIndex];
            try
            {
                SelectFolder(dir);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine($"Access denied to {dir}");
                new ErrorPopup().InitPopup(this, "Access denied");
            }
        }
        else
        {
            // TODO: file opening
        }

        _selectedItems.Clear();
    }

    private void CloseItem()
    {
        var parentFolder = Directory.GetParent(_currentPath)?.FullName;
        if (Directory.Exists(parentFolder))
        {
            SelectFolder(parentFolder);
        }

        // TODO: file closing
    }

    private void DeleteItems()
    {
        // TODO: delete stuff
    }

    private void CreateItem()
    {
        // TODO: create stuff
    }
}
