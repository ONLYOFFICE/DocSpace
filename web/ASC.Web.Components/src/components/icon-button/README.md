# IconButton

IconButton is used for a action on a page

### Usage

```js
import { IconButton } from "asc-web-components";
```

```jsx
<IconButton
  size="25"
  isDisabled={false}
  onClick={() => alert("Clicked")}
  iconName={"SearchIcon"}
  isFill={true}
  isClickable={false}
/>
```

### Properties

| Props           |        Type        | Required | Values |     Default     | Description                                           |
| --------------- | :----------------: | :------: | :----: | :-------------: | ----------------------------------------------------- |
| `color`         |      `string`      |    -     |   -    |    `#d0d5da`    | Icon color                                            |
| `hoverColor`    |      `string`      |    -     |   -    |        -        | Icon color on hover action                            |
| `clickColor`    |      `string`      |    -     |   -    |        -        | Icon color on click action                            |
| `size`          | `number`,`string`  |    -     |   -    |      `25`       | Button height and width value                         |
| `isDisabled`    |       `bool`       |    -     |   -    |     `false`     | Tells when the button should present a disabled state |
| `iconName`      |      `string`      |    ✅    |   -    | `AZSortingIcon` | Icon name                                             |
| `iconHoverName` |      `string`      |    -     |   -    |        -        | Icon name on hover action                             |
| `iconClickName` |      `string`      |    -     |   -    |        -        | Icon name on click action                             |
| `isFill`        |       `bool`       |    -     |   -    |     `true`      | Determines if icon fill is needed                     |
| `onClick`       |       `func`       |    -     |   -    |        -        | What the button will trigger when clicked             |
| `onMouseEnter`  |       `func`       |    -     |   -    |        -        | What the button will trigger when cursor enter        |
| `onMouseDown`   |       `func`       |    -     |   -    |        -        | What the button will trigger when cursor down         |
| `onMouseUp`     |       `func`       |    -     |   -    |        -        | What the button will trigger when cursor up           |
| `isClickable`   |       `bool`       |    -     |   -    |     `false`     | Set cursor value                                      |
| `className`     |      `string`      |    -     |   -    |        -        | Set component class                                   |
| `id`            | `string`, `number` |    -     |   -    |        -        | Set component id                                      |
