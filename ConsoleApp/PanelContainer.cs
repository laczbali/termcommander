﻿using Display;
using Display.Models;

namespace ConsoleApp;
internal class PanelContainer : NcWindow
{
    protected override void InitializeInner(bool softInit = false)
    {
        var leftPanel = new FilesystemView();
        var rightPanel = new FilesystemView();

        leftPanel.Initialize(Size.GetHorizontalPartial(2, 0));
        rightPanel.Initialize(Size.GetHorizontalPartial(2, 1));

        Children.Add(leftPanel);
        Children.Add(rightPanel);

        Children[0].IsActive = true;
    }

    protected override UpdateResult UpdateInner(string? keypressed)
    {
        if (keypressed == "left")
        {
            Children[0].IsActive = true;
            Children[1].IsActive = false;
        }
        if (keypressed == "right")
        {
            Children[0].IsActive = false;
            Children[1].IsActive = true;
        }

        return new UpdateResult();
    }

    protected override void DisposeInner(bool softDispose = false)
    {
        Children.Clear();
    }
}
