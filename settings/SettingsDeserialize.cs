using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;


public class Settings
{
    public bool Clearaftercopy { get; set; }
    public bool Clearforcopy { get; set; }
    public bool? Listingart { get; set; }
    public bool Savelastpaths { get; set; }
    public string Pathfrom { get; set; }
    public string Pathto { get; set; }
    public string Packageformat { get; set; }
    public bool Overrite { get; set; }
}
