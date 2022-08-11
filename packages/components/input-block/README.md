# InputBlock

InputBlock description

### Usage

```js
import InputBlock from "@docspace/components/input-block";
```

```js
const mask = [/\d/, /\d/, "/", /\d/, /\d/, "/", /\d/, /\d/, /\d/, /\d/];
```

```jsx
<InputBlock
  mask={mask}
  iconName={"static/images/search.react.svg"}
  onIconClick={(event) => alert(event.target.value)}
  onChange={(event) => alert(event.target.value)}
>
  <Button
    size="base"
    isDisabled={false}
    onClick={() => alert("Button clicked")}
    label="OK"
  />
</InputBlock>
```

### Properties

| Props            |      Type      | Required |             Values              | Default | Description                                                                                            |
| ---------------- | :------------: | :------: | :-----------------------------: | :-----: | ------------------------------------------------------------------------------------------------------ |
| `autoComplete`   |    `string`    |    -     |                -                |    -    | Used as HTML `autocomplete` property                                                                   |
| `className`      |    `string`    |    -     |                -                |    -    | Accepts class                                                                                          |
| `hasError`       |     `bool`     |    -     |                -                |    -    | Indicates the input field has an error                                                                 |
| `hasWarning`     |     `bool`     |    -     |                -                |    -    | Indicates the input field has a warning                                                                |
| `iconColor`      |    `string`    |    -     |                -                | `black` | Specifies the icon color                                                                               |
| `iconName`       |    `string`    |    -     |                -                |    -    | Specifies the icon name                                                                                |
| `iconSize`       |    `number`    |    ✅    |                -                |    -    | Size icon                                                                                              |
| `id`             |    `string`    |    -     |                -                |    -    | Used as HTML `id` property                                                                             |
| `isAutofocussed` |     `bool`     |    -     |                -                |    -    | Focus the input field on initial render                                                                |
| `isDisabled`     |     `bool`     |    -     |                -                | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved)                     |
| `isIconFill`     |     `bool`     |    -     |                -                | `false` | Determines if icon fill is needed                                                                      |
| `isReadOnly`     |     `bool`     |    -     |                -                | `false` | Indicates that the field is displaying read-only content                                               |
| `mask`           |    `array`     |    -     |                -                |    -    | input text mask                                                                                        |
| `name`           |    `string`    |    -     |                -                |    -    | Used as HTML `name` property                                                                           |
| `onBlur`         |     `func`     |    -     |                -                |    -    | Called when field is blurred                                                                           |
| `onChange`       |     `func`     |    -     |                -                |    -    | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `onFocus`        |     `func`     |    -     |                -                |    -    | Called when field is focused                                                                           |
| `onIconClick`    |     `func`     |    ✅    |                -                |    -    | Will be triggered whenever an icon is clicked                                                          |
| `placeholder`    |    `string`    |    -     |                -                |    -    | Placeholder text for the input                                                                         |
| `scale`          |     `bool`     |    -     |                -                |    -    | Indicates the input field has scale                                                                    |
| `size`           |    `string`    |    -     | `base`, `middle`, `big`, `huge` | `base`  | Supported size of the input fields.                                                                    |
| `style`          | `obj`, `array` |    -     |                -                |    -    | Accepts css style                                                                                      |
| `type`           |    `string`    |    -     |       `text`, `password`        | `text`  | Supported type of the input fields.                                                                    |
| `value`          |    `string`    |    ✅    |                -                |    -    | Value of the input                                                                                     |
