using UnityEngine;

[CreateAssetMenu(fileName = EditorStrings.TileFootprint_FileName, menuName = EditorStrings.TileFootprint_MenuAddress)]
public class TileFootprint : ScriptableObject
{
    public enum RotationOp
    {
        Rot90,
        Rot180,
        Rot270
    }
    
    public int Size;
    public bool[] Footprint;

    public int[] Entrances;
    
    public int TileCount => Footprint.Length;
    
    public int TileSize => Size * Size;

    public (int,int) IndexToXY(int index) => (index % Size, index / Size);
    
    public void RotateFootprint(RotationOp rotationOp)
    {
        var size = (int)Mathf.Sqrt(Footprint.Length);
        var result = new bool[Footprint.Length];

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                // 90 degrees
                if(rotationOp == RotationOp.Rot90)
                    result[i * size + j] = Footprint[(size - j - 1) * size + i];
                
                // 180 degrees
                if(rotationOp == RotationOp.Rot180)
                    result[i * size + j] = Footprint[(size - i - 1) * size + (size - j - 1)];
                
                // 270 degrees
                if(rotationOp == RotationOp.Rot270)
                    result[i * size + j] = Footprint[j * size + (size - i - 1)];
            }
        }

        Footprint = result;
    }    
}