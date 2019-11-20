# Button

Button is used for a action on a page.

### Usage

```js
import { Button } from "asc-web-components";
```

```jsx
<Button
  size="base"
  isDisabled={false}
  onClick={() => alert("Button clicked")}
  label="OK"
/>
```

### Properties

| Props        |   Type   | Required |         Values          | Default | Description                                           |
| ------------ | :------: | :------: | :---------------------: | :-----: | ----------------------------------------------------- |
| `label`      | `string` |    -     |            -            |    -    | Button text                                           |
| `primary`    |  `bool`  |    -     |            -            | `false` | Tells when the button should be primary               |
| `size`       | `oneOf`  |    -     | `base`, `middle`, `big` | `base`  | Size of button                                        |
| `scale`      |  `bool`  |    -     |            -            | `false` | Scale width of button to 100%                         |
| `icon`       |  `node`  |    -     |            -            | `null`  | Icon node element                                     |
| `tabIndex`   | `number` |    -     |            -            |  `-1`   | Button tab index                                      |
| `isHovered`  |  `bool`  |    -     |            -            | `false` | Tells when the button should present a hovered state  |
| `isClicked`  |  `bool`  |    -     |            -            | `false` | Tells when the button should present a clicked state  |
| `isDisabled` |  `bool`  |    -     |            -            | `false` | Tells when the button should present a disabled state |
| `isLoading`  |  `bool`  |    -     |            -            | `false` | Tells when the button should show loader icon         |
| `onClick`    |  `func`  |    -     |            -            |    -    | What the button will trigger when clicked             |
