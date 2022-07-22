# FieldContainer

Responsive form field container

### Usage

```js
import FieldContainer from "@docspace/components/field-container";
```

```jsx
<FieldContainer labelText="Name:">
  <TextInput value="" onChange={(e) => console.log(e.target.value)} />
</FieldContainer>
```

### Properties

| Props                     |       Type        | Required | Values |  Default  | Description                                      |
| ------------------------- | :---------------: | :------: | :----: | :-------: | ------------------------------------------------ |
| `className`               |     `string`      |    -     |   -    |     -     | Accepts class                                    |
| `errorColor`              |     `string`      |    -     |   -    | `#C96C27` | Error text color                                 |
| `errorMessageWidth`       |     `string`      |    -     |   -    |  `320px`  | Error text width                                 |
| `errorMessage`            |     `string`      |    -     |   -    |     -     | Error message text                               |
| `hasError`                |      `bool`       |    -     |   -    |  `false`  | Indicates that the field is incorrect            |
| `helpButtonHeaderContent` |     `string`      |    -     |   -    |     -     | Tooltip header content (tooltip opened in aside) |
| `id`                      |     `string`      |    -     |   -    |     -     | Accepts id                                       |
| `isRequired`              |      `bool`       |    -     |   -    |  `false`  | Indicates that the field is required to fill     |
| `isVertical`              |      `bool`       |    -     |   -    |  `false`  | Vertical or horizontal alignment                 |
| `labelText`               |     `string`      |    -     |   -    |     -     | Field label text                                 |
| `labelVisible`            |      `bool`       |    -     |   -    |  `true`   | Sets visibility of field label section           |
| `maxLabelWidth`           |     `string`      |    -     |   -    |  `110px`  | Max label width in horizontal alignment          |
| `style`                   |  `obj`, `array`   |    -     |   -    |     -     | Accepts css style                                |
| `tooltipContent`          | `object`,`string` |    -     |   -    |     -     | Tooltip content                                  |
