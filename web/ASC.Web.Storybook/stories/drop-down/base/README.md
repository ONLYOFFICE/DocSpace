# DropDown

## Usage

```js
import { DropDown } from 'asc-web-components';
```

#### Description

Is a dropdown with any number of action

#### Usage

```js
<DropDown opened={false}></DropDown>
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `opened`           | `bool`   |    -     | -                           | `false`        | Tells when the dropdown should be opened                          |
| `directionX`       | `oneOf`  |    -     | `left`, `right`             | `left`         | Sets the opening direction relative to the parent                 |
| `directionY`       | `oneOf`  |    -     | `top`, `bottom`             | `bottom`       | Sets the opening direction relative to the parent                 |
| `manualWidth`      | `string` |    -     | -                           | -              | Required if you need to specify the exact width of the component, for example 100%|
| `manualY`          | `string` |    -     | -                           | -              | Required if you need to specify the exact distance from the parent component|
| `maxHeight`        | `number` |    -     | -                           | -              | Required if the scrollbar is displayed                            |