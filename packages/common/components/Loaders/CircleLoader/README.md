# Circle Loader

Component that displays circle loader

### Usage

```js
import Loaders from "@docspace/common/components/Loaders";
```

```jsx
<Loaders.Circle />
```

```jsx
<Loaders.Circle x="15" y="15" radius="15" animate="false" title="Loading..." />
```

### Properties

| Props               |   Type   | Required | Values |  Default  | Description                                      |
| ------------------- | :------: | :------: | :----: | :-------: | ------------------------------------------------ |
| `title`             | `string` |    -     |   -    |    ``     | It's used to describe what element it is.        |
| `x`                 | `string` |    -     |   -    |    `3`    | Sets the x offset                                |
| `y`                 | `string` |    -     |   -    |    `3`    | Sets the y offset                                |
| `width`             | `string` |    -     |   -    |  `100%`   | Sets the width                                   |
| `height`            | `string` |    -     |   -    |  `100%`   | Sets the height                                  |
| `radius`            | `string` |    -     |   -    |    `3`    | Sets the circle radius                           |
| `backgroundColor`   | `string` |    -     |   -    | `#000000` | Used as background of animation                  |
| `foregroundColor`   | `string` |    -     |   -    | `#000000` | Used as the foreground of animation              |
| `backgroundOpacity` | `number` |    -     |   -    |    0.2    | Background opacity (0 = transparent, 1 = opaque) |
| `foregroundOpacity` | `number` |    -     |   -    |   0.15    | Animation opacity (0 = transparent, 1 = opaque)  |
| `speed`             | `number` |    -     |   -    |     2     | Animation speed in seconds                       |
| `animate`           |  `bool`  |    -     |   -    |   true    | Opt-out of animations                            |
