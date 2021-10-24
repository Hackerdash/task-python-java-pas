using System.Linq;
using System.Runtime.CompilerServices;

namespace JeuDeLaVie
{
    public static class ArrayGPS
    {
        private static int cMem, cMemAncient;
        private static int[] ancientMemPyramid;
        private static bool[] ancientMemPyramidBool;

        public static int SwapTablesNew { get; private set; } = 1;
        public static int SwapTablesOld { get; private set; } = 0;
        public static int SwapTablesNewB { get; private set; } = 1;

        public static void BackupTablesNumbers()
        {
            SwapTablesNewB = SwapTablesNew;
        }

        public static void CycleAdd()
        {
            SwapTablesOld = SwapTablesNew;
            do
            {
                SwapTablesNew = (SwapTablesNew >= cMem - 1) ? 0 : SwapTablesNew + 1;
            } while (ancientMemPyramid.Contains(SwapTablesNew));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void pushAncient1()
        {
            if (ancientMemPyramidBool[0])
            {
                pushAncient(ancientMemPyramid[0], 1);
                ancientMemPyramid[0] = SwapTablesNewB;
                ancientMemPyramidBool[0] = false;
            }
            else
            {
                ancientMemPyramid[0] = SwapTablesNewB;
                ancientMemPyramidBool[0] = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void pushAncient(int swap, int id = 0)
        {
            if (ancientMemPyramidBool[id])
            {
                ancientMemPyramidBool[id] = false;
                if (id < cMemAncient - 1)
                {
                    pushAncient(ancientMemPyramid[id], id + 1);
                }
                ancientMemPyramid[id] = swap;
            }
            else
            {
                ancientMemPyramid[id] = swap;
                ancientMemPyramidBool[id] = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CycleReset(int cycleMemory, int nbAncient)
        {
            SwapTablesNew = 1;
            SwapTablesOld = 0;
            BackupTablesNumbers();
            cMemAncient = nbAncient;
            cMem = cycleMemory;

            ancientMemPyramid = new int[nbAncient];
            ancientMemPyramidBool = new bool[nbAncient];
            for(int i=0; i<nbAncient; i++)
            {
                ancientMemPyramidBool[i] = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CycleEmulateNew()
        {
            int r = SwapTablesNewB;
            do {
                r = (r >= cMem - 1) ? 0 : r + 1;
            } while (ancientMemPyramid.Contains((r >= cMem - 1) ? 0 : r + 1)) ;
            return r;
        }
    }
}
