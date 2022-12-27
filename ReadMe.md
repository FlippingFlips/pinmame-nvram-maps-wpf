# NvMaps Helper - Windows
---

Finding the scores and writing mapping files can be a laborious task, this aims to take some of the pain away.

## Quick Guide
---

### Get Scores
---

- Load up a game and note the HighScores, the names of the scores and the initials of the players

- Play a 4 player game and get some scores (a single ball for each player should be fine). Screen shot the scores or write them down.

### User interface
---

- Find the saved ram (.nv) for the game and drop it onto the UI (top left)

- Fill in your name etc

### Last Scores
---

Older games don't save score so this could be skipped.

- Fill in the fields for the last scores. You might want to prefix some zeros if your score is low

### High Scores
---

- Fill in the fields, `Label, ShortLabel, Initials, Score`

## Searching
---

Use the `Find Scores` button and you should see some selections in the `S, I` columns. These are the search result offsets for Score and Initials.

Some games may have the initials in multiple places so from here you select the correct offset which is easy to do as the scores, initials will be closer together.

### Generate Mapping
---

This will populate the JSON mapping box and show you the output.

You can preview your output and check your search results selections are good, as it should display the scores you wrote down from the start.

### Save map
---

Saves the nvram, the json mapping and app state. So next time you drop your memory in it will keep all the input given.

* Ripleys demo included

![](/screens/screen_wpf_a.jpg)

![](/screens/screen_wpf_b.jpg)

![](/screens/screen_wpf_c.jpg)

![](/screens/screen_wpf_d.jpg)

![](/screens/screen_wpf_e.jpg)