using System;

namespace JeuDeLaVie
{
    public class StructureTemplate
    {
        public byte[,] StructureMap { get; }
        public string Id { get; }

        public StructureTemplate(byte[,] map, string id)
        {
            Id = id;
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            StructureMap = (byte[,])map.Clone();
        }

        public byte? getValue(int directionFrom,int x,int y, bool flipped=false)
        {
            if (!flipped)
            {
                switch (directionFrom)
                {
                    default:
                    case 0://north
                        if (x >= StructureMap.GetLength(0) || y >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;
                        return StructureMap[x, y];
                    case 1://east
                        if (y >= StructureMap.GetLength(0) || x >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;

                        return StructureMap[StructureMap.GetLength(0)-1-y, x];
                    case 2://south
                        if (x >= StructureMap.GetLength(0) || y >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;

                        return StructureMap[StructureMap.GetLength(0)-1 - x, StructureMap.GetLength(1)-1 - y];
                    case 3://west
                        if (y >= StructureMap.GetLength(0) || x >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;

                        return StructureMap[y, StructureMap.GetLength(1) - 1 - x];
                }
            }
            else//flip model
            {
                switch (directionFrom)
                {
                    default:
                    case 0://north
                        if (x >= StructureMap.GetLength(0) || y >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;
                        return StructureMap[StructureMap.GetLength(0) - 1 - x, y];
                    case 1://east
                        if (y >= StructureMap.GetLength(0) || x >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;

                        return StructureMap[y, x];
                    case 2://south
                        if (x >= StructureMap.GetLength(0) || y >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;

                        return StructureMap[x, StructureMap.GetLength(1) - 1 - y];
                    case 3://west
                        if (y >= StructureMap.GetLength(0) || x >= StructureMap.GetLength(1) || y < 0 || x < 0)
                            return null;

                        return StructureMap[StructureMap.GetLength(0)-1-y, StructureMap.GetLength(1) - 1 - x];
                }
            }
        }

        public int getWidth(int d)
        {
            if(d==0 || d==2)
                return StructureMap.GetLength(0);
            else
                return StructureMap.GetLength(1);
        }

        public int getHeight(int d)
        {
            if (d == 0 || d == 2)
                return StructureMap.GetLength(1);
            else
                return StructureMap.GetLength(0);
        }
    }
}
