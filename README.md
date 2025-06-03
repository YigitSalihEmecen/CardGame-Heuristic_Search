# Heuristic Search in Card Games: An AI Implementation Study

**Course:** Artificial Intelligence  
**Student:** Yigit Salih Emecen & Samil Mirza Ozavar   
**Topic:** Heuristic Search Card Game  
**Date:** May 29, 2025  

## Abstract

This project explores the implementation of heuristic search algorithms in a competitive card game environment. The developed Unity-based card game features an AI opponent that uses sophisticated heuristic evaluation functions to make strategic decisions. The game challenges players to collect cards of the same suit while competing against an AI that analyzes game state, evaluates potential moves, and employs multiple fallback strategies to optimize its gameplay.

## 1. Introduction

For this AI course assignment, I decided to implement a heuristic search algorithm in the context of a card game. The project demonstrates how AI can be applied to decision-making in competitive scenarios where incomplete information and strategic thinking are crucial. The game I created is essentially a suit-collection card game where both human players and AI opponents try to gather all cards of the same suit before their opponent does.

The main motivation behind choosing this topic was to understand how heuristic functions can guide AI decision-making in environments where:
- There are multiple possible actions at each turn
- The optimal choice depends on both current state and future possibilities
- Uncertainty exists due to hidden information (opponent's hand and remaining deck)

## 2. Game Design and Rules

### 2.1 Basic Mechanics
The game follows these simple but strategic rules:
- **Objective**: Be the first to collect all cards of the same suit
- **Game Modes**: 3-card mode (easier) or 6-card mode (more complex)
- **Turn Structure**: Discard one card → Draw one card → Switch turns
- **Win Conditions**: 
  - Primary: All cards in hand are the same suit
  - Secondary: If deck runs out, player with lowest total card value wins

### 2.2 Strategic Elements
What makes this game interesting from an AI perspective is that every decision has long-term consequences. When the AI discards a card, it's not just getting rid of something unwanted—it's also giving information to the opponent and potentially helping them complete their suit. This creates a fascinating decision-making problem that's perfect for heuristic approaches.

## 3. AI Implementation: Heuristic Search Algorithm

### 3.1 Problem Formulation
From an AI perspective, each turn presents a search problem:
- **State**: Current hand configuration, visible discard pile, known information
- **Actions**: Which card to discard from the current hand
- **Goal**: Reach a state where all cards are the same suit
- **Heuristic**: Evaluation function that estimates the "potential" of each suit

### 3.2 The Heuristic Function
The core of my AI implementation is a multi-factor heuristic that evaluates each suit's potential:

```
Potential Score = Base Potential × Hand Count Factor

Where:
Base Potential = 1.0 - (Cards Discarded / 13)
Hand Count Factor = Number of cards of this suit in AI's hand
```

This heuristic considers:
1. **Scarcity**: Suits with fewer discarded cards have higher potential (more cards still available)
2. **Current Investment**: Suits with more cards already in hand get priority
3. **Opponent Information**: By tracking the discard pile, the AI can infer which suits are less likely to be completed

### 3.3 Decision-Making Strategy
The AI uses a hierarchical approach:

1. **Primary Strategy**: Calculate potential scores for all suits and discard from the suit with the lowest potential
2. **Fallback Strategy**: If potential scores are equal, discard from the suit with the fewest cards in hand
3. **Ultimate Fallback**: In edge cases, simply discard the first card

This approach ensures the AI always has a valid move while trying to optimize for the best possible outcome.

### 3.4 Search Space Considerations
While this might seem like a simple greedy algorithm, it's actually performing a form of heuristic search:
- It evaluates multiple possible actions (discarding different cards)
- It uses domain knowledge (suit distribution) to guide decisions
- It considers both immediate and future implications of each choice

## 4. Technical Implementation

### 4.1 Code Architecture
The project is structured around several key components:

- **Game Controller**: Manages overall game flow, turn management, and win condition checking
- **AI Strategy Methods**: Implements the heuristic evaluation and decision-making logic
- **Card Action Methods**: Handles card movements, animations, and game state updates
- **GameMode System**: Allows dynamic switching between 3-card and 6-card modes

### 4.2 Key Features Implemented
- **Smooth Animations**: Used DOTween for polished card movements and transitions
- **Robust State Management**: Prevents illegal moves and handles edge cases
- **Dynamic Layout System**: Automatically arranges cards based on game mode
- **Comprehensive Logging**: Detailed debug output to understand AI decision-making process

### 4.3 Technical Challenges and Solutions

**Challenge 1: Preventing Multiple Actions**
*Problem*: Players could click multiple cards during animations, causing game state issues
*Solution*: Implemented state flags (`isDiscardInProgress`, `isGameOver`) to prevent concurrent actions

**Challenge 2: Dynamic Card Layout**
*Problem*: Different game modes required different card arrangements
*Solution*: Created flexible layout system that calculates positions based on card count and spacing parameters

**Challenge 3: AI Decision Consistency**
*Problem*: AI needed to make reasonable decisions even with incomplete information
*Solution*: Implemented multiple fallback strategies to ensure AI always has a valid, logical move

## 5. Results and Observations

### 5.1 AI Performance
Through testing, I observed that the AI:
- Successfully avoids discarding cards from suits it's actively collecting
- Adapts its strategy based on what's been discarded
- Makes reasonable decisions even in complex 6-card scenarios
- Provides a challenging but fair opponent for human players

### 5.2 Heuristic Effectiveness
The potential-based heuristic proved effective because:
- It balances current hand state with future possibilities
- It incorporates observable information (discard pile) to make informed decisions
- It provides clear prioritization even when choices seem equivalent

### 5.3 Learning Outcomes
This project helped me understand:
- How heuristic functions can encode domain knowledge
- The importance of handling edge cases in AI systems
- How to balance computational efficiency with decision quality
- The challenges of implementing AI in real-time interactive systems

## 6. Reflections and Future Improvements

### 6.1 What Worked Well
- The hierarchical decision-making approach proved robust and reliable
- The potential-based scoring system created believable AI behavior
- Clean code organization made the system easy to debug and extend

### 6.2 Areas for Improvement
If I were to continue this project, I would consider:
- **Opponent Modeling**: Track player's discarding patterns to predict their strategy
- **Lookahead Search**: Implement minimax or similar algorithms to consider multiple moves ahead
- **Machine Learning Integration**: Use reinforcement learning to improve the heuristic over time
- **More Complex Scenarios**: Add special cards or rule variations to increase strategic depth

### 6.3 Academic Connection
This project connected several concepts from our AI course:
- **Search Algorithms**: While not implementing traditional search trees, the AI evaluates multiple options and selects the best one
- **Heuristic Design**: The potential scoring function encodes domain knowledge to guide decision-making
- **State Space**: Each game configuration represents a state, with actions leading to new states
- **Optimization**: The AI attempts to maximize its chances of winning through strategic card selection

## 7. Technical Setup and Usage

### 7.1 Installation
1. Clone this repository
2. Open the project in Unity (2021.3 or newer recommended)
3. Open the Difficulty_Selection scene to start
4. Press Play to begin

### 7.2 Controls
- Click on cards in your hand to discard them
- The game automatically handles drawing and turn management
- Use the restart button to play again

### 7.3 Code Structure
The main game logic is in `game_controller.cs`, with key methods:
- `GetAICardToDiscard()`: Implements the heuristic decision-making
- `DiscardCard()` and `DrawCard()`: Handle player actions
- `ArrangeHand()`: Manages dynamic card layouts
- `EndGameCheck()`: Evaluates win conditions

## 8. Conclusion

This project successfully demonstrates the application of heuristic search principles in a game environment. The AI opponent uses domain knowledge to make strategic decisions, providing an engaging challenge for human players while showcasing fundamental AI concepts.

The implementation shows how even relatively simple heuristics can create sophisticated behavior when properly designed and integrated. The project also highlights the importance of robust software engineering practices when building interactive AI systems.

Overall, this assignment helped me gain practical experience with AI algorithm implementation while creating something genuinely fun to play with!

---

**Repository**: [CardGame-Heuristic_Search](https://github.com/YigitSalihEmecen/CardGame-Heuristic_Search)  
**Developed by**: Yigit Salih Emecen & Samil Mirza Ozavar
**Course**: Artificial Intelligence  
**May 2025**
