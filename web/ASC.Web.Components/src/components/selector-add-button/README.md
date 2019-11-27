# SelectorAddButton

### Usage

```js
import { SelectorAddButton } from "asc-web-components";
```

```jsx
<SelectorAddButton
  title="Add item"
  onClick={() => console.log("onClose")}
></SelectorAddButton>
```

### Properties

| Props        |    Type    | Required | Values | Default | Description                                           |
| ------------ | :--------: | :------: | :----: | :-----: | ----------------------------------------------------- |
| `isDisabled` |   `bool`   |    -     |   -    | `false` | Tells when the button should present a disabled state |
| `title`      |  `string`  |    -     |   -    |    -    | Title text                                            |
| `onClick`    | `function` |    -     |   -    |    -    | What the button will trigger when clicked             |
| `className`  |  `string`  |    -     |   -    |    -    | Attribute className                                   |
