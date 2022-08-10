# Badge

Used for buttons, numbers or status markers next to icons.

### Usage

```js
import Badge from "@docspace/components/badge";
```

```jsx
<Badge
  label="10"
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

| Props             |      Type      | Required | Values |  Default  | Description          |
| ----------------- | :------------: | :------: | :----: | :-------: | -------------------- |
| `backgroundColor` |    `string`    |    -     |   -    | `#ED7309` | CSS background-color |
| `borderRadius`    |    `string`    |    -     |   -    |  `11px`   | CSS border-radius    |
| `className`       |    `string`    |    -     |   -    |     -     | Accepts class        |
| `color`           |    `string`    |    -     |   -    | `#FFFFFF` | CSS color            |
| `fontSize`        |    `string`    |    -     |   -    |  `11px`   | CSS font-size        |
| `fontWeight`      |    `number`    |    -     |   -    |   `800`   | CSS font-weight      |
| `id`              |    `string`    |    -     |   -    |     -     | Accepts id           |
| `maxWidth`        |    `string`    |    -     |   -    |  `50px`   | CSS max-width        |
| `label`           |    `string`    |    -     |   -    |    `0`    | Value                |
| `onClick`         |     `func`     |    -     |   -    |     -     | onClick event        |
| `padding`         |    `string`    |    -     |   -    |  `0 5px`  | CSS padding          |
| `style`           | `obj`, `array` |    -     |   -    |     -     | Accepts css style    |
