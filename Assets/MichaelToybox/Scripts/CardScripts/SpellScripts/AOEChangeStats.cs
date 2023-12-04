using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEChangeStats : BaseAOESpell
{
    [Header("Stat Change")]
    [SerializeField] int attackChange;
    [SerializeField] int healthChange;

    public override void SetupEffectEntry()
    {
        base.SetupEffectEntry();

        string desc = "";

        if(attackChange > 0)
        {
            desc += "+" + attackChange;
        }
        else if (attackChange < 0)
        {
            desc += "-" + attackChange;
        }

        desc += "attack ";

        if (healthChange > 0)
        {
            desc += "+" + healthChange;
        }
        else if (healthChange < 0)
        {
            desc += "-" + healthChange;
        }

        desc += "health";

        cardEffectEntry.description = desc;
    }

    public override void Cast()
    {
        base.Cast();

        foreach (BaseCard card in targets)
        {

            //Gets minion reference. if card is not a minion it will be null
            BaseMinion minion = card.GetComponent<BaseMinion>();
            //Gets hero reference. if card is not a hero it will be null
            BaseHero hero = card.GetComponent<BaseHero>();

            if (minion == true)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                card.visualManager.AddStatChangeEntry(minion.attack + minion.CalculateAttackChange(attackChange), minion.health + minion.CalculateHealthChange(healthChange), cardEffectEntry);
                playAnimCopy.cardVisualsToUpdate.Add(card); //adds card to updater (updates card visuals after animation)

                //changes minion stats
                minion.ChangeAttack(attackChange);
                minion.ChangeHealth(healthChange);
            }
            else if (hero == true && hero.isDead == false)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                card.visualManager.AddStatChangeEntry(hero.attack + hero.CalculateAttackChange(attackChange), hero.health + hero.CalculateHealthChange(healthChange), cardEffectEntry);
                playAnimCopy.cardVisualsToUpdate.Add(card); //adds card to updater (updates card visuals after animation)

                //changes hero stats
                hero.ChangeAttack(attackChange);
                hero.ChangeHealth(healthChange);
            }
        }

        if (targets.Count > 0)
        {
            playAnimCopy.target = targets[0].gameObject; //sets target
            anim.PlayAnimation(playAnimCopy); //plays animation
        }

        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all targets of the spell

        bool effectsFriendlyHero = false;
        bool effectsFriendlyMinions = false;
        bool effectsPlayerHero = false;
        bool effectsPlayerMinions = false;

        foreach (BaseCard card in targets)
        {
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion == true)
                {
                    value += minion.CalculateAttackChange(attackChange);
                    value += minion.CalculateHealthChange(healthChange);
                    effectsFriendlyMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value += hero.CalculateAttackChange(attackChange) * 2; //2x multiplier for giving attack to a hero
                    value += hero.CalculateHealthChange(healthChange);
                    effectsFriendlyHero = true;
                }
            }
            else //target is on the opposite team
            {
                if (minion == true)
                {
                    value -= minion.CalculateAttackChange(attackChange);
                    value -= minion.CalculateHealthChange(healthChange);
                    effectsPlayerMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value -= hero.CalculateAttackChange(attackChange) * 3;//3x multiplier for giving attack to a hero
                    value -= hero.CalculateHealthChange(healthChange);
                    effectsPlayerHero = true;
                }
            }
        }

        value -= manaCost; //subtracts mana cost
        value += valueBoostAI; //adds in value boost

        //checks if AI is agressive, friendly character is effected, and attack is increased
        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && (effectsFriendlyMinions == true || effectsFriendlyHero == true) && attackChange > 0)
            value = value - 2;
        //checks if AI is mid range, friendly minion is effected, and stats are increased
        if (ai.playstyle == EnemyPlaystyle.MID_RANGE && effectsFriendlyMinions == true && (attackChange + healthChange) > 0)
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive, friendly hero is effected, and health is increased
        if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && effectsFriendlyHero == true && healthChange > 0)
            value = (int)(value * ValueToPercent(ai.defenseValue));
        //checks if AI is defensive, enemy character is effected, and attack is lowered
        else if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && (effectsPlayerMinions == true || effectsPlayerHero == true) && attackChange < 0)
            value = (int)(value * ValueToPercent(ai.defenseValue));

        return value;
    }
}
