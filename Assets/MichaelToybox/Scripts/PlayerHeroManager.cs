using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script manages player heroes outside of combat
public class PlayerHeroManager : MonoBehaviour
{
    CombatManager combatManager;
    public List<BaseHero> heroes;

    private void Awake()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>(); //gets combat manager reference
    }

    public void CopyCombatHealthToHeroes()
    {
        foreach(BaseHero hero in combatManager.playerHeroes)
        {
            BaseHero perminantHero = GetHeroByName(hero.name);
            //sets health of hero equal to
            perminantHero.health = hero.health;

            //checks if the hero health is greater than the max health
            if (perminantHero.health > perminantHero.maxHealth)
                perminantHero.health = perminantHero.maxHealth;
        }
    }

    /// <summary>
    /// Returns a hero with the matching name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BaseHero GetHeroByName(string name)
    {
        foreach(BaseHero hero in heroes)
        {
            if (hero.name == name)
                return hero;
        }

        return null;
    }
}
