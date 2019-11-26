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

| Props                     |       Type        | Required | Values |  Default  | Description                                      |
| ------------------------- | :---------------: | :------: | :----: | :-------: | ------------------------------------------------ |
| `isVertical`              |      `bool`       |    -     |   -    |  `false`  | Vertical or horizontal alignment                 |
| `isRequired`              |      `bool`       |    -     |   -    |  `false`  | Indicates that the field is required to fill     |
| `hasError`                |      `bool`       |    -     |   -    |  `false`  | Indicates that the field is incorrect            |
| `labelText`               |     `string`      |    -     |   -    |     -     | Field label text                                 |
| `tooltipContent`          | `object`,`string` |    -     |   -    |     -     | Tooltip content                                  |
| `helpButtonHeaderContent` |     `string`      |    -     |   -    |     -     | Tooltip header content (tooltip opened in aside) |
| `maxLabelWidth`           |     `string`      |    -     |   -    |  `110px`  | Max label width in horizontal alignment          |
| `errorMessage`            |     `string`      |    -     |   -    |     -     | Error message text                               |
| `errorColor`              |     `string`      |    -     |   -    | `#C96C27` | Error text color                                 |
| `errorMessageWidth`       |     `string`      |    -     |   -    |  `320px`  | Error text width                                 |
