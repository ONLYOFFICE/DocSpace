# Input: TextInput

## Usage

```js
import { TextInput } from 'asc-web-components';
```

#### Description

Input description

#### Usage

```js
const mask =[/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];

<TextInput mask={mask} value="text" onChange={event => alert(event.target.value)} />;
```

#### Properties

| Props                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `id`                   | `string` |    -     | -                            | -       | Used as HTML `id` property                                                                             |
| `name`                 | `string` |    -     | -                            | -       | Used as HTML `name` property                                                                           |
| `value`                | `string` |    ✅    | -                            | -       | Value of the input                                                                                     |
| `autoComplete`         | `string` |    -     | -                            | -       | Used as HTML `autocomplete` property                                                                   |
| `onChange`             | `func`   |    -     | -                            | -       | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `onBlur`               | `func`   |    -     | -                            | -       | Called when field is blurred                                                                           |
| `onFocus`              | `func`   |    -     | -                            | -       | Called when field is focused                                                                           |
| `isAutoFocussed`       | `bool`   |    -     | -                            | -       | Focus the input field on initial render                                                                |
| `isDisabled`           | `bool`   |    -     | -                            | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved)                     |
| `isReadOnly`           | `bool`   |    -     | -                            | `false` | Indicates that the field is displaying read-only content                                               |
| `hasError`             | `bool`   |    -     | -                            | -       | Indicates the input field has an error                                                                 |
| `hasWarning`           | `bool`   |    -     | -                            | -       | Indicates the input field has a warning                                                                |
| `placeholder`          | `string` |    -     | -                            | -       | Placeholder text for the input                                                                         |
| `type`          | `string`        |          | `text`, `password` | `text` | Supported type of the input fields.                         |
| `size`          | `string`        |          | `base`, `middle`, `big`, `huge` | `base` | Supported size of the input fields.                         |
| `scale`           | `bool`   |    -     | -                            | -       | Indicates the input field has scale                                                                |
| `withBorder`           | `bool`            |    -     | -                            | `true`  | Indicates that component contain border      |
| `mask`                 | `array`   |    -  | -                   | -       | input text mask                                                                             |

### Main Functions and use cases are:

- Input field for single-line strings