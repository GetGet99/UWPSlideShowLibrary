# UWPSlideShowLibrary

UWP Slide Show Library is a library that makes making SlideShow easier by abstracting Slide show code in the back. Using this project, you only need to work on a bit of UI Code.

Why did I do this?

* It has more possibilities, such as static background, acrylic, mica, easing animations, programmable UI automation, and anything else you can do in UWP.

Example Project with two slides is included. (only basic features; do not include what I say is "possible.")

If you can understand the code written in the sample project and have basic UWP knowledge, you might be able to do what you want.

## Screenshots

![Title Slide](https://media.discordapp.net/attachments/761586679416619088/969550596812202034/unknown.png)

![Statistic Slide](https://media.discordapp.net/attachments/761586679416619088/969550597525209088/unknown.png)

![Explaination SLide](https://media.discordapp.net/attachments/761586679416619088/969550597760098344/unknown.png)

![Non Full screen](https://media.discordapp.net/attachments/761586679416619088/969550597076447252/unknown.png)

![Non Full Screen](https://media.discordapp.net/attachments/761586679416619088/969550597076447252/unknown.png)

## Features

1. (Only i/n Sample Project. You can copy and paste the code from there if you need this feature) It does not break if the slide is shown in other resolutions. Instead, it will scale and center itself.
2. Slide management is abstracted away. You derive the new slide from Slide class, and you don't need to care about the internal part of how the slide system works.