# Deck Mech

By: Benjamin Blas Maristany, Diego Medina Molina

## Overview

Deck Mech is a deckbuilding game where players will build their desired armor and then fight until one remains standing. They will first take turns discovering new robot parts and choosing the ones that attract them. Stay on your toes, however, as your opponent can steal your favorite mech parts from right under your grasp. Balance your defenses and make sure you build the strongest mech you can!

## Player Interactivity

We have opted to play Deck Mech with a **point-and-click** playstyle. We felt that this option was the most player-friendly since the gameplay mostly revolves around players choosing cards rather than placing them anywhere specific on the play area.

## Game Rules

Deck Mech is played in phases. Players will first build their armor by drawing Armor Cards, though they don’t have complete control over what they end up with. Each armor piece will have a certain amount of each stat. After completing their armor, they will attack each other repeatedly by drawing Attack Cards until one player reaches 0 HP.

### Cards and Stats

There are 5 main types of attacks that the player must resist against:
- Impact Attacks
- Thermal Attacks
- Freeze Attacks
- Electric Attacks
- Void Attacks

Each armor piece will have a certain amount of points allocated to resisting these 5 types of attack, as well as an **HP** stat. Generally, each armor piece will resist primarily against one type and have some resistance against one other type and 0 for the rest. Some armor types might be balanced, meaning that they resist all 5 types at a lower capacity than other armor pieces might.

Each attack card will deal a certain amount of base damage of a specified type to the recipient. All attack damage is dealt directly to the HP. The final damage dealt is calculated by subtracting the Attack Card's power from the player's resistance stat of the attack's specific type (if the attack is a 15 pt. Freeze Attack and the player's combined Freeze Resistance stat is 5, the attack will deal 10 HP in damage).

### Armor Building Phase
Players will decide who is Player 1 and who is Player 2. Starting with Player 1, each player will alternate turns doing the following:

1. Draw 3 cards from the shuffled Armor Deck.  
2. Select one of the 3 cards to add to the Assembly Pile. Place the other cards in a discard deck. This goes until the Assembly Pile has at least one of each of the 4 armor piece types (one helmet, one chestplate, one set of gauntlets, one set of greaves). If the armor piece deck is depleted, the discard pile may be shuffled and used as the new deck.  
3. As soon as the 4th missing piece is added, the player who added it must build their full armor from the pieces in the Assembly Pile. If there is more than one card for a single type of armor (e.g. two different helmets), the player may choose which piece they want.  
4. When the first player has finalized their armor construction, the second player will continue the Draw-3-Pick-1 pattern to finish creating their own armor.  
5. When both Armors are created, all the player’s stats from their armor pieces will be combined to get a final readout of each player’s HP and resistances for all types.

### Fighting Phase

The player who ends up with the lower HP stat will go first.

1. Players will take turns drawing 2 attack cards and choosing 1 to use. The unused attack card will go into a discard pile that will be shuffled and used if the attack pile is depleted.
2. The amount of damage that the defending player will receive is equal to the amount of damage that the attacking card does minus their defense stat for the attack card’s specific type.
3. When a player is hit with a **Finishing Blow** (meaning the attack they are bout receive will fully deplete their HP), they will have the chance to nullify the attack in one **Last Stand**. To do this, they must draw 1 attack card. The card that they choose can nullify the finishing blow *if and only if* one of two conditions is met:

- Parry Condition: The last stand attack is of the *same type* and is *stronger or equal* to the finishing blow. The attacks will cancel each other before reaching either player.
- Abort: The last stand attack *is itself enough to be a finishing blow to the other player* (meaning that its attack power minus the other player's corresponding resistance is stil enough to destroy them). Both players will abort their attacks since one must survive.

If a final blow is parried or aborted, the game continues with the player who survived their final blow drawing 2 Attack Cards.

If a final blow cannot be parried or aborted by the last stand, the final attacking player wins.


## Item Storage

[to be added]

## Resources

All of our card sprites were created by us. The images contain both original works and transformations of free-licenced community images from https://www.pixilart.com.