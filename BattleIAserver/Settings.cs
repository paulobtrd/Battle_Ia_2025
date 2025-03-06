using System;

namespace BattleIAserver
{
    public class Settings
    {
        public int ServerPort { get; set; } = 4226;
        public int DelayBetweenEachBotTurn { get; set; } = 500;
        public UInt16 MapWidth { get; set; } = 100;//60;
        public UInt16 MapHeight { get; set; } = 100;//50;
        public UInt16 MapPercentWall { get; set; } = 3;
        public UInt16 MapPercentEnergy { get; set; } = 5;

        public UInt16 EnergyPodFrom { get; set; } = 1;
        public UInt16 EnergyPodTo { get; set; } = 50;
        public UInt16 EnergyPodLessEvery { get; set; } = 5;
        public UInt16 EnergyPodLessValue { get; set; } = 1;
        public UInt16 EnergyPodMin { get; set; } = 10;

        //[Newtonsoft.Json.JsonIgnoreAttribute]
        public UInt16 EnergyPodMax { get; set; } = 0;

        public UInt16 EnergyCloakCostMultiplier { get; set; } = 2;
        public UInt16 EnergyLostByCloak { get; set; } = 1;

        public UInt16 EnergyStart { get; set; } = 100;
        public UInt16 EnergyLostByTurn { get; set; } = 1;
        public UInt16 EnergyLostByShield { get; set; } = 1;
        public UInt16 EnergyLostByMove { get; set; } = 1;
        public UInt16 EnergyLostShot { get; set; } = 2;
        public UInt16 EnergyLostContactWall { get; set; } = 5;
        public UInt16 EnergyLostContactEnemy { get; set; } = 15;

        public UInt16 PointByTurn { get; set; } = 1;
        public UInt16 PointByEnergyFound { get; set; } = 8;
        public UInt16 PointByEnnemyTouch { get; set; } = 20;
        public UInt16 PointByEnnemyKill { get; set; } = 70;

        public UInt16 ShieldAbsorptionRatio { get; set; } = 3;

        public string IndestructibleBotId { get; set; } = "92718bb2-563f-4859-9b16-7755a32b35c9"; //"6bab03cf-fe94-431f-8c8b-16b18aaaa7ed";
        public bool IndestructibleBot { get; set; } = true;
        public bool MapWithBonus { get; set; } = true;
        public bool MapWithMalus { get; set; } = true;

    }
}
