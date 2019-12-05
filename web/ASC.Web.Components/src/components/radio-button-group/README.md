# RadioButtonGroup

RadioButtonGroup allow you to add group radiobutton

### Usage

```js
import { RadioButtonGroup } from "asc-web-components";
```

```jsx
<RadioButtonGroup
  name="fruits"
  selected="banana"
  options={[
    { value: "apple", label: "Sweet apple" },
    { value: "banana", label: "Banana" },
    { value: "Mandarin" }
  ]}
/>
```

### Properties

| Props        |      Type      | Required | Values | Default | Description                                                                              |
| ------------ | :------------: | :------: | :----: | :-----: | ---------------------------------------------------------------------------------------- |
| `className`  |    `string`    |    -     |   -    |    -    | Accepts class                                                                            |
| `id`         |    `string`    |    -     |   -    |    -    | Accepts id                                                                               |
| `isDisabled` |     `bool`     |    -     |   -    | `false` | Disabling all radiobutton in group                                                       |
| `name`       |    `string`    |    ✅    |   -    |    -    | Used as HTML `name` property for `<input>` tag. Used for identification RadioButtonGroup |
| `onClick`    |     `func`     |    -     |   -    |    -    | Allow you to handle clicking events on `<RadioButton />` component                       |
| `options`    |   `arrayOf`    |    ✅    |   -    |    -    | Array of objects, contains props for each `<RadioButton />` component                    |
| `selected`   |    `string`    |    ✅    |   -    |    -    | Value of the selected radiobutton                                                        |
| `spacing`    |    `number`    |    -     |   -    |  `33`   | Margin (in px) between radiobutton                                                       |
| `style`      | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                                        |
