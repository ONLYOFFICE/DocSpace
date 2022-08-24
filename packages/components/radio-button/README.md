# RadioButton

RadioButton allow you to add radiobutton

### Usage

```js
import RadioButton from "@docspace/components/radio-button";
```

```jsx
<RadioButton name="fruits" value="apple" label="Sweet apple" />
```

### Properties

`<RadioButtonGroup />` props supersede RadioButton props

| Props         |       Type       | Required |          Values          |   Default    | Description                                                                                                                                                                                                       |
| ------------- | :--------------: | :------: | :----------------------: | :----------: | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `className`   |     `string`     |    -     |            -             |      -       | Accepts class                                                                                                                                                                                                     |
| `id`          |     `string`     |    -     |            -             |      -       | Accepts id                                                                                                                                                                                                        |
| `isChecked`   |      `bool`      |    -     |            -             |   `false`    | Used as HTML `checked` property for each `<input>` tag                                                                                                                                                            |
| `isDisabled`  |      `bool`      |    -     |            -             |   `false`    | Used as HTML `disabled` property for each `<input>` tag                                                                                                                                                           |
| `label`       |     `string`     |    -     |            -             |      -       | Name of the radiobutton. If missed, `value` will be used                                                                                                                                                          |
| `fontSize`    |     `string`     |    -     |            -             |      -       | Font size of link                                                                                                                                                                                                 |
| `fontWeight`  | `number, string` |    -     |            -             |      -       | Font weight of link                                                                                                                                                                                               |
| `name`        |     `string`     |    ✅    |            -             |      -       | Used as HTML `name` property for `<input>` tag.                                                                                                                                                                   |
| `onClick`     |      `func`      |    -     |            -             |      -       | Allow you to handle clicking events on component                                                                                                                                                                  |
| `orientation` |     `oneOf`      |    -     | `vertical`, `horizontal` | `horizontal` | Position of radiobuttons                                                                                                                                                                                          |
| `spacing`     |     `string`     |    -     |            -             |    `15px`    | Margin between radiobutton. If orientation `horizontal`, it is `margin-left`(apply for all radiobuttons, except first), if orientation `vertical`, it is `margin-bottom`(apply for all radiobuttons, except last) |
| `style`       |  `obj`, `array`  |    -     |            -             |      -       | Accepts css style                                                                                                                                                                                                 |
| `value`       |     `string`     |    ✅    |            -             |      -       | Used as HTML `value` property for `<input>` tag. Used for identification each radiobutton                                                                                                                         |
| `width`       |     `string`     |    -     |            -             |    `100%`    | Width of RadioButtonGroup container                                                                                                                                                                               |
