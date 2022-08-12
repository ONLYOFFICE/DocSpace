# ContextMenuButton

ContextMenuButton is used for displaying context menu actions on a list's item

### Usage

```js
import ContextMenuButton from "@docspace/components/context-menu-button";
```

```jsx
<ContextMenuButton
  iconName="static/images/vertical-dots.react.svg"
  size={16}
  color="#A3A9AE"
  isDisabled={false}
  title="Actions"
  getData={() => [
    {
      key: "key",
      label: "label",
      onClick: () => alert("label"),
    },
  ]}
/>
```

### Properties

| Props           |      Type      | Required |     Values     |      Default       | Description                                           |
| --------------- | :------------: | :------: | :------------: | :----------------: | ----------------------------------------------------- |
| `className`     |    `string`    |    -     |       -        |         -          | Accepts class                                         |
| `clickColor`    |    `string`    |    -     |       -        |         -          | Specifies the icon click color                        |
| `color`         |    `string`    |    -     |       -        |         -          | Specifies the icon color                              |
| `data`          |    `array`     |    -     |       -        |       `[ ]`        | Array of options for display                          |
| `directionX`    |    `oneOf`     |    -     | `left`,`right` |       `left`       | What the button will trigger when mouse out of button |
| `getData`       |     `func`     |    ✅    |       -        |         -          | Function for converting to inner data                 |
| `hoverColor`    |    `string`    |    -     |       -        |         -          | Specifies the icon hover color                        |
| `iconClickName` |    `string`    |    -     |       -        |         -          | Specifies the icon click name                         |
| `iconHoverName` |    `string`    |    -     |       -        |         -          | Specifies the icon hover name                         |
| `iconName`      |    `string`    |    -     |       -        | `VerticalDotsIcon` | Specifies the icon name                               |
| `id`            |    `string`    |    -     |       -        |         -          | Accepts id                                            |
| `isDisabled`    |     `bool`     |    -     |       -        |      `false`       | Tells when the button should present a disabled state |
| `onMouseEnter`  |     `func`     |    -     |       -        |         -          | What the button will trigger when mouse hovered       |
| `onMouseLeave`  |     `func`     |    -     |       -        |         -          | What the button will trigger when mouse leave         |
| `onMouseOut`    |     `func`     |    -     |       -        |         -          | What the button will trigger when mouse out of button |
| `onMouseOver`   |     `func`     |    -     |       -        |         -          | What the button will trigger when mouse over button   |
| `opened`        |     `bool`     |    -     |       -        |      `false`       | Tells when the button should present a opened state   |
| `size`          |    `number`    |    -     |       -        |        `16`        | Specifies the icon size                               |
| `style`         | `obj`, `array` |    -     |       -        |         -          | Accepts css style                                     |
| `title`         |    `string`    |    -     |       -        |         -          | Specifies the icon title                              |
