using System;
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


    // ****************************************************************************************************
    /// On doit effectuer une action
    public byte[] GetAction()
    {
        int direction = 0;
        bool ok = false;
        // Si le bot vient d'être touché
        if (hasBeenHit)
        {
            // Le bot a-t-il encore du bouclier ?
            if (currentShieldLevel == 0)
            {
                // NON ! On s'empresse d'en réactiver un de suite !
                currentShieldLevel = (byte)rand.Next(1, 9);
                return BotHelper.ActionShield(currentShieldLevel);
            }
            // oui, il reste du bouclier actif

            // On réinitialise notre flag
            hasBeenHit = false;

            // Puis on déplace fissa le bot, au hazard...
            do{
                direction = rand.Next(1, 5);
                switch (direction){
                    case 1 : if(map[x,y-1,0]>2 && y-1>=0){ok=true;} break;
                    case 2 : if(map[x-1,y,0]>2 && x-1>=0){ok=true;} break;
                    case 3 : if(map[x,y+1,0]>2 && y+1<hauteurMap){ok=true;} break;
                    case 4 : if(map[x+1,y,0]>2 && x+1<hauteurMap){ok=true;} break;
                }
            }while(ok!=true);
            switch (direction){
                case 1 : y--; Console.WriteLine("North"); break;
                case 2 : x--; Console.WriteLine("West"); break;
                case 3 : y++; Console.WriteLine("South"); break;
                case 4 : x++; Console.WriteLine("East"); break;
            }
            return BotHelper.ActionMove((MoveDirection)direction);
            /*
            Explications :
                rand.Next(1, 5)   : tire un nombre aléatoire entre 1 (inclus) et 5 (exclu), donc 1, 2, 3 ou 4
                (MoveDirection)x : converti 'x' en type MoveDirection
                sachant que 1 = North, 2 = West, 3 = South et 4 = East
             */
        }

        // S'il n'y a pas de bouclier actif, on en active un
        if (currentShieldLevel == 0)
        {
            currentShieldLevel = 1;
            return BotHelper.ActionShield(currentShieldLevel);
        }

        // On déplace le bot au hazard
        do{
            direction = rand.Next(1, 5);
            switch (direction){
                case 1 : if(map[x,y-1,0]>2 && y-1>=0){ok=true;} break;
                case 2 : if(map[x-1,y,0]>2 && x-1>=0){ok=true;} break;
                case 3 : if(map[x,y+1,0]>2 && y+1<hauteurMap){ok=true;} break;
                case 4 : if(map[x+1,y,0]>2 && x+1<hauteurMap){ok=true;} break;
            }
        }while(ok!=true);
        switch (direction){
            case 1 : y--; Console.WriteLine("North"); break;
            case 2 : x--; Console.WriteLine("West"); break;
            case 3 : y++; Console.WriteLine("South"); break;
            case 4 : x++; Console.WriteLine("East"); break;
        }
        return BotHelper.ActionMove((MoveDirection)direction);


        // Voici d'autres exemples d'actions possibles
        // -------------------------------------------

        // Si on ne veut rien faire, passer son tour
        // return BotHelper.ActionNone();

        // Déplacement du bot au nord
        // return BotHelper.ActionMove(MoveDirection.North);

        // Activation d'un bouclier de protection de niveau 10 (peut encaisser 10 points de dégats)
        // return BotHelper.ActionShield(10);

        // Activation d'un voile d'invisibilité sur une surface de 15
        // return BotHelper.ActionCloak(15);

        // Tir dans la direction sud
        // return BotHelper.ActionShoot(MoveDirection.South);

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