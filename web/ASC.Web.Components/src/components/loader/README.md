# Loader

Loader component is used for displaying loading actions on a page

### Usage

```js
import { Loader } from "asc-web-components";
```

```jsx
<Loader 
  type="base" 
  color="black" 
  size={18} 
  label="Loading" 
/>
```

### Properties

| Props       |       Type        | Required |                Values                | Default | Description       |
| ----------- | :---------------: | :------: | :----------------------------------: | :-----: | ----------------- |
| `className` |     `string`      |    -     |                  -                   |    -    | Class name        |
| `color`     |     `string`      |    -     |                  -                   |    -    | Font color        |
| `id`        |     `string`      |    -     |                  -                   |    -    | Accepts id        |
| `label`     |     `string`      |    -     |                  -                   |    -    | Text label        |
| `size`      | `number`,`string` |    -     |                  -                   |    -    | Font size         |
| `style`     |  `obj`, `array`   |    -     |                  -                   |    -    | Accepts css style |
| `type`      |      `oneOf`      |    -     | `base`, `oval`, `dual-ring`, `rombs` | `base`  | -                 |
