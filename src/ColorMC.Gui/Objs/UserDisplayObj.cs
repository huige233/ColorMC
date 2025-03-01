﻿using ColorMC.Core.Game.Auth;

namespace ColorMC.Gui.Objs;

public record UserDisplayObj
{
    public bool Use { get; set; }
    public string Name { get; set; }
    public string UUID { get; set; }
    public string Type { get; set; }
    public string Text1 { get; set; }
    public string Text2 { get; set; }

    public AuthType AuthType { get; set; }
}

public record UserDisplayObj1
{
    public string Name { get; set; }
    public string Info { get; set; }
    public AuthType Type { get; set; }
}