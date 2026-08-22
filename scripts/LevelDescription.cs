using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace Mario3D.scripts;

public class Square
{
    [JsonPropertyName("height")] public int Height { get; set; }
    [JsonPropertyName("width")] public int Width { get; set; }
    [JsonPropertyName("x")] public int Y { get; set; }
    [JsonPropertyName("y")] public int X { get; set; }

    public Vector3I GetOrigin()
    {
        return new Vector3I(X / 16, -Y / 16, 0);
    }
}

public class Enemy
{
    [JsonPropertyName("player_name")] public string Type { get; set; }
    [JsonPropertyName("height")] public int Height { get; set; }
    [JsonPropertyName("width")] public int Width { get; set; }
    [JsonPropertyName("x")] public int Y { get; set; }
    [JsonPropertyName("y")] public int X { get; set; }
}

public class Dynamic
{
    [JsonPropertyName("enemies")] public Enemy[] Enemies { get; set; }
}

public class Static
{
    [JsonPropertyName("positionCollision")]
    public Square[] PositionCollisions { get; set; }

    [JsonPropertyName("spawn")] public Square Spawn { get; set; }
    [JsonPropertyName("end")] public Square End { get; set; }
    [JsonPropertyName("magicBean")] public Square MagicBean { get; set; }
    [JsonPropertyName("pipes")] public Square[] Pipes { get; set; }
}

public class LevelDescription
{
    [JsonPropertyName("levelCols")] public int LevelCols { get; set; }
    [JsonPropertyName("levelRows")] public int LevelRows { get; set; }
    [JsonPropertyName("static")] public Static Static { get; set; }
    [JsonPropertyName("dynamic")] public Dynamic Dynamic { get; set; }

    void ParseJson(Json json)
    {
    }
}