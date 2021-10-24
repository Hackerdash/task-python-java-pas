using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace JeuDeLaVie
{
    public static class JeuDeLaVieTable
    {
        private static int _tailleX, _tailleY, _cycleMemory, _cycleStateAncient = 0, cycleSummary = 0, _memoryDistance, tailleYMinus, tailleXMinus, threadNumber = 4;
        private static int[] cycleSummaries;
        private static int[,] cycleRowSummaries;
        private static Random generateur;
        private static bool structureNature = true, _verifyStale;
        private static StructureManager StructureMgr;
        private static byte[][,] TableauDeLaVie;
        private static int[][][] cycleMapTable1;
        private static int IndexStorageSwapNew = 4, IndexStorageSwapNow = 5, IndexStorageSwapOld = 6;
        private static byte[,] tempTableauDeLaVieOld, tempTableauDeLaVieNew;
        private static int[][] threadYCords;
        private static Thread[] threadArrayTable;

        public static Color[] DonneeTables { get; private set; }
        public static bool Stale { get; private set; } = false;
        public static int StaleCycle { get; private set; } = 0;
        public static bool AffichageChangement { get; set; }
        public static Color[] teamColors = { Color.Blue, Color.Red, Color.Chocolate, Color.White};

        static JeuDeLaVieTable()
        {
            StructureMgr = new StructureManager();
            generateur = new Random();
        }
        public static void GenerateNew(int memoryDistance = 1000, int nbAncientSummaries = 9, bool affichageChangement = false, double probabilite = 0.02, int tailleX = 800, int tailleY = 600, bool staleProof = false, bool verifyStale = false)
        {
            _verifyStale = verifyStale;

            if (nbAncientSummaries < 1)
                nbAncientSummaries = 1;
            _memoryDistance = memoryDistance;
            _cycleMemory = nbAncientSummaries + 2;
            ArrayGPS.CycleReset(nbAncientSummaries + 2, nbAncientSummaries);
            AffichageChangement = affichageChangement;
            TableauDeLaVie = new byte[nbAncientSummaries + 2][,];
            for(int i=0; i < nbAncientSummaries + 2; i++)
            {
                TableauDeLaVie[i] = new byte[tailleX, tailleY];
            }
            cycleMapTable1 = new int[tailleY][][];
            cycleSummaries = new int[nbAncientSummaries + 2];
            cycleRowSummaries = new int[nbAncientSummaries + 2, tailleY];
            _tailleX = tailleX;
            _tailleY = tailleY;
            DonneeTables = new Color[_tailleX * _tailleY];
            threadYCords = new int[threadNumber][];
            threadArrayTable = new Thread[threadNumber];
            //testcell threads math V2
            for (int i=0, yRange=0, memRange=0; i<threadNumber; i++, yRange+=(tailleY/threadNumber), memRange += (_cycleMemory / threadNumber))
            {
                threadYCords[i] = new int[5];

                threadYCords[i][0] = yRange;
                threadYCords[i][2] = yRange * _tailleX;
                threadYCords[i][3] = memRange;
                if (i == threadNumber - 1)
                {
                    threadYCords[i][1] = tailleY;
                    threadYCords[i][4] = _cycleMemory;
                }
                else
                {
                    threadYCords[i][1] = yRange + (tailleY / threadNumber);
                    threadYCords[i][4] = memRange + (_cycleMemory / threadNumber);
                }
            }

            tailleYMinus = _tailleY - 1;
            tailleXMinus = _tailleX - 1;
            Stale = false;

            //instancie le tableau de valeurs
            for (int y = 0; y < tailleY; y++)
            {
                cycleMapTable1[y] = new int[tailleX][];
                for (int x = 0; x < tailleX; x++)
                {
                    cycleMapTable1[y][x] = new int[7];
                }

            }

            for (int y = 0; y < tailleY; y++)
            {
                for (int x = 0; x < tailleX; x++)
                {
                    bool yMax = y == tailleYMinus, xMax = x == tailleXMinus, yZero = y == 0, xZero = x == 0;
                    int yMinus = y - 1, yPlus = y + 1, xMinus = x - 1, xPlus = x + 1;

                    cycleMapTable1[y][x][0] = xZero ? tailleXMinus : xMinus;
                    cycleMapTable1[y][x][1] = yZero ? tailleYMinus : yMinus;
                    cycleMapTable1[y][x][2] = xMax ? 0 : xPlus;
                    cycleMapTable1[y][x][3] = yMax ? 0 : yPlus;

                    if (structureNature)
                    {
                        bool r = (generateur.NextDouble() <= 0.5);
                        int direction = generateur.Next(0, 4);
                        double selectedIndex = ((double)generateur.Next(0, 100000)) / 100000, templatesSum = StructureMgr.StructureTemplatesNature.Sum(i => i.percentageChance);

                        if (selectedIndex < templatesSum)
                        {
                            double summaryT = 0;
                            StructureTemplateNature i = null;

                            for (int g = 0; i == null && g < StructureMgr.StructureTemplatesNature.Count; g++)
                            {
                                summaryT += StructureMgr.StructureTemplatesNature[g].percentageChance;
                                if (summaryT > selectedIndex)
                                {
                                    i = StructureMgr.StructureTemplatesNature[g];
                                }
                            }

                            if (i != null && i.percentageChance > selectedIndex)
                            {
                                for (int f = 0; f < i.getHeight(direction); f++)
                                {
                                    for (int g = 0; g < i.getWidth(direction); g++)
                                    {
                                        if (i.getValue(direction, g, f, r) > 0)
                                            setLife(g + x - i.getWidth(direction) / 2, f + y - i.getHeight(direction) / 2, i.getValue(direction, g, f, r) ?? 0);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        //todo
                    }
                }
            }

            //crée une copie initiale pour pas que l'affichage initial crée un erreur
            ArrayGPS.BackupTablesNumbers();
            generateImage();
        }


        public static void generateImage()
        {
            for (int y = 0, yByXtotal = 0; y < _tailleY; y++, yByXtotal += _tailleX)
            {
                for (int x = 0; x < _tailleX; x++)
                {
                    if (TableauDeLaVie[ArrayGPS.SwapTablesNew][x, y] == 1)
                        DonneeTables[yByXtotal + x] = Color.Black;
                    if (TableauDeLaVie[ArrayGPS.SwapTablesNew][x, y] == 2)
                        DonneeTables[yByXtotal + x] = Color.Yellow;
                    if (TableauDeLaVie[ArrayGPS.SwapTablesNew][x, y] == 63)
                        DonneeTables[yByXtotal + x] = Color.Purple;
                }
            }
        }

        public static void setLife(int x, int y, byte value = 1)
        {
            int rX = x < 0 ? _tailleX + x : (x >= _tailleX ? x - _tailleX : x),
                rY = y < 0 ? _tailleY + y : (y >= _tailleY ? y - _tailleY : y),
                nbYBackY = rY * _tailleX;
            if (value == 253)
            {
                TableauDeLaVie[ArrayGPS.SwapTablesNew][rX, rY] = 0;
            }else if(value>=100 && value < 200)
            {
                cycleMapTable1[rY][rX][IndexStorageSwapNew] = value;
                cycleMapTable1[rY][rX][IndexStorageSwapNow] = value;
                cycleMapTable1[rY][rX][IndexStorageSwapOld] = value;
                TableauDeLaVie[ArrayGPS.SwapTablesNew][rX, rY] = 255;
            }
            else
            {
                TableauDeLaVie[ArrayGPS.SwapTablesNew][rX, rY] = value;
            }
        }
        
        public static void CalculerCycleThread(object data)
        {
            int[] intData = (int[])data, tZero, tOne;
            for (int y = intData[0], nbYBackY = intData[2]; y < intData[1]; y++, nbYBackY += _tailleX)
            {
                int[][] tempTableY = cycleMapTable1[y];
                int cycleXRowSummary = 0;
                for (int x = 0; x < _tailleX; x++)
                {
                    int[] tempTableX = tempTableY[x];
                    int cellSummary = tempTableauDeLaVieOld[tempTableX[0], tempTableX[1]] +
                                    tempTableauDeLaVieOld[x, tempTableX[1]] +
                                    tempTableauDeLaVieOld[tempTableX[2], tempTableX[1]] +
                                    tempTableauDeLaVieOld[tempTableX[0], y] +
                                    tempTableauDeLaVieOld[tempTableX[2], y] +
                                    tempTableauDeLaVieOld[tempTableX[0], tempTableX[3]] +
                                    tempTableauDeLaVieOld[x, tempTableX[3]] +
                                    tempTableauDeLaVieOld[tempTableX[2], tempTableX[3]], cellSummary2 = cellSummary % 255 + cellSummary / 255, oldTableL = tempTableauDeLaVieOld[x, y];
                    
                    //Choice
                    Color c = Color.Black;
                    byte intWeight = 1;
                    
                    if (oldTableL == 2)
                    {
                        if (cellSummary2 < 4 || (cellSummary2 >= 12 && cellSummary2 < 17))
                        {
                            DonneeTables[nbYBackY + x] = Color.Transparent;
                            tempTableauDeLaVieNew[x, y] = 0;
                            tempTableX[IndexStorageSwapNew] = 0;
                        }
                        else
                        {
                            cycleXRowSummary++;
                            DonneeTables[nbYBackY + x] = Color.Yellow;
                            tempTableauDeLaVieNew[x, y] = 2;
                        }
                    }
                    else if (oldTableL == 63)
                    {
                        DonneeTables[nbYBackY + x] = Color.Purple;
                        tempTableauDeLaVieNew[x, y] = 63;
                        cycleXRowSummary++;
                    }
                    else
                    {
                        if (cellSummary2 == 3 || (cellSummary2 == 2 && oldTableL > 0))
                        {                            
                            if (cellSummary > 255)
                            {
                                tZero = new int[8] {
                                cycleMapTable1[tempTableX[1]][tempTableX[0]][IndexStorageSwapOld],
                                cycleMapTable1[tempTableX[1]][x][IndexStorageSwapOld],
                                cycleMapTable1[tempTableX[1]][tempTableX[2]][IndexStorageSwapOld],
                                tempTableY[tempTableX[0]][IndexStorageSwapOld],
                                tempTableY[tempTableX[2]][IndexStorageSwapOld],
                                cycleMapTable1[tempTableX[3]][tempTableX[0]][IndexStorageSwapOld],
                                cycleMapTable1[tempTableX[3]][x][IndexStorageSwapOld],
                                cycleMapTable1[tempTableX[3]][tempTableX[2]][IndexStorageSwapOld]
                                };
                                tOne = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                                for (int k = 0; k < 8; k++)
                                {
                                    if (tZero[k] > 0)
                                    {
                                        for (int l = 0; l < 8; l++)
                                        {
                                            if (tZero[l] == tZero[k] && l != k)
                                            {
                                                tOne[k]++;
                                                tOne[l] = 0;
                                            }
                                        }
                                    }
                                    if (k > 0 && tOne[k] > 0)
                                    {
                                        if (tOne[0] < tOne[k])
                                        {
                                            tOne[0] = tOne[k];
                                            tZero[0] = tZero[k];
                                            tOne[k] = 0;
                                            tZero[k] = 0;
                                        }
                                        else if (tOne[0] == tOne[k] && k != 1)
                                        {
                                            tOne[1] = tOne[k];
                                            tZero[1] = tZero[k];
                                            tOne[k] = 0;
                                            tZero[k] = 0;
                                        }
                                    }
                                }

                                if (tOne[1] != tOne[0])
                                {
                                    tempTableX[IndexStorageSwapNew] = tZero[0];
                                    tempTableX[IndexStorageSwapNow] = tZero[0];
                                    intWeight = 255;
                                    c = teamColors[tZero[0] - 100];
                                }                              
                            }
                            else if(tempTableX[IndexStorageSwapNow] >= 100 && tempTableX[IndexStorageSwapNow] < 200)
                            {
                                intWeight = 255;
                                c = teamColors[tempTableX[IndexStorageSwapNow] - 100];
                                tempTableX[IndexStorageSwapNew] = tempTableX[IndexStorageSwapNow];
                            }
                            cycleXRowSummary++;
                            DonneeTables[nbYBackY + x] = c;
                            tempTableauDeLaVieNew[x, y] = intWeight;
                        }
                        else
                        if (cellSummary2 > 10)
                        {
                            cycleXRowSummary++;
                            DonneeTables[nbYBackY + x] = Color.Yellow;
                            tempTableauDeLaVieNew[x, y] = 2;
                        }
                        else
                        {
                            tempTableauDeLaVieNew[x, y] = 0;
                            tempTableX[IndexStorageSwapNew] = 0;
                            DonneeTables[nbYBackY + x] = Color.Transparent;
                        }
                    }
                }
                cycleSummary += cycleXRowSummary;
                cycleRowSummaries[ArrayGPS.SwapTablesNew, y] = cycleXRowSummary;
            }

            //checkout
            if (!_verifyStale) {
                for (int oStart = intData[3]; oStart < intData[4]; oStart++)
                {
                    if (cycleSummaries[oStart] == cycleSummaries[ArrayGPS.SwapTablesNewB] && oStart != ArrayGPS.SwapTablesNewB && oStart != ArrayGPS.SwapTablesNew)
                    {
                        //if there's a match, looks deeper into it
                        bool cancelledLookup = false;

                        for (int i = 0; i < _tailleY; i++)
                        {
                            if (cycleRowSummaries[oStart, i] != cycleRowSummaries[ArrayGPS.SwapTablesNewB, i])
                            {
                                cancelledLookup = true;
                                break;
                            }
                        }

                        if (!cancelledLookup)
                        {
                            byte[,] TableauDeLaVieoStart = TableauDeLaVie[oStart], TableauDeLaVieNewB = TableauDeLaVie[ArrayGPS.SwapTablesNewB];
                            for (int y = 0; y < _tailleY; y++)
                            {
                                for (int x = 0; x < _tailleX; x++)
                                {
                                    if (TableauDeLaVieNewB[x, y] != TableauDeLaVieoStart[x, y])
                                    {
                                        cancelledLookup = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (cancelledLookup)
                        {
                            continue;
                        }
                        else
                        {
                            Stale = true;
                            StaleCycle = 0;
                            int i = 0;
                            for (int oToNew = oStart; oToNew != ArrayGPS.SwapTablesNewB; oToNew++, i++)
                            {
                                if (oToNew == _cycleMemory - 1)
                                {
                                    oToNew = -1;
                                }
                                i++;
                            }
                            StaleCycle = _memoryDistance * (2 ^ (i - 3)) + _cycleStateAncient;//bad calcul doing later
                        }
                    }
                }
            }
        }

        public static void CalculerCycle()
        {
            //thread calcule old
            ArrayGPS.BackupTablesNumbers();
            //translate les tables utilises vers le haut
            ArrayGPS.CycleAdd();

            int tempSwap = IndexStorageSwapNew;
            IndexStorageSwapNew = IndexStorageSwapOld;
            IndexStorageSwapOld = IndexStorageSwapNow;
            IndexStorageSwapNow = tempSwap;

            _cycleStateAncient++;
            if (_cycleStateAncient == _memoryDistance)
            {
                ArrayGPS.pushAncient1();
                _cycleStateAncient = 0;
            }

            cycleSummary = 0;
            //calcule le nombre de cellule adjascent
            //divide by 4 thread
            tempTableauDeLaVieOld = TableauDeLaVie[ArrayGPS.SwapTablesOld];
            tempTableauDeLaVieNew = TableauDeLaVie[ArrayGPS.SwapTablesNew];

            for (int i=0; i<threadNumber; i++)
            {
                threadArrayTable[i] = new Thread(CalculerCycleThread);
                threadArrayTable[i].Start(threadYCords[i]);
            }

            for (int i = 0; i < threadNumber; i++)
            {
                threadArrayTable[i].Join();
            }

            //set the cycle summaries in case there's a match
            cycleSummaries[ArrayGPS.SwapTablesNew] = cycleSummary;
        }
    }
}