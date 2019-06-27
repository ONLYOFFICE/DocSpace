# DateInput

#### Description

Custom date input

#### Usage

```js
import { DateInput } from 'asc-web-components';

<DateInput value="text"/>
```

#### Properties

| Props                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `id`                   | `string` |    -     | -                            | -       | Used as HTML `id` property                                                                             |
| `name`                 | `string` |    -     | -                            | -       | Used as HTML `name` property                                                                           |
| `value`                | `string` |    ✅    | -                            | -       | Value of the input                                                                                     |
| `onChange`             | `func`   |    -     | -                            | -       | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `onBlur`               | `func`   |    -     | -                            | -       | Called when field is blurred                                                                           |
| `onFocus`              | `func`   |    -     | -                            | -       | Called when field is focused                                                                           |
| `isAutofocussed`       | `bool`   |    -     | -                            | -       | Focus the input field on initial render                                                                |
| `isDisabled`           | `bool`   |    -     | -                            | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved)                     |
| `isReadOnly`           | `bool`   |    -     | -                            | `false` | Indicates that the field is displaying read-only content                                               |
| `hasError`             | `bool`   |    -     | -                            | -       | Indicates the input field has an error                                                                 |
| `hasWarning`           | `bool`   |    -     | -                            | -       | Indicates the input field has a warning                                                                |
| `placeholder`          | `string` |    -     | -                            | -       | Placeholder text for the input                                                                         |
