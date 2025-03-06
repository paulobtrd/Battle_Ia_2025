using System;
using System.Collections.Generic;
using BattleIA;


public class FeederIA
{

    // Pour faire des tirages de nombres aléatoires
    Random rand = new Random();
    // Pour détecter si c'est le tout premier tour du jeu
    bool isFirstTime;
    // mémorisation du niveau du bouclier de protection
    UInt16 currentShieldLevel;
    // variable qui permet de savoir si le bot a été touché ou non
    bool hasBeenHit;

    const int hauteurMap = 100;
    const int largeurMap = 100;
    
    const int scanEnergyPercent = 15;

    // Initialisation des variables de coord du bot, et du tableau map[,]
    int[,,] map = new int[2*largeurMap+1, 2*hauteurMap+1, 2];
    int x = largeurMap/2;
    int y = hauteurMap/2;

    UInt16 energyLevel = 0;

    byte ScanSize = 0;

    private int lastDirection = 0;


    public void consoleForeColorWrite(string couleur, string texte){
        switch (couleur){
            case "Red" :        if(Console.ForegroundColor != ConsoleColor.Red)     Console.ForegroundColor = ConsoleColor.Red; break;
            case "Magenta" :    if(Console.ForegroundColor != ConsoleColor.Magenta) Console.ForegroundColor = ConsoleColor.Magenta; break;
            case "Blue" :       if(Console.ForegroundColor != ConsoleColor.Blue)    Console.ForegroundColor = ConsoleColor.Blue; break;
            case "Cyan" :       if(Console.ForegroundColor != ConsoleColor.Cyan)    Console.ForegroundColor = ConsoleColor.Cyan; break;
            case "Green" :      if(Console.ForegroundColor != ConsoleColor.Green)   Console.ForegroundColor = ConsoleColor.Green; break;
            case "Yellow" :     if(Console.ForegroundColor != ConsoleColor.Yellow)  Console.ForegroundColor = ConsoleColor.Yellow; break;
            case "DarkRed" :    if(Console.ForegroundColor != ConsoleColor.DarkRed) Console.ForegroundColor = ConsoleColor.DarkRed; break;
            case "DarkMagenta" :if(Console.ForegroundColor != ConsoleColor.DarkMagenta) Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
            case "DarkBlue" :   if(Console.ForegroundColor != ConsoleColor.DarkBlue)    Console.ForegroundColor = ConsoleColor.DarkBlue; break;
            case "DarkGreen" :  if(Console.ForegroundColor != ConsoleColor.DarkGreen)   Console.ForegroundColor = ConsoleColor.DarkGreen; break;
            case "DarkYellow" : if(Console.ForegroundColor != ConsoleColor.DarkYellow)  Console.ForegroundColor = ConsoleColor.DarkYellow; break;
            case "White" :      if(Console.ForegroundColor != ConsoleColor.White)   Console.ForegroundColor = ConsoleColor.White; break;
            case "DarkGray" :   if(Console.ForegroundColor != ConsoleColor.DarkGray)Console.ForegroundColor = ConsoleColor.DarkGray; break;
            case "Black" :      if(Console.ForegroundColor != ConsoleColor.Black)   Console.ForegroundColor = ConsoleColor.Yellow; break;
            default :           if(Console.ForegroundColor != ConsoleColor.Gray)    Console.ForegroundColor = ConsoleColor.Gray; break;
        }
        Console.Write(texte);
    }


    public void DoInit()
    {
        isFirstTime = true;
        currentShieldLevel = 0;
        hasBeenHit = false;
        lastDirection = 0;
    }

    // ****************************************************************************************************
    /// Réception de la mise à jour des informations du bot
    public void StatusReport(UInt16 turn, UInt16 energy, UInt16 shieldLevel, UInt16 cloakLevel)
    {
        energyLevel = energy;
        // Si le niveau du bouclier a baissé, c'est que l'on a reçu un coup
        if (currentShieldLevel != shieldLevel)
        {
            currentShieldLevel = shieldLevel;
            hasBeenHit = true;
        }
    }

    // --- A* : Recherche du chemin vers la plus proche cellule d'énergie ---
    private class Node
    {
        public int X, Y;
        public int G; // Coût depuis le départ.
        public int H; // Coût heuristique.
        public int F { get { return G + H; } }
        public Node Parent;
 
        public Node(int x, int y)
        {
            X = x;
            Y = y;
            G = 0;
            H = 0;
            Parent = null;
        }
    }
 
    private List<Node> FindPathToNearestEnergy()
    {
        (int minX, int maxX, int minY, int maxY) = GetSearchBounds();
        List<(int X, int Y)> goals = GetEnergyGoals(minX, maxX, minY, maxY);
        if (goals.Count == 0)
            return null;
 
        List<Node> openList = new List<Node>();
        bool[,] closed = new bool[largeurMap, hauteurMap];
        Node start = new Node(x, y)
        {
            G = 0,
            H = CalculateHeuristic(x, y, goals)
        };
        openList.Add(start);
 
        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            Node current = openList[0];
            openList.RemoveAt(0);
            closed[current.X, current.Y] = true;
 
            // Si une cellule d'énergie est atteinte, on reconstruit le chemin.
            if (map[current.X, current.Y, 0] == 4)
                return ReconstructPath(current);
 
            ExpandNeighbors(current, openList, closed, goals, minX, maxX, minY, maxY);
        }
        return null;
    }
 
    private (int minX, int maxX, int minY, int maxY) GetSearchBounds()
    {
        int d = (ScanSize - 1) / 2;
        int minX = Math.Max(0, x - d);
        int maxX = Math.Min(largeurMap - 1, x + d);
        int minY = Math.Max(0, y - d);
        int maxY = Math.Min(hauteurMap - 1, y + d);
        return (minX, maxX, minY, maxY);
    }
 
    private List<(int X, int Y)> GetEnergyGoals(int minX, int maxX, int minY, int maxY)
    {
        List<(int X, int Y)> goals = new List<(int, int)>();
        for (int j = minY; j <= maxY; j++)
        {
            for (int i = minX; i <= maxX; i++)
            {
                if (map[i, j, 0] == 4)
                    goals.Add((i, j));
            }
        }
        return goals;
    }
 
    private int CalculateHeuristic(int i, int j, List<(int X, int Y)> goals)
    {
        int min = int.MaxValue;
        foreach (var goal in goals)
        {
            int dist = Math.Abs(i - goal.X) + Math.Abs(j - goal.Y);
            if (dist < min)
                min = dist;
        }
        return min;
    }
 
    private void ExpandNeighbors(Node current, List<Node> openList, bool[,] closed, List<(int X, int Y)> goals, int minX, int maxX, int minY, int maxY)
    {
        int[] dx = { 0, -1, 0, 1 };
        int[] dy = { -1, 0, 1, 0 };
        for (int i = 0; i < 4; i++)
        {
            int newX = current.X + dx[i];
            int newY = current.Y + dy[i];
            if (newX < minX || newX > maxX || newY < minY || newY > maxY)
                continue;
            if (map[newX, newY, 0] <= 2)
                continue;
            if (closed[newX, newY])
                continue;
 
            int tentativeG = current.G + 1;
            Node neighbor = openList.Find(n => n.X == newX && n.Y == newY);
            if (neighbor == null)
            {
                neighbor = new Node(newX, newY)
                {
                    G = tentativeG,
                    H = CalculateHeuristic(newX, newY, goals),
                    Parent = current
                };
                openList.Add(neighbor);
            }
            else if (tentativeG < neighbor.G)
            {
                neighbor.G = tentativeG;
                neighbor.Parent = current;
            }
        }
    }
 
    private List<Node> ReconstructPath(Node node)
    {
        List<Node> path = new List<Node>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
 
    // --- Fonctions utilitaires de déplacement ---
    private int OppositeDirection(int d)
    {
        switch (d)
        {
            case 1: return 3; // Nord <-> Sud
            case 2: return 4; // Ouest <-> Est
            case 3: return 1;
            case 4: return 2;
            default: return 0;
        }
    }
 
    private bool IsAccessible(int d)
    {
        switch (d)
        {
            case 1: return (y - 1 >= 0 && map[x, y - 1, 0] > 2);
            case 2: return (x - 1 >= 0 && map[x - 1, y, 0] > 2);
            case 3: return (y + 1 < hauteurMap && map[x, y + 1, 0] > 2);
            case 4: return (x + 1 < largeurMap && map[x + 1, y, 0] > 2);
            default: return false;
        }
    }
 
    private int CheckAdjacentEnergy()
    {
        if (y - 1 >= 0 && map[x, y - 1, 0] == 4)
        {
            y--;
            lastDirection = 1;
            return 1;
        }
        if (x + 1 < largeurMap && map[x + 1, y, 0] == 4)
        {
            x++;
            lastDirection = 4;
            return 4;
        }
        if (x - 1 >= 0 && map[x - 1, y, 0] == 4)
        {
            x--;
            lastDirection = 2;
            return 2;
        }
        if (y + 1 < hauteurMap && map[x, y + 1, 0] == 4)
        {
            y++;
            lastDirection = 3;
            return 3;
        }
        return 0;
    }
 
    private int GetDirectionFromDelta(int dx, int dy)
    {
        if (dx == 1) return 4;
        if (dx == -1) return 2;
        if (dy == 1) return 3;
        if (dy == -1) return 1;
        return 0;
    }
 
    private int GetRandomAccessibleDirection()
    {
        List<int> possibles = new List<int>();
        if (y - 1 >= 0 && map[x, y - 1, 0] > 2) possibles.Add(1);
        if (x + 1 < largeurMap && map[x + 1, y, 0] > 2) possibles.Add(4);
        if (x - 1 >= 0 && map[x - 1, y, 0] > 2) possibles.Add(2);
        if (y + 1 < hauteurMap && map[x, y + 1, 0] > 2) possibles.Add(3);
        if (possibles.Count > 0)
            return possibles[rand.Next(possibles.Count)];
        return 0;
    }
 
    private void UpdatePosition(int direction)
    {
        switch (direction)
        {
            case 1: y--; break;
            case 2: x--; break;
            case 3: y++; break;
            case 4: x++; break;
        }
    }
 
    // Détermine la meilleure direction à suivre.
    public int BestDir()
    {
        // 1. Vérification immédiate des cases adjacentes.
        int adjacentDir = CheckAdjacentEnergy();
        if (adjacentDir != 0)
            return adjacentDir;
 
        // 2. Recherche via A*.
        List<Node> path = FindPathToNearestEnergy();
        int bestDirection = 0;
        if (path != null && path.Count > 1)
        {
            Node nextStep = path[1];
            bestDirection = GetDirectionFromDelta(nextStep.X - x, nextStep.Y - y);
        }
        else
        {
            bestDirection = GetRandomAccessibleDirection();
        }
 
        // 3. Éviter le demi-tour immédiat.
        if (lastDirection != 0 && bestDirection == OppositeDirection(lastDirection))
        {
            if (IsAccessible(lastDirection))
                bestDirection = lastDirection;
        }
 
        UpdatePosition(bestDirection);
        lastDirection = bestDirection;
        return bestDirection;
    }

    // ****************************************************************************************************
    /// On doit effectuer une action
    public byte[] GetAction()
    {
        if (hasBeenHit)
        {
            return HandleHit();
        }
        if (currentShieldLevel == 0)
        {
            currentShieldLevel = 1;
            return BotHelper.ActionShield(currentShieldLevel);
        }
        return BotHelper.ActionMove((MoveDirection)BestDir());
    }
 
    private byte[] HandleHit()
    {
        if (currentShieldLevel == 0)
        {
            currentShieldLevel = (byte)rand.Next(1, 9);
            return BotHelper.ActionShield(currentShieldLevel);
        }
        hasBeenHit = false;
        return BotHelper.ActionMove((MoveDirection)BestDir());
    }


    // ****************************************************************************************************
    /// On nous demande la distance de scan que l'on veut effectuer
    public byte GetScanSurface()    //max 31
    {
        if (isFirstTime)
        {
            ScanSize = 15*2+1;      // La toute première fois, le bot fait un scan d'une surface de 15 cases autour de lui
            isFirstTime = false;
        } else {
            ScanSize = Convert.ToByte(((energyLevel*scanEnergyPercent)/100) *2 +1);
            switch (ScanSize)
            {
                case >30 *2+1: ScanSize=30 *2+1;  break;
                case <4  *2+1: ScanSize=4 *2+1;   break;
            }
        }
        return Convert.ToByte((ScanSize-1)/2);
    }

    public int[] BestQuatre(int scanSize)
    {
        int sum = 0;
        int[] bestZone = new int[4];
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);
 
        // Quadrant en haut à gauche
        for (int i = Math.Max(0, x - (scanSize - 1) / 2); i < Math.Min(mapWidth, x + 1); i++)
        {
            for (int j = Math.Max(0, y - (scanSize - 1) / 2); j < Math.Min(mapHeight, y); j++)
            {
                if (map[i, j, 0] == 4)
                    sum++;
            }
        }
        bestZone[0] = sum;
        Console.WriteLine($"En haut à gauche : {bestZone[0]}");
        sum = 0;
 
        // Quadrant en haut à droite
        for (int i = Math.Max(0, x + 1); i < Math.Min(mapWidth, x + (scanSize - 1) / 2 + 1); i++)
        {
            for (int j = Math.Max(0, y - (scanSize - 1) / 2); j < Math.Min(mapHeight, y + 1); j++)
            {
                if (map[i, j, 0] == 4)
                    sum++;
            }
        }
        bestZone[1] = sum;
        Console.WriteLine($"En haut à droite : {bestZone[1]}");
        sum = 0;
 
        // Quadrant en bas à gauche
        for (int i = Math.Max(0, x - (scanSize - 1) / 2); i < Math.Min(mapWidth, x); i++)
        {
            for (int j = Math.Max(0, y); j < Math.Min(mapHeight, y + (scanSize - 1) / 2 + 1); j++)
            {
                if (map[i, j, 0] == 4)
                    sum++;
            }
        }
        bestZone[2] = sum;
        Console.WriteLine($"En bas à gauche : {bestZone[2]}");
        sum = 0;
 
        // Quadrant en bas à droite
        for (int i = Math.Max(0, x); i < Math.Min(mapWidth, x + (scanSize - 1) / 2 + 1); i++)
        {
            for (int j = Math.Max(0, y + 1); j < Math.Min(mapHeight, y + (scanSize - 1) / 2 + 1); j++)
            {
                if (map[i, j, 0] == 4)
                    sum++;
            }
        }
        bestZone[3] = sum;
        Console.WriteLine($"En bas à droite : {bestZone[3]}");
 
        return bestZone;
    }

    // ****************************************************************************************************
    /// Résultat du scan
    public void AreaInformation(byte distance, byte[] informations)
    {
        ScanSize = distance;

        Console.WriteLine($"\nScan d'un diametre de {distance} : ");
        int index = 0;
        for (int j = 0; j < distance; j++)
        {
            for (int i = 0; i < distance; i++)
            {
                switch ((CaseState)informations[index++])
                {
                    case CaseState.Empty: Console.Write("·"); break;
                    case CaseState.Energy: Console.Write("Φ"); break;
                    case CaseState.Ennemy: Console.Write("*"); break;
                    case CaseState.Wall: Console.Write("█"); break;
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        mapEnergyAgeMaj();

        index = 0;
        for (int j = y-(distance-1)/2; j <= y+(distance-1)/2; j++)
        {
            for (int i = x-(distance-1)/2; i <= x+(distance-1)/2; i++)
            {
                if(j>=0 && i>=0 && j<hauteurMap && i<largeurMap) {
                    switch ((CaseState)informations[index])
                    {
                        case CaseState.Wall:    map[i,j,0] = 1;  map[i,j,1] = 0;   break;
                        case CaseState.Ennemy:  map[i,j,0] = 2;  map[i,j,1] = 0;   break;
                        case CaseState.Empty:   map[i,j,0] = 3;  map[i,j,1] = 0;   break;
                        case CaseState.Energy:  map[i,j,0] = 4;  map[i,j,1] = 1;   break;
                    }
                index++;
                }
            }
        }

        mapPrint(0,true);
        mapPrint(1,true);
    }

    public void mapEnergyAgeMaj(){
        for (int j = 0; j < hauteurMap; j++)
        {
            for (int i = 0; i < largeurMap ; i++)
            {
                if(map[i,j,1]!=0){
                    map[i,j,1]++;
                }
            }
        }
    }

    public void mapPrint(int type, bool showScan)
    {
        Console.WriteLine($"\nImpression de la couche {type} de la carte (de taille {hauteurMap}x{largeurMap}) : ");
        for (int j = 0; j < hauteurMap; j++)
        {
            for (int i = 0; i < largeurMap ; i++)
            {
                if(showScan){
                    if(x-(ScanSize-1)/2 <= i && i <= x+(ScanSize-1)/2  &&  y-(ScanSize-1)/2 <=j && j <= y+(ScanSize-1)/2){
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    } else {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                

                if (i == x && j == y) {  
                    if(Console.BackgroundColor != ConsoleColor.DarkYellow){consoleForeColorWrite("Blue","X");} else{consoleForeColorWrite("DarkBlue","X");}
                }
                else {
                    if (type==0){
                        switch (map[i,j,0])
                        {
                            case 0:  consoleForeColorWrite("DarkGray","?"); break; 
                            case 1: if(Console.BackgroundColor != ConsoleColor.DarkYellow){consoleForeColorWrite("DarkGray","█");}else{ consoleForeColorWrite("Gray","█");}  break; 
                            case 2:  consoleForeColorWrite("Red","*"); break; 
                            case 3: consoleForeColorWrite("DarkGray","."); break;
                            case 4: consoleForeColorWrite("Yellow","Φ"); break;
                        }
                    } else {
                        switch (map[i,j,1])
                        {
                            case 0: Console.Write(" ");  break; 
                            case 1: consoleForeColorWrite("Yellow",Convert.ToString(map[i,j,type])); break;
                            case 2: consoleForeColorWrite("White",Convert.ToString(map[i,j,type])); break;
                            case 3: consoleForeColorWrite("Green",Convert.ToString(map[i,j,type])); break;
                            case 4: consoleForeColorWrite("Cyan",Convert.ToString(map[i,j,type])); break;
                            case 5: consoleForeColorWrite("Blue",Convert.ToString(map[i,j,type])); break;
                            default: consoleForeColorWrite("DarkBlue",Convert.ToString(map[i,j,type])); break;
                            case >=10: consoleForeColorWrite("DarkGray","+"); break; 
                        }
                    }
                    
                }
            }
            Console.WriteLine();
        }
        consoleForeColorWrite("Gray","\n");
    }


    private int[]? valeurs;


    private int CalculTest(int z)
    {
        int somme = 0;
        valeurs = new int[z];
        for (int i = 0; i < z; i++)
        {
            valeurs[i] = rand.Next(10);
            Console.Write(valeurs[i] + " ");
            somme += valeurs[i];
        }
        return somme;
    }


    public void DoTest()
    {
        var x = CalculTest(5);
        //        Console.WriteLine(somme);
        Console.Write(x);
        Console.WriteLine(valeurs.Length);
    }

}
