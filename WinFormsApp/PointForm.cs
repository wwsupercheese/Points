using PointLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
namespace WinFormsApp
{
    public partial class PointForm : Form
    {
        private Point[]? points = null;
        public PointForm()
        {
            InitializeComponent();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            listBox.DataSource = null;
        }

        private void BtnSort_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            Random rnd = new();

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();
            }

            listBox.DataSource = points;
        }

        private void BtnSerialize_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new()
            {
                Filter = "XML|*.xml|JSON|*.json|YAML|*.yaml|Custom|*.txt"
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using FileStream fs = new(dlg.FileName, FileMode.Create, FileAccess.Write);
            switch (Path.GetExtension(dlg.FileName))
            {
                case ".xml":
                    XmlSerializer xmlSerializer = new(typeof(Point[]), [typeof(Point3D)]);
                    xmlSerializer.Serialize(fs, points);
                    break;

                case ".json":
                    JsonSerializerOptions jsonOptions = new()
                    {
                        WriteIndented = true // Для удобочитаемого формата
                    };
                    
                    // Создаем список для хранения сериализованных объектов
                    List<object> serializedPoints = [.. points];

                    // Сериализуем весь список в файл
                    using (StreamWriter writer = new(fs))
                    {
                        JsonSerializer.Serialize(writer.BaseStream, serializedPoints, jsonOptions);
                    }
                    break;

                case ".yaml":
                    Serializer yamlSerializer = new();
                    using (StreamWriter writer = new(fs))
                    {
                        yamlSerializer.Serialize(writer, points);
                    }
                    break;

                case ".txt":
                    using (StreamWriter writer = new(fs))
                    {
                        foreach (Point point in points)
                        {
                            writer.WriteLine($"X: {point.X}, Y: {point.Y}");
                            if (point is Point3D point3D)
                            {
                                writer.WriteLine($"Z: {point3D.Z}");
                            }
                        }
                    }
                    break;
                default: _ = MessageBox.Show("Unsupported file format."); break;
            }
        }

        private void BtnDeserialize_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                Filter =
                    "All|*.xml;*.json;*.yaml;*.txt|" +
                    "XML|*.xml|" +
                    "JSON|*.json|" +
                    "YAML|*.yaml|" +
                    "Custom|*.txt"
            };
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (FileStream fs = new(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".xml":
                        XmlSerializer xmlSerializer = new(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xmlSerializer.Deserialize(fs);
                        break;

                    case ".json":
                        using (StreamReader reader = new(fs))
                        {
                            string json = reader.ReadToEnd();
                            JsonElement[]? jsonArray = JsonSerializer.Deserialize<JsonElement[]>(json);

                            List<Point> pointList = [];

                            foreach (JsonElement element in jsonArray)
                            {
                                if (element.TryGetProperty("X", out JsonElement x) && element.TryGetProperty("Y", out JsonElement y))
                                {
                                    // Проверяем наличие Z
                                    if (element.TryGetProperty("Z", out JsonElement z))
                                    {
                                        pointList.Add(new Point3D
                                        {
                                            X = x.GetInt32(),
                                            Y = y.GetInt32(),
                                            Z = z.GetInt32()
                                        });
                                    }
                                    else
                                    {
                                        pointList.Add(new Point
                                        {
                                            X = x.GetInt32(),
                                            Y = y.GetInt32()
                                        });
                                    }
                                }
                            }

                            points = [.. pointList];
                        }
                        break;

                    case ".yaml":
                        Deserializer yamlDeserializer = new();
                        using (StreamReader reader = new(fs))
                        {
                            // Десериализуем в динамический объект
                            List<Dictionary<string, object>> dynamicPoints = yamlDeserializer.Deserialize<List<Dictionary<string, object>>>(reader);
                            points = [.. dynamicPoints.Select(dp =>
                            {
                                // Проверяем, содержит ли объект параметр Z
                                return dp.TryGetValue("Z", out object? value)
                                    ? new Point3D
                                    {
                                        X = Convert.ToInt32(dp["X"]),
                                        Y = Convert.ToInt32(dp["Y"]),
                                        Z = Convert.ToInt32(value)
                                    }
                                    : new Point
                                    {
                                        X = Convert.ToInt32(dp["X"]),
                                        Y = Convert.ToInt32(dp["Y"])
                                    };
                            })];
                        }
                        break;

                    case ".txt":
                        using (StreamReader reader = new(fs))
                        {
                            string[] lines = reader.ReadToEnd().Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
                            List<Point> pointList = [];
                            for (int i = 0; i < lines.Length; i++)
                            {
                                string[] coords = lines[i].Split([','], StringSplitOptions.RemoveEmptyEntries);
                                Point point = new()
                                {
                                    X = int.Parse(coords[0].Split(':')[1].Trim()),
                                    Y = int.Parse(coords[1].Split(':')[1].Trim())
                                };

                                // Проверяем, есть ли Z-координата
                                if (i + 1 < lines.Length && lines[i + 1].StartsWith("Z:"))
                                {
                                    Point3D point3D = new()
                                    {
                                        X = point.X,
                                        Y = point.Y,
                                        Z = int.Parse(lines[i + 1].Split(':')[1].Trim())
                                    };
                                    pointList.Add(point3D);
                                    i++; // Пропустить следующую строку с Z
                                }
                                else
                                {
                                    pointList.Add(point);
                                }
                            }
                            points = [.. pointList];
                        }
                        break;
                    default: break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;

        }
    }
}
