# ContextMenuButton

ContextMenuButton is used for displaying context menu actions on a list's item

### Usage

```js
import { ContextMenuButton } from "asc-web-components";
```

```jsx
<ContextMenuButton
  iconName="VerticalDotsIcon"
  size={16}
  color="#A3A9AE"
  isDisabled={false}
  title="Actions"
  getData={() => [
    {
      key: "key",
      label: "label",
      onClick: () => alert("label")
    }
  ]}
/>
```

### Properties

| Props           |   Type   | Required |     Values     |      Default       | Description                                           |
| --------------- | :------: | :------: | :------------: | :----------------: | ----------------------------------------------------- |
| `title`         | `string` |    -     |       -        |         -          | Specifies the icon title                              |
| `iconName`      | `string` |    -     |       -        | `VerticalDotsIcon` | Specifies the icon name                               |
| `iconHoverName` | `string` |    -     |       -        |         -          | Specifies the icon hover name                         |
| `iconClickName` | `string` |    -     |       -        |         -          | Specifies the icon click name                         |
| `size`          | `number` |    -     |       -        |        `16`        | Specifies the icon size                               |
| `color`         | `string` |    -     |       -        |         -          | Specifies the icon color                              |
| `hoverColor`    | `string` |    -     |       -        |         -          | Specifies the icon hover color                        |
| `clickColor`    | `string` |    -     |       -        |         -          | Specifies the icon click color                        |
| `isDisabled`    |  `bool`  |    -     |       -        |      `false`       | Tells when the button should present a disabled state |
| `onMouseEnter`  |  `func`  |    -     |       -        |         -          | What the button will trigger when mouse hovered       |
| `onMouseLeave`  |  `func`  |    -     |       -        |         -          | What the button will trigger when mouse leave         |
| `onMouseOver`   |  `func`  |    -     |       -        |         -          | What the button will trigger when mouse over button   |
| `onMouseOut`    |  `func`  |    -     |       -        |         -          | What the button will trigger when mouse out of button |
| `directionX`    | `oneOf`  |    -     | `left`,`right` |       `left`       | What the button will trigger when mouse out of button |
| `opened`        |  `bool`  |    -     |       -        |      `false`       | Tells when the button should present a opened state   |
| `data`          | `array`  |    -     |       -        |       `[ ]`        | Array of options for display                          |
| `getData`       |  `func`  |    ✅    |       -        |         -          | Function for converting to inner data                 |
