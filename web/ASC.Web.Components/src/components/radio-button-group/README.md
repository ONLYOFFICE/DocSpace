# RadioButtonGroup

#### Description

RadioButtonGroup allow you to add group radiobuttons

#### Usage

```js
import { RadioButtonGroup } from 'asc-web-components';

<RadioButtonGroup 
  name='fruits' 
  selected='banana'
  options={
            [
              { value: 'apple', label: 'Sweet apple'},
              { value: 'banana', label: 'Banana'},
              { value: 'Mandarin'}
            ]
          } 
            />
```

#### Properties

| Props                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `name`                 | `string` |    ✅     | -                            | -       | Used as HTML `name` property for `<input>` tag. Used for identification RadioButtonGroup                             
| `selected`                | `string` |    ✅    | -                            | -       | Value of the selected radiobutton      
| `options`                | `arrayOf` |    ✅    | -           | -       | Array of objects, contains props for each `<RadioButton />` component
| `spacing`                | `number` |    -    | -           | 33       | Margin (in px) between radiobuttons
| `isDisabled`                | `bool` |    -    | -           | `false`       | Disabling all radiobuttons in group
| `onClick`                | `func` |    -    | -           | -       | Allow you to handle clicking events on `<RadioButton />` component
