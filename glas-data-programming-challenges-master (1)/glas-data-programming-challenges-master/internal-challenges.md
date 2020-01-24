# 1

## Card Flipping Game

#### Description

This challenge is about a simple card flipping solitaire game. You're presented with a sequence of cards, some face up, some face down. You can remove any face up card, but you must then flip the adjacent cards (if any). The goal is to successfully remove every card. Making the wrong move can get you stuck.

In this challenge, a 1 signifies a face up card and a 0 signifies a face down card. We will also use zero-based indexing, starting from the left, to indicate specific cards. So, to illustrate a game, consider this starting card set.
```
0100110
```

I can choose to remove cards 1, 4, or 5 since these are face up. If I remove card 1, the game looks like this (using . to signify an empty spot):
```
1.10110
```
I had to flip cards 0 and 2 since they were adjacent. Next I could choose to remove cards 0, 2, 4, or 5. I choose card 0:
```
..10110
```
Since it has no adjacent cards, there were no cards to flip. I can win this game by continuing with: 2, 3, 5, 4, 6.

Supposed instead I started with card 4:
```
0101.00
```
This is unsolvable since there's an "island" of zeros, and cards in such islands can never be flipped face up.

#### Input Description
As input you will be given a sequence of 0 and 1, no spaces.

#### Output Description
Your program must print a sequence of moves that leads to a win. If there is no solution, it must print "no solution". In general, if there's one solution then there are many possible solutions.

Optional output format: Illustrate the solution step by step.

#### Sample Inputs
```
0100110
01001100111
100001100101000
```

#### Sample Outputs
```
1 0 2 3 5 4 6
no solution
0 1 2 3 4 6 5 7 8 11 10 9 12 13 14
```

#### Challenge Inputs
```
0100110
001011011101001001000
1010010101001011011001011101111
1101110110000001010111011100110
```

#### Bonus Input
```
010111111111100100101000100110111000101111001001011011000011000
```


# 2

## Number adding

### Description

A number is input in computer then a new no should get printed by adding one to each of its digit. If you encounter a 9, insert a 10 (don't carry over, just shift things around).

For example, 998 becomes 10109.

### Bonus

This challenge is trivial to do if you map it to a string to iterate over the input, operate, and then cast it back. Instead, try doing it without casting it as a string at any point, keep it numeric (int, float if you need it) only.