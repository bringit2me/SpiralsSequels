using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChangeSpellDamage : BaseTargetSpell
{
    [Header("Spell Damage Change")]
    public int spellDamageChange;

    public override void SetupEffectEntry()
    {
        base.SetupEffectEntry();
        if(spellDamageChange > 0)
            cardEffectEntry.description = "+" + spellDamageChange + " Spell Damage";
        else
            cardEffectEntry.description = "-" + spellDamageChange + " Spell Damage";
    }

    public override void CastAtTarget()
    {

        //Gets minion reference. if card is not a minion it will be null
        BaseMinion minion = target.GetComponent<BaseMinion>();
        //Gets hero reference. if card is not a hero it will be null
        BaseHero hero = target.GetComponent<BaseHero>();

        if (minion == true)
        {
            minion.ChangeSpellDamage(spellDamageChange); //changes spell damage

            minion.visualManager.AddEffectEntry(cardEffectEntry);

            playAnimCopy.target = target.gameObject; //sets anim target
            anim.PlayAnimation(playAnimCopy); //plays animation
        }
        if (hero == true)
        {
            hero.ChangeSpellDamage(spellDamageChange); //changes spell damage

            hero.visualManager.AddEffectEntry(cardEffectEntry);

            playAnimCopy.target = target.gameObject; //sets anim target
            anim.PlayAnimation(playAnimCopy); //plays animation
        }

        combatManager.UpdateSpellDamage(target.team); //updates spell damage for the cards team
        base.CastAtTarget();
    }

    public override CardValueEntry CalculateValueAI(BaseEnemyAI ai)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = this;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all potential targets of the spell

        foreach (BaseCard card in targets)
        {
            if (card == null)
                continue;

            int value = 0;
            //Gets minion reference. if card is not a minion it will be null
            BaseMinion minion = card.GetComponent<BaseMinion>();
            //Gets hero reference. if card is not a hero it will be null
            BaseHero hero = card.GetComponent<BaseHero>();

            int cardAttack = 0;
            int cardHealth = 0;

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion != null)
                {
                    //changes value
                    value += minion.CalculateSpellDamageChange(spellDamageChange);

                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value += hero.CalculateSpellDamageChange(spellDamageChange) * 3; //3x multiplier for modifying hero spell damage

                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }
            else //target is on the opposite team
            {
                //NOTE: For cards on the other team, we want to make the calculated values negative. so that +2 attack has a -2 value. and that -2 health has a +2 value.
                if (minion != null)
                {
                    //changes value
                    value -= minion.CalculateSpellDamageChange(spellDamageChange);

                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value -= hero.CalculateSpellDamageChange(spellDamageChange) * 3; //Times 3 multiplier for modifying hero spell damage on player hero

                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }

            value -= manaCost; //subtracts mana cost
            value += valueBoostAI; //adds in value boost

            //checks if AI is agressive, spell gives spell damage, and target is on the same team
            //if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && spellDamageChange > 0 && card.team == playerManager.team)
            //value = (int)(value * ValueToPercent(ai.aggroValue));
            //checks if AI is mid range, target is a hero, the spell damage changes are positive, and target is on the same team
            if (ai.playstyle == EnemyPlaystyle.MID_RANGE && hero != null && spellDamageChange > 0 && card.team == playerManager.team)
                value = (int)(value * ValueToPercent(ai.midRangeValue));
            //checks if AI is defensive, target is a hero, spell damage change is positive, and target is on the same team
            if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && hero != null && spellDamageChange > 0 && card.team == playerManager.team)
                value = (int)(value * ValueToPercent(ai.defenseValue));
            //checks if AI is defensive, target is a hero, health change is negative, and target is on the opposite team
            if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && hero != null && spellDamageChange < 0 && card.team != playerManager.team)
                value = (int)(value * ValueToPercent(ai.defenseValue));


            if (value > entry.value) //if the target has a higher value
            {
                entry.value = value;
                entry.target = card;
            }
            else if (value == entry.value && entry.target != null) //if the target and a previously found best target have an equal value
            {
                if (entry.target.GetComponent<BaseMinion>() == true) //previously found best target is a minion
                {
                    BaseMinion entryMinion = entry.target.GetComponent<BaseMinion>();
                    if (hero != null) //new found target is a hero
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryMinion.health < cardHealth) //new found target has more health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryMinion.health == cardHealth && entryMinion.attack < cardAttack) //new found target has equal health but more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
                else if (entry.target.GetComponent<BaseHero>() == true) //previously found best target is hero
                {
                    BaseHero entryHero = entry.target.GetComponent<BaseHero>();
                    if (entryHero.health < cardHealth) //new found target has more health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryHero.health == cardHealth && entryHero.attack < cardAttack) //new found target has equal health but more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
            }
        }

        return entry;
    }
}
