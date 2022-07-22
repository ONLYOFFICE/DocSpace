# Rectangle Loader

Component that displays rectangle loader

### Usage

```js
import Loaders from "@docspace/common/components/Loaders";
```

```jsx
<Loaders.Rectangle />
```

```jsx
<Loaders.Rectangle
  width="100"
  height="50"
  borderRadius="5"
  animate="false"
  title="Loading..."
/>
```

### Properties

| Props               |   Type   | Required | Values |  Default  | Description                                      |
| ------------------- | :------: | :------: | :----: | :-------: | ------------------------------------------------ |
| `title`             | `string` |    -     |   -    |    ``     | It's used to describe what element it is.        |
| `x`                 | `string` |    -     |   -    |    `0`    | Sets the x offset                                |
| `y`                 | `string` |    -     |   -    |    `0`    | Sets the y offset                                |
| `width`             | `string` |    -     |   -    |  `100%`   | Sets the width                                   |
| `height`            | `string` |    -     |   -    |   `32`    | Sets the height                                  |
| `borderRadius`      | `string` |    -     |   -    |    `3`    | Sets the corners rounding                        |
| `backgroundColor`   | `string` |    -     |   -    | `#000000` | Used as background of animation                  |
| `foregroundColor`   | `string` |    -     |   -    | `#000000` | Used as the foreground of animation              |
| `backgroundOpacity` | `number` |    -     |   -    |    0.2    | Background opacity (0 = transparent, 1 = opaque) |
| `foregroundOpacity` | `number` |    -     |   -    |   0.15    | Animation opacity (0 = transparent, 1 = opaque)  |
| `speed`             | `number` |    -     |   -    |     2     | Animation speed in seconds                       |
| `animate`           |  `bool`  |    -     |   -    |   true    | Opt-out of animations                            |
