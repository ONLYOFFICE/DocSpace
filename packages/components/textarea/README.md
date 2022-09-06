# Textarea

Textarea is used for displaying custom textarea

### Usage

```js
import Textarea from "@docspace/components/textarea";
```

```jsx
<Textarea
  placeholder="Add comment"
  onChange={(event) => alert(event.target.value)}
  value="value"
/>
```

### Properties

| Props            |      Type      | Required | Values | Default | Description                                              |
| ---------------- | :------------: | :------: | :----: | :-----: | -------------------------------------------------------- |
| `className`      |    `string`    |    -     |   -    |    -    | Class name                                               |
| `id`             |    `string`    |    -     |   -    |    -    | Used as HTML `id` property                               |
| `isDisabled`     |     `bool`     |    -     |   -    | `false` | Indicates that the field cannot be used                  |
| `isReadOnly`     |     `bool`     |    -     |   -    | `false` | Indicates that the field is displaying read-only content |
| `hasError`       |     `bool`     |    -     |   -    |    -    | Indicates the input field has an error                   |
| `name`           |    `string`    |    -     |   -    |    -    | Used as HTML `name` property                             |
| `onChange`       |     `func`     |    -     |   -    |    -    | Allow you to handle changing events of component         |
| `placeholder`    |    `string`    |    -     |   -    |    -    | Placeholder for Textarea                                 |
| `style`          | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                        |
| `value`          |    `string`    |    -     |   -    |    -    | Value for Textarea                                       |
| `fontSize`       |    `number`    |    -     |   -    |   13    | Value for font-size                                      |
| `heightTextArea` |    `number`    |    -     |   -    |    -    | Value for height text-area                               |
