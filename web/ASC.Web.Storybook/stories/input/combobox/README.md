# ComboBox

#### Description

Custom combo box input

Options have options: 
key - Item key, may be a string or a number,
label - Display text,
icon - Optional name of icon that will be displayed before label

#### Usage

```js
import { ComboBox } from 'asc-web-components';

const options = [
    {
        key: 1,
        icon: 'CatalogEmployeeIcon',
        label: 'Option 1'
    },
    {
        key: 2,
        icon: 'CatalogGuestIcon',
        label: 'Option 2',
    },
    {
        key: 3,
        label: 'Option 3'
    },
    {
        key: 4,
        label: 'Option 4'
    },
    {
        key: 5,
        icon: 'CopyIcon',
        label: 'Option 5'
    }
];

<ComboBox options={options} isDisabled={false} withBorder={true} selectedOption={25} onSelect={option => console.log('selected', option)}/>
```

#### Properties

| Props                  | Type              | Required | Values                       | Default | Description                                  |
| ---------------------- | ----------------- | :------: | ---------------------------- | ------- | -------------------------------------------- |
| `options`              | `array`           |    ✅    | -                            | -       | Combo box options                            |
| `isDisabled`           | `bool`            |    -     | -                            | `false` | Indicates that component is disabled         |
| `withBorder`           | `bool`            |    -     | -                            | `true`  | Indicates that component contain border      |
| `selectedOption`       | `string`,`number` |    -     | -                            | `0`     | Index of option selected by default          |
| `onSelect`             | `func`            |    -     | -                            | -       | Will be triggered whenever an ComboBox is selected option |