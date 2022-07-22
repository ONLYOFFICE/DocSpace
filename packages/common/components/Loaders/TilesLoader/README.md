# Tiles Loader

Component that displays tiles loader

### Usage

```js
import Loaders from "@docspace/common/components/Loaders";
```

```jsx
<Loaders.Tiles />
```

```jsx
<Loaders.Tiles isRectangle={true} />
```

### Properties

| Props               |   Type   | Required | Values |  Default  | Description                                      |
| ------------------- | :------: | :------: | :----: | :-------: | ------------------------------------------------ |
| `title`             | `string` |    -     |   -    |    ``     | It's used to describe what element it is.        |
| `isRectangle`       |  `bool`  |    -     |   -    |   true    | Show rectangle or circle?                        |
| `borderRadius`      | `string` |    -     |   -    |    `3`    | Sets the corners rounding                        |
| `backgroundColor`   | `string` |    -     |   -    | `#000000` | Used as background of animation                  |
| `foregroundColor`   | `string` |    -     |   -    | `#000000` | Used as the foreground of animation              |
| `backgroundOpacity` | `number` |    -     |   -    |    0.2    | Background opacity (0 = transparent, 1 = opaque) |
| `foregroundOpacity` | `number` |    -     |   -    |   0.25    | Animation opacity (0 = transparent, 1 = opaque)  |
| `speed`             | `number` |    -     |   -    |     2     | Animation speed in seconds                       |
| `animate`           |  `bool`  |    -     |   -    |   true    | Opt-out of animations                            |
| `foldersCount`      | `number` |    -     |   -    |     3     | Set the number of folders                        |
| `filesCount`        | `number` |    -     |   -    |     3     | Set the number of files                          |
