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
              { value: 'apple', label: 'Sweet apple', disabled: true },
              { value: 'mandarin', label: 'Mandarin'},
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
| `options`                | `arrayOf` |    ✅    | -           | -       | Radiobuttons data: it is array of objects, each of this can include next information: `value` (required), `label`, `disabled`
| `spaceBtwnElems`                | `number` |    -    | -           | 33       | Margin (in px) between radiobuttons
| `isDisabledGroup`                | `bool` |    -    | -           | `false`       | Disabling all radiobuttons in group

#### Prop `options`

| Name                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `value`                 | `string` |    ✅     | -                            | -       | Used as HTML `value` property for `<input>` tag. Used for identification each radiobutton  
| `label`                | `string` |    -    | -                            | -       | Name of the radiobutton. If missed, `value` will be used
| `disabled`                | `bool` |    -    | -                            | `false`       | Used as HTML `disabled` property for each `<input>` tag