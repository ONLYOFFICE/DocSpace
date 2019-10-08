# Buttons: IconButton

## Usage

```js
import { IconButton } from "asc-web-components";
```

#### Description

IconButton is used for a action on a page.

#### Usage

```js
<IconButton
  size="25"
  isDisabled={false}
  onClick={() => alert("Clicked")}
  iconName={"SearchIcon"}
  isFill={true}
  isClickable={false}
/>
```

#### Properties

| Props         | Type                     | Required | Values | Default         | Description                                           |
| ------------- | ------------------------ | :------: | ------ | --------------- | ----------------------------------------------------- |
| `color`       | `string`                 |    -     | -      | `#d0d5da`       | Icon color                                            |
| `size`        | `` number` or `string `` |    -     | -      | `25`            | Button height and width value                         |
| `isDisabled`  | `bool`                   |    -     | -      | `false`         | Tells when the button should present a disabled state |
| `iconName`    | `string`                 |    ✅    | -      | `AZSortingIcon` | Icon name                                             |
| `isFill`      | `bool`                   |    -     | -      | `true`          | Determines if icon fill is needed                     |
| `onClick`     | `func`                   |    -     | -      | -               | What the button will trigger when clicked             |
| `isClickable` | `bool`                   |    -     | -      | `false`         | Set cursor value                                      |
