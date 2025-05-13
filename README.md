# Card Game - Suit Collection

A strategic card game built in Unity where players and AI opponents take turns drawing and discarding cards with the goal of collecting cards of the same suit.

## Game Overview

In this card game, players compete against an AI opponent to be the first to collect a hand entirely of the same suit. The game features two modes: 3-card mode or 6-card mode, providing different levels of complexity and strategy.

## How to Play

### Objective
- Be the first to collect all cards of the same suit in your hand.
- If the deck runs out before anyone achieves this, the player with the lowest total card value wins.

### Game Modes
- **3-Card Mode**: Each player holds 3 cards arranged in a single row.
- **6-Card Mode**: Each player holds 6 cards arranged in a 3×2 grid.

### Game Flow
1. At the start of the game, each player is dealt their initial hand (3 or 6 cards depending on the selected mode).
2. Players take turns to:
   - Discard one card from their hand to the discard pile
   - Draw a new card from the deck
3. The game continues until one player collects all cards of the same suit or the deck runs out.

## Strategic Elements

### Player Strategy
- Monitor your hand for suits that are appearing more frequently
- Consider which suits are being discarded by the AI
- Try to collect cards of a single suit while discarding cards of other suits

### AI Strategy
The AI uses a sophisticated decision-making process:

1. **Suit Analysis**: The AI evaluates both its hand and the discard pile to understand suit distribution.
2. **Potential Calculation**: For each suit in its hand, the AI calculates a "potential score" based on:
   - How many cards of that suit it already holds
   - How many cards of that suit have been discarded (fewer discards = higher potential)
3. **Decision Making**: 
   - Primary strategy: Discard cards from the suit with the lowest potential score
   - Fallback strategy: Discard from the suit with the fewest cards in hand
   - Ultimate fallback: Discard the first card in hand

## Technical Implementation

### Key Features
- **Animated Card Movements**: Smooth transitions using DOTween for card movements and transforms
- **Dynamic Card Layouts**: Automatically arranges cards based on the selected game mode
- **Robust Turn Management**: Prevents action during animations and maintains proper game flow
- **Win Condition Detection**: Continuously checks for winning conditions after each card draw
- **UI Animations**: Animated transitions for end game messages and restart options

### Code Architecture
- **Game Controller**: Manages game flow, card actions, and win conditions
- **GameMode**: Static class that maintains game settings across scenes
- **Level Loader**: Handles scene transitions and game mode selection
- **Card**: Individual card behavior and interaction

## Development
The game was developed in Unity with C# and features:
- Clean, well-organized code with logical grouping of related functions
- Extensive error prevention with state flags and condition checks
- Visually appealing card animations and transitions
- Intelligent AI opponent with multiple fallback strategies

## Installation and Setup
1. Clone this repository
2. Open the project in Unity (2021.3 or newer recommended)
3. Open the starting scene (main menu)
4. Press Play to start the game

## Controls
- Click on cards in your hand to discard them
- The game will automatically draw a new card and continue play

---

*Developed by Yigit Salih Emecen*
