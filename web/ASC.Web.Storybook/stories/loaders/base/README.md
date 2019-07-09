# Loaders: Base

## Usage

```js
import { Loader } from 'asc-web-components';
```

#### Description

Loader component is used for displaying loading actions on a page.

#### Usage

```js
<Loader type="base" color="black" size={18} label="Loading" />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `type`             | `oneOf`  |    -     | `base`, `oval`, `dual-ring`, `rombs` | `base`     | -                                                                     |
| `color`          | `string`   |    -     | -                           | -         | Font color                                  |
| `size`         | `number` or `string`   |    -     | -               | -         | Font size                                |
| `label`          | `string`   |    -    | -                           | -         | Text label                                             |


