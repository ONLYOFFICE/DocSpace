# Floating Button

Component that displays floating button

### Usage

```js
import FloatingButton from "@docspace/common/components/FloatingButton";
```

```jsx
<FloatingButton icon="upload" alert={false} />
```

```jsx
<FloatingButton icon="trash" alert={true} percent={45} />
```

### Properties

| Props       |      Type      | Required |                     Values                     | Default  | Description                 |
| ----------- | :------------: | :------: | :--------------------------------------------: | :------: | --------------------------- |
| `alert`     |     `bool`     |    -     |                       -                        | `false`  | Shows the alert             |
| `className` |    `string`    |    -     |                       -                        |    -     | Accepts class               |
| `icon`      |    `oneOf`     |    -     | `upload`, `file`, `trash`, `move`, `duplicate` | `upload` | Sets the icon on the button |
| `id`        |    `string`    |    -     |                       -                        |    -     | Accepts id                  |
| `percent`   |    `number`    |    -     |                       -                        |   `0`    | Load fullness               |
| `style`     | `obj`, `array` |    -     |                       -                        |    -     | Accepts css style           |
