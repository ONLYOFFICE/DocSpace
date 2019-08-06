# ComboBox

#### Description

Custom combo box input

#### Usage

```js
import { ComboBox } from 'asc-web-components';

const options = [
    {
        key: 25,
        label: '25 per page'
    },
    {
        key: 50,
        label: '50 per page',
    },
    {
        key: 100,
        label: '100 per page'
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