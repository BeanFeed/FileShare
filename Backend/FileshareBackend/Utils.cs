using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FileshareBackend.Models;
using FileshareBackend.Services;

namespace FileshareBackend;

public class Utils
{
    public static string CleanString(string dirtyString)
    {
        /*
        if (dirtyString == null) dirtyString = "";
        HashSet<char> removeChars = new HashSet<char>("?&^$#@!()+-,:;<>’\'-_*");
        StringBuilder result = new StringBuilder(dirtyString.Length);
        foreach (char c in dirtyString)
            if (!removeChars.Contains(c)) // prevent dirty chars
                result.Append(c);
                */
        //return result.ToString().Replace("..","");
        return dirtyString != null ? dirtyString.Replace("..", "") : "";
    }
    
    public static SameSiteMode GetSSM(IConfiguration config)
    {
        var ssm = config["SSM"];
        if (ssm == null || ssm.ToLower() == "none") return SameSiteMode.None;
        else return SameSiteMode.Lax;
    }

    public static void InitiateMeta(IConfiguration config)
    {
        var path = config["DirectoryRootPath"];
        GenerateMeta(path, config);
    }

    private static void GenerateMeta(string path, IConfiguration config)
    {
        FSEntryModel entries = new FileSystemService(config).GetFromDirectory(path);
        if (!File.Exists(Path.Join(path, "fsmeta.json")))
        {
            List<ItemMeta> DirectoryMetas = new List<ItemMeta>();
            List<ItemMeta> FileMetas = new List<ItemMeta>();
            
            foreach (var dir in entries.Directories)
            {
                ItemMeta meta = new ItemMeta()
                {
                    Name = dir.Name,
                    Owner = "root",
                    Visibility = "public"
                };
                DirectoryMetas.Add(meta);
            }

            foreach (var file in entries.Files)
            {
                ItemMeta meta = new ItemMeta()
                {
                    Name = file.Name,
                    Owner = "root",
                    Visibility = "public"
                };
                FileMetas.Add(meta);
            }

            MetaFileModel metaFile = new MetaFileModel()
            {
                Type = "FileShareMeta",
                Owner = "root",
                Visibility = "public",
                Directories = DirectoryMetas.ToArray(),
                Files = FileMetas.ToArray()
            };
            var stream = File.OpenWrite(Path.Join(path, "fsmeta.json"));
            stream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(metaFile, new JsonSerializerOptions(){WriteIndented = true})));
            stream.Close();
            stream.Dispose();
        }
        foreach (var dir in entries.Directories)
        {
            GenerateMeta(Path.Join(path, dir.Name), config);
        }
        
        
    }
}