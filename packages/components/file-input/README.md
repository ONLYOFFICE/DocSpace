# FileInput

File entry field

### Usage

```js
import FileInput from "@docspace/components/file-input";
```

```jsx
<FileInput
  placeholder="Input file"
  accept=".doc, .docx"
  onInput={(file) => {
    console.log(
      file,
      `name: ${file.name}`,
      `lastModified: ${file.lastModifiedDate}`,
      `size: ${file.size}`
    );
  }}
/>
```

### Properties

| Props         |      Type      | Required |                  Values                  | Default | Description                                                                        |
| ------------- | :------------: | :------: | :--------------------------------------: | :-----: | ---------------------------------------------------------------------------------- |
| `className`   |    `string`    |    -     |                    -                     |    -    | Accepts class                                                                      |
| `hasError`    |     `bool`     |    -     |                    -                     | `false` | Indicates the input field has an error                                             |
| `hasWarning`  |     `bool`     |    -     |                    -                     | `false` | Indicates the input field has a warning                                            |
| `id`          |    `string`    |    -     |                    -                     |    -    | Used as HTML `id` property                                                         |
| `isDisabled`  |     `bool`     |    -     |                    -                     | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved) |
| `name`        |    `string`    |    -     |                    -                     |    -    | Used as HTML `name` property                                                       |
| `onInput`     |     `func`     |    -     |                    -                     |    -    | Called when a file is selected                                                     |
| `placeholder` |    `string`    |    -     |                    -                     |    -    | Placeholder text for the input                                                     |
| `scale`       |     `bool`     |    -     |                    -                     | `false` | Indicates the input field has scale                                                |
| `size`        |    `string`    |    -     | `base`, `middle`, `big`, `huge`, `large` | `base`  | Supported size of the input fields.                                                |
| `style`       | `obj`, `array` |    -     |                    -                     |    -    | Accepts css style                                                                  |
| `accept`      |    `string`    |    -     |                    -                     |    -    | Specifies files visible for upload                                                 |
