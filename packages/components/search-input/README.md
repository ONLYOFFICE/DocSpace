# SearchInput

SearchInput description

### Usage

```js
import SearchInput from "@docspace/components/search-input";
```

```jsx
<SearchInput
  isNeedFilter={true}
  getFilterData={() => [
    {
      key: "filter-example",
      group: "filter-example",
      label: "example group",
      isHeader: true,
    },
    { key: "filter-example-test", group: "filter-example", label: "Test" },
  ]}
  onSearchClick={(result) => {
    console.log(result);
  }}
  onChangeFilter={(result) => {
    console.log(result);
  }}
/>
```

### Properties

| Props                |      Type      | Required |             Values              | Default | Description                                                                                            |
| -------------------- | :------------: | :------: | :-----------------------------: | :-----: | ------------------------------------------------------------------------------------------------------ |
| `className`          |    `string`    |    -     |                -                |    -    | Accepts class                                                                                          |
| `id`                 |    `string`    |    -     |                -                |    -    | Used as HTML `id` property                                                                             |
| `isDisabled`         |     `bool`     |    -     |                -                | `false` | Indicates that the field cannot be used (e.g not authorized, or changes not saved)                     |
| `isNeedFilter`       |     `bool`     |    -     |                -                | `false` | Determines if filter is needed                                                                         |
| `onChange`           |     `func`     |    -     |                -                |    -    | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `placeholder`        |    `string`    |    -     |                -                |    -    | Placeholder text for the input                                                                         |
| `scale`              |     `bool`     |    -     |                -                |    -    | Indicates the input field has scale                                                                    |
| `selectedFilterData` |    `array`     |    -     |                -                |    -    | Selected filter data                                                                                   |
| `size`               |    `string`    |    -     | `base`, `middle`, `big`, `huge` | `base`  | Supported size of the input fields.                                                                    |
| `style`              | `obj`, `array` |    -     |                -                |    -    | Accepts css style                                                                                      |
| `value`              |    `string`    |    -     |                -                |    -    | Value of the input                                                                                     |
