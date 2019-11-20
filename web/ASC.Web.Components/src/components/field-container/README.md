# FieldContainer

Responsive form field container

### Usage

```js
import { FieldContainer } from "asc-web-components";
```

```jsx
<FieldContainer labelText="Name:">
  <TextInput value="" onChange={e => console.log(e.target.value)} />
</FieldContainer>
```

### Properties

| Props                     |       Type        | Required | Values | Default | Description                                      |
| ------------------------- | :---------------: | :------: | :----: | :-----: | ------------------------------------------------ |
| `isVertical`              |      `bool`       |    -     |   -    | `false` | Vertical or horizontal alignment                 |
| `isRequired`              |      `bool`       |    -     |   -    | `false` | Indicates that the field is required to fill     |
| `hasError`                |      `bool`       |    -     |   -    | `false` | Indicates that the field is incorrect            |
| `labelText`               |     `string`      |    -     |   -    |    -    | Field label text                                 |
| `tooltipContent`          | `object`,`string` |    ✅    |   -    |    -    | Tooltip content                                  |
| `HelpButtonHeaderContent` |     `string`      |    -     |   -    |    -    | Tooltip header content (tooltip opened in aside) |
