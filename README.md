# Deck Mech

By: Benjamin Blas Maristany, Diego Medina Molina

## Overview

Deck Mech is a deckbuilding game where players will build their desired armor and then fight until one remains standing. They will first take turns discovering new robot parts and choosing the ones that attract them. Stay on your toes, however, as your opponent can steal your favorite mech parts from right under your grasp. Balance your defenses and make sure you build the strongest mech you can!

## Player Interactivity

We have opted to play Deck Mech with a **point-and-click** playstyle. We felt that this option was the most player-friendly since the gameplay mostly revolves around players choosing cards rather than placing them anywhere specific on the play area. A single mouse is used to control both players. The game will instruct which player should be in control of the mouse at all times.

## Game Rules

Deck Mech is played in phases. Players will first build their armor by drawing Armor Cards, though they don’t have complete control over what they end up with. Each armor piece will have certain stats and resistances. After completing their armor, they will attack each other repeatedly by drawing Attack Cards until one player reaches 0 HP.

### Cards and Stats

There are 5 main types of attacks that the player must resist against:
- Impact Attacks
- Thermal Attacks
- Freeze Attacks
- Electric Attacks
- Void Attacks

Each armor piece will have a certain amount of points allocated to resisting these 5 types of attack, as well as an **HP** stat. Generally, each armor piece will resist primarily against one type and may have some resistance against one other type and 0 for the rest. Some armor types might be balanced, meaning that they will resist all 5 types at a lower capacity than other more specialized armor pieces might.

Each attack card will deal a certain amount of base damage of a specified type to the recipient. All attack damage is dealt directly to the HP. The final damage dealt is calculated by subtracting the defending player's resistance stat of the attack's specific type from the card's specified damage (i.e. if the attack is a 15 pt. Freeze Attack and the player's combined Freeze Resistance stat is 5, the attack will deal a net 10 HP in damage).

### Armor Building Phase
Players will decide who is Player 1 and who is Player 2. Starting with Player 1, each player will alternate turns doing the following:

1. Draw 3 cards from the shuffled Armor Deck.  
2. Select one of the 3 cards to add to the Hangar, which is a pile of chosen Armor Cards that is face up and spread around the play area in an organized manner. The non-chosen cards are added to the discard deck. This goes until the Hangar contains at least one of each of the 4 armor piece types (one helmet, one chestplate, one set of gauntlets, one set of greaves). If the Armor Card Deck is depleted before this phase ends, the discard pile may be shuffled and used as the new deck.  
3. As soon as the 4th missing piece is added, the player who added it must build their full armor from the pieces available in the Hangar. If there is more than one card for a single type of armor (e.g. two different helmets), the player may choose which piece they want. Note that this may include armor pieces that their opponent may have placed, creating an opportunity to steal armor pieces and mess with their opponent's plans.  
5. When one player has finalized their armor construction, the second player will continue the Draw-3-Pick-1 pattern to finish creating their own armor.  
6. When both Armors are created, all the player’s stats from their armor pieces will be combined to get a final readout of each player’s HP and resistances for all types.

### Fighting Phase

The player who ends up with the lower HP stat will go first.

1. Players will take turns drawing 2 attack cards and choosing 1 to attack with. The unused attack card will go into a discard pile that will be shuffled and used if the attack pile is depleted before the game ends.
2. The amount of damage that the defending player will receive is equal to the amount of damage that the attacking card does minus their defense stat for the attack card’s specific type.
3. When a player is hit with a **Finishing Blow** (meaning that the attack they are about receive will fully deplete their HP), they will have the chance to nullify the attack in one **Last Stand**. To do this, they will draw 1 attack card. The card that they choose can nullify the finishing blow *if and only if* one of two conditions is met:

- Parry Condition: The last stand attack is of the *same type* and is *stronger or equal* to the finishing blow. The attacks will cancel each other before reaching either player.
- Abort Condition: The last stand attack *is itself enough to be a finishing blow to the other player* (meaning that its attack power minus the other player's corresponding resistance is stil enough to destroy them). Both players will abort their attacks since one must survive.

If a final blow is parried or aborted, the game continues with the player who survived their final blow drawing 2 Attack Cards.

If a final blow cannot be parried or aborted by the last stand, the final attacking player wins.


## Item Storage

As previously mentioned, the Deck Mech player experience revolves around choosing cards rather than manipulating them around the play area. Therefore, we opted to keep our game lightweight by creating all of our in-game card objects *as they are drawn from the deck*.

All of our cards are permanently stored in the game as our own custom class `CardData`, which is an asset that is cloned once per card and is more lightweight than storing dozens of GameObject prefabs. The `CardData` assets are added as inputs to the `GameManager` script's `DeckManager` component. The `DeckManager` stores all the cards in different **queues** of `CardData`, which simulate our card decks separately. We also added a function in this script to shuffle any given deck and it is called as needed. Our `ArmorPhaseManager` and `FightPhaseManager` scripts handle the card draws and selections that our players make. Only when a card is drawn (dequeued) is when the actual in-game card is instantiated by the phase manager as a `CardUI` GameObject, whose data is populated into an `ArmorCardPrefab` or `AttackCardPefab`, depending on the card type. This object is later destroyed when the card is discarded, while the corresponding `CardData` is added to its appropriate discard "deck" (another queue). This item storage method ensures that when a card is taken off-screen, it only exists as lightweight data assets and not GameObjects.

## Resources

All of our card sprites were created by us. The images contain both original works and transformations of free-licenced community images from https://www.pixilart.com.

Our background images were obtained from Pixabay, all of which have free licenses:
* Main Menu image (Mech): https://pixabay.com/photos/model-toys-robot-science-fiction-947924
* Armor Phase image (Hangar): https://pixabay.com/illustrations/science-fiction-hangar-technology-4255632
* Fight Phase image (Watery landscape): https://pixabay.com/photos/water-nature-sun-sea-ocean-tech-3101241
