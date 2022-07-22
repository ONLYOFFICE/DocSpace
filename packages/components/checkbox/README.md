# Checkbox

Custom checkbox input

### Usage

```js
import Checkbox from "@docspace/components/checkbox";
```

```jsx
<Checkbox
  id="id"
  name="name"
  value="value"
  label="label"
  isChecked={false}
  isIndeterminate={false}
  isDisabled={false}
  onChange={() => {}}
/>
```

### Properties

| Props             |      Type      | Required | Values | Default | Description                                                 |
| ----------------- | :------------: | :------: | :----: | :-----: | ----------------------------------------------------------- |
| `className`       |    `string`    |    -     |   -    |    -    | Accepts class                                               |
| `id`              |    `string`    |    -     |   -    |    -    | Used as HTML `id` property                                  |
| `isChecked`       |     `bool`     |    -     |   -    | `false` | The checked property sets the checked state of a checkbox   |
| `isDisabled`      |     `bool`     |    -     |   -    |    -    | Disables the Checkbox input                                 |
| `isIndeterminate` |     `bool`     |    -     |   -    |    -    | If true, this state is shown as a rectangle in the checkbox |
| `label`           |    `string`    |    -     |   -    |    -    | Label of the input                                          |
| `name`            |    `string`    |    -     |   -    |    -    | Used as HTML `name` property                                |
| `onChange`        |     `func`     |    ✅    |   -    |    -    | Will be triggered whenever an CheckboxInput is clicked      |
| `style`           | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                           |
| `value`           |    `string`    |    -     |   -    |    -    | Value of the input                                          |
| `title`           |     `bool`     |    -     |   -    |    -    | Title                                                       |
| `truncate`        |     `bool`     |    -     |   -    | `false` | Disables word wrapping                                      |
| `color`           |    `string`    |    -     |   -    | `#FFFF` | Makes the checkbox a different color                        |
