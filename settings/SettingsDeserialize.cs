using System.Collections.Generic;

public class Settings
{
    public bool Clearaftercopy { get; set; } = false;
    public bool Clearforcopy { get; set; } = false;
    public bool? Listingart { get; set; }
    public bool Savelastpaths { get; set; } = true;
    public string? Pathfrom { get; set; }
    public string? Pathto { get; set; }
    public int Packageformat { get; set; } = 0;
    public bool Overrite { get; set; } = true;

    public bool DateAsName { get; set; } = true;

    public string? ZipName { get; set; }

    public string? TempfolderPath { get; set; }

    public bool MultipleSources { get; } = true;
    public List<string> Sources { get; set; } = new();
    public bool CreateOwnFolder { get; set; } = false;
    public string? OwnFolderName { get; set; }
    public bool OwnFolderDate { get; set; } = false;
    
    public bool CustomMica { get; set; } = true;

    public byte MicaIntensy { get; set; } = 0;
}

    
