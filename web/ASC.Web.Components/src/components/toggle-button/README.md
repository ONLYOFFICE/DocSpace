# ToggleButton

Custom toggle button input

### Usage

```js
import { ToggleButton } from "asc-web-components";
```

```jsx
<ToggleButton
  label="text"
  onChange={event => console.log(event.target.value)}
  isChecked={false}
/>
```

#### Properties

| Props        |   Type   | Required | Values | Default | Description                                                    |
| ------------ | :------: | :------: | :----: | :-----: | -------------------------------------------------------------- |
| `label`      | `string` |    -     |   -    |    -    | Label of the input                                             |
| `isChecked`  |  `bool`  |    -     |   -    |    -    | The checked property sets the checked state of a ToggleButton. |
| `isDisabled` |  `bool`  |    -     |   -    |    -    | Disables the ToggleButton                                      |
| `onChange`   |  `func`  |    ✅    |   -    |    -    | Will be triggered whenever an ToggleButton is clicked          |
