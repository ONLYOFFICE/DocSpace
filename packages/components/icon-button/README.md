# IconButton

IconButton is used for a action on a page

### Usage

```js
import IconButton from "@docspace/components/icon-button";
```

```jsx
<IconButton
  size="25"
  isDisabled={false}
  onClick={() => alert("Clicked")}
  iconName={"static/images/search.react.svg"}
  isFill={true}
  isClickable={false}
/>
```

### Properties

| Props           |        Type        | Required | Values |     Default     | Description                                           |
| --------------- | :----------------: | :------: | :----: | :-------------: | ----------------------------------------------------- |
| `className`     |      `string`      |    -     |   -    |        -        | Set component class                                   |
| `clickColor`    |      `string`      |    -     |   -    |        -        | Icon color on click action                            |
| `color`         |      `string`      |    -     |   -    |    `#d0d5da`    | Icon color                                            |
| `hoverColor`    |      `string`      |    -     |   -    |        -        | Icon color on hover action                            |
| `iconClickName` |      `string`      |    -     |   -    |        -        | Icon name on click action                             |
| `iconHoverName` |      `string`      |    -     |   -    |        -        | Icon name on hover action                             |
| `iconName`      |      `string`      |    ✅    |   -    | `AZSortingIcon` | Icon name                                             |
| `id`            | `string`, `number` |    -     |   -    |        -        | Set component id                                      |
| `isClickable`   |       `bool`       |    -     |   -    |     `false`     | Set cursor value                                      |
| `isDisabled`    |       `bool`       |    -     |   -    |     `false`     | Tells when the button should present a disabled state |
| `isFill`        |       `bool`       |    -     |   -    |     `true`      | Determines if icon fill is needed                     |
| `onClick`       |       `func`       |    -     |   -    |        -        | What the button will trigger when clicked             |
| `onMouseDown`   |       `func`       |    -     |   -    |        -        | What the button will trigger when cursor down         |
| `onMouseEnter`  |       `func`       |    -     |   -    |        -        | What the button will trigger when cursor enter        |
| `onMouseUp`     |       `func`       |    -     |   -    |        -        | What the button will trigger when cursor up           |
| `size`          | `number`,`string`  |    -     |   -    |      `25`       | Button height and width value                         |
| `style`         |   `obj`, `array`   |    -     |   -    |        -        | Accepts css style                                     |
