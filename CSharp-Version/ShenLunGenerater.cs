using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json; // 需要通过 NuGet 包管理器安装 Newtonsoft.Json

public class Slscq
{
    private Dictionary<string, List<string>> data;
    private Random random;

    public Slscq(string jsonPath)
    {
        try
        {
            string json = File.ReadAllText(jsonPath);
            data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            Console.WriteLine($"尝试打开的数据源文件路径: {jsonPath}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"错误: 数据源文件 {jsonPath} 未找到");
            data = new Dictionary<string, List<string>>();
        }
        catch (JsonSerializationException)
        {
            Console.WriteLine($"错误: 数据源文件 {jsonPath} 格式不正确");
            data = new Dictionary<string, List<string>>();
        }
        random = new Random((int)DateTime.Now.Ticks);
    }

    public string GetRandomElement(string elementType)
    {
        if (data.ContainsKey(elementType))
        {
            return data[elementType][random.Next(data[elementType].Count)];
        }
        return string.Empty;
    }

    public string ReplaceXx(string inputStr, string theme)
    {
        return inputStr.Replace("xx", theme);
    }

    public string ReplaceVn(string inputStr)
    {
        return Regex.Replace(inputStr, @"vn", match =>
        {
            int count = random.Next(1, 5);
            var elements = new List<string>();
            for (int i = 0; i < count; i++)
            {
                elements.Add(GetRandomElement("verb") + GetRandomElement("noun"));
            }
            return string.Join("，", elements);
        });
    }

    public string ReplaceV(string inputStr)
    {
        return Regex.Replace(inputStr, @"v", match => GetRandomElement("verb"));
    }

    public string ReplaceN(string inputStr)
    {
        return Regex.Replace(inputStr, @"n", match => GetRandomElement("noun"));
    }

    public string ReplaceSs(string inputStr)
    {
        return Regex.Replace(inputStr, @"ss", match => GetRandomElement("sentence"));
    }

    public string ReplaceSp(string inputStr)
    {
        return Regex.Replace(inputStr, @"sp", match => GetRandomElement("parallel_sentence"));
    }

    public string ReplaceP(string inputStr)
    {
        return Regex.Replace(inputStr, @"p", match => GetRandomElement("phrase"));
    }

    public string ReplaceAll(string inputStr, string theme)
    {
        inputStr = ReplaceVn(inputStr);
        inputStr = ReplaceV(inputStr);
        inputStr = ReplaceN(inputStr);
        inputStr = ReplaceSs(inputStr);
        inputStr = ReplaceSp(inputStr);
        inputStr = ReplaceP(inputStr);
        inputStr = ReplaceXx(inputStr, theme);
        return inputStr;
    }

    public Dictionary<string, string> Gen(string theme = "年轻人买房", int essayNum = 500)
    {
        int endNum = (int)(essayNum * 0.15);
        int beginNum = (int)(essayNum * 0.15);
        int bodyNum = (int)(essayNum * 0.7);

        string title = ReplaceAll(GetRandomElement("title"), theme);
        string begin = "";
        string body = "";
        string end = "";

        while (begin.Length < beginNum)
        {
            begin += ReplaceAll(GetRandomElement("beginning"), theme);
        }

        while (body.Length < bodyNum)
        {
            body += ReplaceAll(GetRandomElement("body"), theme);
        }

        while (end.Length < endNum)
        {
            end += ReplaceAll(GetRandomElement("ending"), theme);
        }

        return new Dictionary<string, string>
        {
            { "title", title },
            { "begin", begin },
            { "body", body },
            { "end", end }
        };
    }

    public string GenText(string theme = "年轻人买房", int essayNum = 500)
    {
        var result = Gen(theme, essayNum);
        return $"{result["title"]}\n    {result["begin"]}\n    {result["body"]}\n    {result["end"]}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        string theme = "年轻人买房";
        int essayNum = 500;
        string dataSource = "data.json";

        if (args.Length > 0)
        {
            theme = args[0];
        }
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-n" || args[i] == "--essay_num")
            {
                if (i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out essayNum);
                }
            }
            else if (args[i] == "-d" || args[i] == "--data_source")
            {
                if (i + 1 < args.Length)
                {
                    dataSource = args[i + 1];
                }
            }
        }

        // 交互式输入
        if (theme == "年轻人买房")
        {
            Console.Write("欢迎使用申论生成器，本程序会根据你提供的主题，结合data.json中的数据，生成一篇申论文章。\n请注意，本程序应当与data.json文件处在同一目录下运行\n");
            Console.Write("请输入文章主题 (默认: 年轻人买房): ");
            string input = Console.ReadLine();
            theme = string.IsNullOrEmpty(input) ? "年轻人买房" : input;
        }
        if (essayNum == 500)
        {
            Console.Write("请输入文章最少字数 (默认: 500): ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out essayNum))
            {
                essayNum = 500;
            }
        }

        try
        {
            Slscq arcGen = new Slscq(dataSource);
            string arcText = arcGen.GenText(theme, essayNum);
            int actualEssayNum = arcText.Length;
            Console.WriteLine($"生成的文章主题: {theme}");
            Console.WriteLine($"生成的文章字数: {actualEssayNum}");
            Console.WriteLine("\n生成的文章:");
            Console.WriteLine(arcText);

            // 提示用户是否保存文章到文件
            Console.Write("是否要将文章保存到文件？(y/n): ");
            string saveChoice = Console.ReadLine();
            if (saveChoice.Trim().ToLower() == "y")
            {
                Console.Write("请输入保存文件的路径（例如：C:\\Users\\YourName\\Desktop\\article.txt）: ");
                string filePath = Console.ReadLine();
                try
                {
                    File.WriteAllText(filePath, arcText);
                    Console.WriteLine($"文章已成功保存到 {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"保存文件时发生错误: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }

        // 等待用户输入后再关闭控制台窗口
        Console.WriteLine("按回车键退出...");
        Console.ReadLine();
    }
}
