# Checkbox

Custom checkbox input

### Usage

```js
import { Checkbox } from "asc-web-components";
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

| Props             |   Type   | Required | Values | Default | Description                                                 |
| ----------------- | :------: | :------: | :----: | :-----: | ----------------------------------------------------------- |
| `id`              | `string` |    -     |   -    |    -    | Used as HTML `id` property                                  |
| `name`            | `string` |    -     |   -    |    -    | Used as HTML `name` property                                |
| `value`           | `string` |    -     |   -    |    -    | Value of the input                                          |
| `label`           | `string` |    -     |   -    |    -    | Label of the input                                          |
| `isChecked`       |  `bool`  |    -     |   -    | `false` | The checked property sets the checked state of a checkbox   |
| `isIndeterminate` |  `bool`  |    -     |   -    |    -    | If true, this state is shown as a rectangle in the checkbox |
| `isDisabled`      |  `bool`  |    -     |   -    |    -    | Disables the Checkbox input                                 |
| `onChange`        |  `func`  |    ✅    |   -    |    -    | Will be triggered whenever an CheckboxInput is clicked      |
