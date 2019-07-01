# DateInput

#### Description

Custom date input

#### Usage

```js
import { DateInput } from 'asc-web-components';

<DateInput selected={new Date()} dateFormat="dd.MM.yyyy" onChange={date => {}}/>
```

#### Properties

https://reactdatepicker.com/

| Props        | Type     | Required | Values | Default | Description                             |
| ------------ | -------- | :------: | ------ | ------- | --------------------------------------- |
| `id`         | `string` |    -     | -      | -       | Used as HTML `id` property              |
| `name`       | `string` |    -     | -      | -       | Used as HTML `name` property            |
| `disabled`   | `bool`   |    -     | -      | -       | Used as HTML `disabled` property        |
| `readOnly`   | `bool`   |    -     | -      | -       | Used as HTML `readOnly` property        |
| `selected`   | `date`   |    -     | -      | -       | Selected date value                     |
| `onChange`   | `func`   |    -     | -      | -       | OnChange event                          |
| `dateFormat` | `string` |    -     | -      | -       | Date format string                      |
| `hasError`   | `bool`   |    -     | -      | -       | Indicates the input field has an error  |
| `hasWarning` | `bool`   |    -     | -      | -       | Indicates the input field has a warning |
