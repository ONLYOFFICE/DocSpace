# ToggleButton

Custom toggle button input

### Usage

```js
import ToggleButton from "@docspace/components/toggle-button";
```

```jsx
<ToggleButton
  label="text"
  onChange={(event) => console.log(event.target.value)}
  isChecked={false}
/>
```

#### Properties

| Props        |        Type        | Required | Values | Default | Description                                                    |
| ------------ | :----------------: | :------: | :----: | :-----: | -------------------------------------------------------------- |
| `className`  |      `string`      |    -     |   -    |    -    | Class name                                                     |
| `id`         | `string`, `number` |    -     |   -    |    -    | Set component id                                               |
| `isChecked`  |       `bool`       |    -     |   -    |    -    | The checked property sets the checked state of a ToggleButton. |
| `isDisabled` |       `bool`       |    -     |   -    |    -    | Disables the ToggleButton                                      |
| `label`      |      `string`      |    -     |   -    |    -    | Label of the input                                             |
| `onChange`   |       `func`       |    ✅    |   -    |    -    | Will be triggered whenever an ToggleButton is clicked          |
| `style`      |   `obj`, `array`   |    -     |   -    |    -    | Accepts css style                                              |
