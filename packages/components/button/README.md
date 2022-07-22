# Button

Button is used for a action on a page.

### Usage

```js
import Button from "@docspace/components/button";
```

```jsx
<Button
  size="extraSmall"
  isDisabled={false}
  onClick={() => alert("Button clicked")}
  label="OK"
/>
```

### Properties

| Props        |      Type      | Required |                  Values                   |   Default    | Description                                                                                          |
| ------------ | :------------: | :------: | :---------------------------------------: | :----------: | ---------------------------------------------------------------------------------------------------- |
| `className`  |    `string`    |    -     |                     -                     |      -       | Accepts class                                                                                        |
| `icon`       |     `node`     |    -     |                     -                     |    `null`    | Icon node element                                                                                    |
| `id`         |    `string`    |    -     |                     -                     |      -       | Accepts id                                                                                           |
| `isClicked`  |     `bool`     |    -     |                     -                     |   `false`    | Tells when the button should present a clicked state                                                 |
| `isDisabled` |     `bool`     |    -     |                     -                     |   `false`    | Tells when the button should present a disabled state                                                |
| `isHovered`  |     `bool`     |    -     |                     -                     |   `false`    | Tells when the button should present a hovered state                                                 |
| `isLoading`  |     `bool`     |    -     |                     -                     |   `false`    | Tells when the button should show loader icon                                                        |
| `label`      |    `string`    |    -     |                     -                     |      -       | Button text                                                                                          |
| `onClick`    |     `func`     |    -     |                     -                     |      -       | What the button will trigger when clicked                                                            |
| `primary`    |     `bool`     |    -     |                     -                     |   `false`    | Tells when the button should be primary                                                              |
| `scale`      |     `bool`     |    -     |                     -                     |   `false`    | Scale width of button to 100%                                                                        |
| `size`       |    `oneOf`     |    -     | `extraSmall`, `small`, `normal`, `medium` | `extraSmall` | Size of button. The normal size equals 36px and 40px in height on the Desktop and Touchcreen devices |
| `style`      | `obj`, `array` |    -     |                     -                     |      -       | Accepts css style                                                                                    |
| `tabIndex`   |    `number`    |    -     |                     -                     |     `-1`     | Button tab index                                                                                     |
| `minWidth`   |    `string`    |    -     |                     -                     |    `null`    | Sets the min width of the button                                                                     |
