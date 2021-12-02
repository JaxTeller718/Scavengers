/*
* Class: EntityZombieHerd
* Author:  Khaine 
* Category: Entity
* Description:
*   Simple hack to create an extra, bloodmoon compatible entity to prevent NREs on spikes. Extends from EntityEnemyAnimal
*/

public class NukeBloodMoons : EntityZombie
{
    public override void OnEntityDeath()
    {
        int nukeSpawns = 0;
        
        GamePrefs.Set(EnumGamePrefs.BloodMoonEnemyCount, nukeSpawns);
        GamePrefs.Set(EnumGamePrefs.BloodMoonFrequency, nukeSpawns);
        GamePrefs.Set(EnumGamePrefs.BloodMoonRange, nukeSpawns);
        GamePrefs.Set(EnumGamePrefs.BloodMoonWarning, nukeSpawns);

        base.OnEntityDeath();
    }
}