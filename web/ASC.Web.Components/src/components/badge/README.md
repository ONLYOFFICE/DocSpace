# Badge

Used for buttons, numbers or status markers next to icons.

### Usage

```js
import { Badge } from "asc-web-components";
```

```jsx
<Badge
  number={10}
  backgroundColor="#ED7309"
  color="#FFFFFF"
  fontSize="11px"
  fontWeight={800}
  borderRadius="11px"
  padding="0 5px"
  maxWidth="50px"
  onClick={() => {}}
/>
```

### Properties

| Props             |   Type   | Required | Values |  Default  | Description          |
| ----------------- | :------: | :------: | :----: | :-------: | -------------------- |
| `number`          | `number` |    -     |   -    |    `0`    | Number value         |
| `backgroundColor` | `string` |    -     |   -    | `#ED7309` | CSS background-color |
| `color`           | `string` |    -     |   -    | `#FFFFFF` | CSS color            |
| `fontSize`        | `string` |    -     |   -    |  `11px`   | CSS font-size        |
| `fontWeight`      | `number` |    -     |   -    |   `800`   | CSS font-weight      |
| `borderRadius`    | `string` |    -     |   -    |  `11px`   | CSS border-radius    |
| `padding`         | `string` |    -     |   -    |  `0 5px`  | CSS padding          |
| `maxWidth`        | `string` |    -     |   -    |  `50px`   | CSS max-width        |
| `onClick`         |  `func`  |    -     |   -    |     -     | onClick event        |
