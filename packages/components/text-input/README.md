# TextInput

Input field for single-line strings

### Usage

```js
import TextInput from "@docspace/components/text-input";
```

```js
const mask = [/\d/, /\d/, "/", /\d/, /\d/, "/", /\d/, /\d/, /\d/, /\d/];
```

```jsx
<TextInput
  mask={mask}
  value="text"
  onChange={(event) => alert(event.target.value)}
/>
```

### Properties

| Props            |        Type        | Required |                  Values                  | Default | Description                                                                                            |
| ---------------- | :----------------: | :------: | :--------------------------------------: | :-----: | ------------------------------------------------------------------------------------------------------ |
| `autoComplete`   |      `string`      |    -     |                    -                     |    -    | Used as HTML `autocomplete` property                                                                   |
| `className`      |      `string`      |    -     |                    -                     |    -    | Accepts class                                                                                          |
| `hasError`       |       `bool`       |    -     |                    -                     |    -    | Indicates the input field has an error                                                                 |
| `hasWarning`     |       `bool`       |    -     |                    -                     |    -    | Indicates the input field has a warning                                                                |
| `id`             |      `string`      |    -     |                    -                     |    -    | Used as HTML `id` property                                                                             |
| `isAutoFocussed` |       `bool`       |    -     |                    -                     |    -    | Focus the input field on initial render                                                                |
| `isDisabled`     |       `bool`       |    -     |                    -                     | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved)                     |
| `isReadOnly`     |       `bool`       |    -     |                    -                     | `false` | Indicates that the field is displaying read-only content                                               |
| `mask`           |      `array`       |    -     |                    -                     |    -    | input text mask                                                                                        |
| `name`           |      `string`      |    -     |                    -                     |    -    | Used as HTML `name` property                                                                           |
| `onBlur`         |       `func`       |    -     |                    -                     |    -    | Called when field is blurred                                                                           |
| `onChange`       |       `func`       |    -     |                    -                     |    -    | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `onFocus`        |       `func`       |    -     |                    -                     |    -    | Called when field is focused                                                                           |
| `placeholder`    |      `string`      |    -     |                    -                     |    -    | Placeholder text for the input                                                                         |
| `scale`          |       `bool`       |    -     |                    -                     |    -    | Indicates the input field has scale                                                                    |
| `size`           |      `string`      |          | `base`, `middle`, `big`, `huge`, `large` | `base`  | Supported size of the input fields.                                                                    |
| `style`          |   `obj`, `array`   |    -     |                    -                     |    -    | Accepts css style                                                                                      |
| `type`           |      `string`      |          |            `text`, `password`            | `text`  | Supported type of the input fields.                                                                    |
| `value`          |      `string`      |    ✅    |                    -                     |    -    | Value of the input                                                                                     |
| `withBorder`     |       `bool`       |    -     |                    -                     | `true`  | Indicates that component contain border                                                                |
| `fontWeight`     | `number`, `string` |    -     |                    -                     |    -    | Sets the font weight                                                                                   |
| `isBold`         |       `bool`       |    -     |                    -                     | `false` | Sets font weight value ​​to 600                                                                        |
