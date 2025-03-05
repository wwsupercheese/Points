using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SoapCore.Serializer;
using System.Windows.Forms;
using PointLib;
using System.Text.Json;
using System.Xml.Serialization;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using System.Linq;
namespace WinFormsApp
{
    public partial class PointForm : Form
    {
        private Point[] points = null;
        public PointForm()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            for (int i = 0; i < points.Length; i++)
                points[i] = new Point();

            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

            listBox.DataSource = points;
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "XML|*.xml|JSON|*.json|YAML|*.yaml|Custom|*.txt";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".xml":
                        var xmlSerializer = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        xmlSerializer.Serialize(fs, points);
                        break;

                    case ".json":
                        var jsonOptions = new JsonSerializerOptions
                        {
                            WriteIndented = true // Для удобочитаемого формата
                        };

                        // Создаем список для хранения сериализованных объектов
                        var serializedPoints = new List<object>();

                        foreach (var point in points)
                        {
                            serializedPoints.Add(point);
                        }

                        // Сериализуем весь список в файл
                        using (var writer = new StreamWriter(fs))
                        {
                            JsonSerializer.Serialize(writer.BaseStream, serializedPoints, jsonOptions);
                        }
                        break;

                    case ".yaml":
                        var yamlSerializer = new Serializer();
                        using (var writer = new StreamWriter(fs))
                        {
                            yamlSerializer.Serialize(writer, points);
                        }
                        break;

                    case ".txt":
                        // Пример пользовательской сериализации
                        using (var writer = new StreamWriter(fs))
                        {
                            foreach (var point in points)
                            {
                                writer.WriteLine($"X: {point.X}, Y: {point.Y}");
                                if (point is Point3D point3D)
                                {
                                    writer.WriteLine($"Z: {point3D.Z}");
                                }
                            }
                        }
                        break;

                    default:
                        MessageBox.Show("Unsupported file format.");
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "XML|*.xml|JSON|*.json|YAML|*.yaml|Custom|*.txt";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".xml":
                        var xmlSerializer = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xmlSerializer.Deserialize(fs);
                        break;

                    case ".json":
                        using (var reader = new StreamReader(fs))
                        {
                            var json = reader.ReadToEnd();
                            var jsonArray = JsonSerializer.Deserialize<JsonElement[]>(json);

                            var pointList = new List<Point>();

                            foreach (var element in jsonArray)
                            {
                                if (element.TryGetProperty("X", out var x) && element.TryGetProperty("Y", out var y))
                                {
                                    // Проверяем наличие Z
                                    if (element.TryGetProperty("Z", out var z))
                                    {
                                        // Создаем Point3D
                                        pointList.Add(new Point3D
                                        {
                                            X = x.GetInt32(),
                                            Y = y.GetInt32(),
                                            Z = z.GetInt32()
                                        });
                                    }
                                    else
                                    {
                                        // Создаем Point
                                        pointList.Add(new Point
                                        {
                                            X = x.GetInt32(),
                                            Y = y.GetInt32()
                                        });
                                    }
                                }
                            }

                            points = pointList.ToArray();
                        }
                        break;

                    case ".yaml":
                        var yamlDeserializer = new Deserializer();
                        using (var reader = new StreamReader(fs))
                        {
                            // Десериализуем в динамический объект
                            var dynamicPoints = yamlDeserializer.Deserialize<List<Dictionary<string, object>>>(reader);
                            points = dynamicPoints.Select(dp =>
                            {
                                // Проверяем, содержит ли объект параметр Z
                                if (dp.ContainsKey("Z"))
                                {
                                    return new Point3D
                                    {
                                        X = Convert.ToInt32(dp["X"]),
                                        Y = Convert.ToInt32(dp["Y"]),
                                        Z = Convert.ToInt32(dp["Z"])
                                    };
                                }
                                else
                                {
                                    return new Point
                                    {
                                        X = Convert.ToInt32(dp["X"]),
                                        Y = Convert.ToInt32(dp["Y"])
                                    };
                                }
                            }).ToArray();
                        }
                        break;

                    case ".txt":
                        // Пример пользовательской десериализации
                        using (var reader = new StreamReader(fs))
                        {
                            var lines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            var pointList = new List<Point>();
                            for (int i = 0; i < lines.Length; i++)
                            {
                                var coords = lines[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                var point = new Point
                                {
                                    X = int.Parse(coords[0].Split(':')[1].Trim()),
                                    Y = int.Parse(coords[1].Split(':')[1].Trim())
                                };

                                // Проверяем, есть ли Z-координата
                                if (i + 1 < lines.Length && lines[i + 1].StartsWith("Z:"))
                                {
                                    var point3D = new Point3D
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
                            points = pointList.ToArray();
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
