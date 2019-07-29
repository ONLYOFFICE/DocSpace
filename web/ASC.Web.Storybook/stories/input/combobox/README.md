# ComboBox

#### Description

Custom combo box input

#### Usage

```js
import { ComboBox } from 'asc-web-components';

<ComboBox options={options} isDisabled={false}/>
```

#### Properties

| Props                  | Type              | Required | Values                       | Default | Description                                  |
| ---------------------- | ----------------- | :------: | ---------------------------- | ------- | -------------------------------------------- |
| `options`              | `array`           |    ✅    | -                            | -       | Combo box options                            |
| `isDisabled`           | `bool`            |    -     | -                            | `false` | Indicates that component is disabled         |
| `withBorder`           | `bool`            |    -     | -                            | `true`  | Indicates that component contain border      |
| `selectedIndex`        | `number`          |    -     | -                            | `0`     | Index of option selected by default          |