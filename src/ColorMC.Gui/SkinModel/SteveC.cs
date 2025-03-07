﻿namespace ColorMC.Gui.SkinModel;

public enum SkinType
{
    /// <summary>
    /// 1.7旧版
    /// </summary>
    Old,
    /// <summary>
    /// 1.8新版
    /// </summary>
    New,
    /// <summary>
    /// 1.8新版纤细
    /// </summary>
    NewSlim,
    /// <summary>
    /// 未知的类型
    /// </summary>
    Unkonw
}


public record ModelItem
{
    public float[] Model;
    public ushort[] Point;
}

public record SteveModel
{
    public ModelItem Head;
    public ModelItem Body;
    public ModelItem LeftArm;
    public ModelItem RightArm;
    public ModelItem LeftLeg;
    public ModelItem RightLeg;
    public ModelItem Cape;
}

public class Steve3DModel
{
    public CubeC CubeC = new();
    public SteveModel GetSteve(SkinType modelType)
    {
        return new()
        {
            Head = new()
            {
                Model = CubeC.GetSquare(),
                Point = CubeC.GetSquareIndicies()
            },
            Body = new()
            {
                Model = CubeC.GetSquare(multiplyZ: 0.5f, multiplyY: 1.5f),
                Point = CubeC.GetSquareIndicies()
            },
            LeftArm = modelType == SkinType.NewSlim ? new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.375f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f
                ),
                Point = CubeC.GetSquareIndicies()
            } : new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f
                ),
                Point = CubeC.GetSquareIndicies()
            },
            RightArm = modelType == SkinType.NewSlim ? new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.375f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f
                ),
                Point = CubeC.GetSquareIndicies()
            } : new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f
                ),
                Point = CubeC.GetSquareIndicies()
            },
            LeftLeg = new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f
                ),
                Point = CubeC.GetSquareIndicies()
            },
            RightLeg = new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f
                ),
                Point = CubeC.GetSquareIndicies()
            },
            Cape = new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 1.25f,
                    multiplyZ: 0.1f,
                    multiplyY: 2f
                ),
                Point = CubeC.GetSquareIndicies()
            }
        };
    }

    public SteveModel GetSteveTop(SkinType modelType)
    {
        var model = new SteveModel
        {
            Head = new()
            {
                Model = CubeC.GetSquare(
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            }
        };

        if (modelType != SkinType.Old)
        {
            model.Body = new()
            {
                Model = CubeC.GetSquare(
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            };

            model.LeftArm = modelType == SkinType.NewSlim ? new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.375f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            } : new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            };

            model.RightArm = modelType == SkinType.NewSlim ? new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.375f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            } : new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            };

            model.LeftLeg = new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            };
            model.RightLeg = new()
            {
                Model = CubeC.GetSquare(
                    multiplyX: 0.5f,
                    multiplyZ: 0.5f,
                    multiplyY: 1.5f,
                    enlarge: 1.125f
                ),
                Point = CubeC.GetSquareIndicies()
            };
        }

        return model;
    }
}
