# FilterInput

Used to filter tables

### Usage

```js
import { FilterInput } from "asc-web-components";
```

```jsx
<FilterInput
  getFilterData={() => [
    {
      key: "filter-example",
      group: "filter-example",
      label: "example group",
      isHeader: true
    },
    { key: "0", group: "filter-example", label: "Test" }
  ]}
  getSortData={() => [
    { key: "name", label: "Name", default: true },
    { key: "surname", label: "Surname", default: true }
  ]}
  onFilter={result => {
    console.log(result);
  }}
/>
```

### Properties

| Props                |      Type      | Required |             Values              | Default | Description                                                                                            |
| -------------------- | :------------: | :------: | :-----------------------------: | :-----: | ------------------------------------------------------------------------------------------------------ |
| `className`          |    `string`    |    -     |                -                |    -    | Accepts class                                                                                          |
| `id`                 |    `string`    |    -     |                -                |    -    | Used as HTML `id` property                                                                             |
| `id`                 |    `string`    |    -     |                -                |    -    | Accepts id                                                                                             |
| `isDisabled`         |     `bool`     |    -     |                -                | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved)                     |
| `onChange`           |     `func`     |    -     |                -                |    -    | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `placeholder`        |    `string`    |    -     |                -                |    -    | Placeholder text for the input                                                                         |
| `scale`              |     `bool`     |    -     |                -                |    -    | Indicates the input field has scale                                                                    |
| `selectedFilterData` |    `object`    |    -     |                -                |    -    | Selected filter data                                                                                   |
| `size`               |    `string`    |          | `base`, `middle`, `big`, `huge` | `base`  | Supported size of the input fields.                                                                    |
| `style`              | `obj`, `array` |    -     |                -                |    -    | Accepts css style                                                                                      |
| `value`              |    `string`    |    -     |                -                |    -    | Value of the input                                                                                     |
