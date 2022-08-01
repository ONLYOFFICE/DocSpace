# AdvancedSelector

## Usage

```js
import AdvancedSelector from "@docspace/common/components/AdvancedSelector";
```

#### Description

Required to select some advanced data.

#### Usage

```js
let options = [{ key: "self", label: "Me" }];

options = [
  ...options,
  ...[...Array(100).keys()].map((index) => {
    return {
      key: `user${index}`,
      label: `User ${index + 1} of ${optionsCount}`,
    };
  }),
];

<AdvancedSelector
  placeholder="Search users"
  onSearchChanged={(e) => console.log(e.target.value)}
  options={options}
  isMultiSelect={false}
  buttonLabel="Add members"
  onSelect={(selectedOptions) => console.log("onSelect", selectedOptions)}
/>;
```

#### Properties

| Props             | Type               | Required | Values | Default | Description |
| ----------------- | ------------------ | :------: | ------ | ------- | ----------- |
| `placeholder`     | `string`           |    -     |        |         |             |
| `options`         | `array of objects` |    -     |        |         |             |
| `isMultiSelect`   | `bool`             |    -     | -      |         |             |
| `buttonLabel`     | `string`           |    -     | -      |         |             |
| `onSearchChanged` | `func`             |    -     | -      |         |             |
| `onSelect`        | `func`             |    -     | -      |         |             |
