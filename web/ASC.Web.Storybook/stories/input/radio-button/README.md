# RadioButton

#### Description

RadioButton allow you to add radiobutton

#### Usage

```js
import { RadioButton } from 'asc-web-components';

<RadioButton
  name='fruits'
  value= 'apple'
  label= 'Sweet apple'
  />
```

#### Properties
`<RadioButtonGroup />` props supersede RadioButton props

| Props                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `value`                 | `string` |    ✅     | -                            | -       | Used as HTML `value` property for `<input>` tag. Used for identification each radiobutton
| `name`                 | `string` |    ✅     | -                            | -       | Used as HTML `name` property for `<input>` tag.
| `label`                | `string` |    -    | -                            | -       | Name of the radiobutton. If missed, `value` will be used
| `isChecked`                | `bool` |    -    | -           | `false`       |  Used as HTML `checked` property for each `<input>` tag
| `isDisabled`                | `bool` |    -    | -           | `false`       |  Used as HTML `disabled` property for each `<input>` tag
| `onClick`                | `func` |    -    | -           | -       | Allow you to handle clicking events on component
