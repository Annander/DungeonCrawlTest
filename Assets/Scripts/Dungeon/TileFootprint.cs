using UnityEngine;

[CreateAssetMenu(fileName = EditorStrings.TileFootprint_FileName, menuName = EditorStrings.TileFootprint_MenuAddress)]
public class TileFootprint : ScriptableObject
{
    public int TileCount => Footprint.Length;

    public int Width, Height;
    public bool[] Footprint;

    public int[] Entrances;

    public int[] IndexToXY(int index)
    {
        var returnArray = new int[] 
        {
            index % Width,
            index / Width
        };

        return returnArray;
    }
}