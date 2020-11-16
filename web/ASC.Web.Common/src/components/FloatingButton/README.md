# Floating Button

Component that displays floating button

### Usage

```js
import { FloatingButton } from "asc-web-common";
```

```jsx
<FloatingButton icon="upload" alert={false} />
```

```jsx
<FloatingButton icon="trash" alert={true} />
```

### Properties

| Props   |  Type   | Required |                     Values                     | Default  | Description                 |
| ------- | :-----: | :------: | :--------------------------------------------: | :------: | --------------------------- |
| `icon`  | `oneOf` |    -     | `upload`, `file`, `trash`, `move`, `duplicate` | `upload` | Sets the icon on the button |
| `alert` | `bool`  |    -     |                       -                        | `false`  | Shows the alert             |
