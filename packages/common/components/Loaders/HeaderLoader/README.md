# Header Loader

Component that displays header loader

### Usage

```js
import Loaders from "@docspace/common/components/Loaders";
```

```jsx
<Loaders.Header />
```

### Properties

| Props               |   Type   | Required | Values | Default | Description                                      |
| ------------------- | :------: | :------: | :----: | :-----: | ------------------------------------------------ |
| `title`             | `string` |    -     |   -    |   ``    | It's used to describe what element it is.        |
| `borderRadius`      | `string` |    -     |   -    |   `3`   | Sets the corners rounding                        |
| `backgroundColor`   | `string` |    -     |   -    | `#fff`  | Used as background of animation                  |
| `foregroundColor`   | `string` |    -     |   -    | `#fff`  | Used as the foreground of animation              |
| `backgroundOpacity` | `number` |    -     |   -    |   0.2   | Background opacity (0 = transparent, 1 = opaque) |
| `foregroundOpacity` | `number` |    -     |   -    |  0.25   | Animation opacity (0 = transparent, 1 = opaque)  |
| `speed`             | `number` |    -     |   -    |    2    | Animation speed in seconds                       |
| `animate`           |  `bool`  |    -     |   -    |  true   | Opt-out of animations                            |
