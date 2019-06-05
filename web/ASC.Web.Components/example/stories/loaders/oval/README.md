# Loaders: Oval

## Usage

```js
import { Loader } from 'asc-web-components';
```

#### Description

Loader component is used for displaying loading actions on a page.

#### Usage

```js
<Loader color="black" size={40} type="oval" label="Loading..." />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `color`          | `string`   |    -     | -                           | -         | Svg color                                  |
| `type`             | `oneOf`  |    -     | `base`, `oval`, `dual-ring`, `rombs` | `base`     | -                                                                     |
| `size`         | `number` or `string`   |    -     | -               | -         | Svg height and width value                                |
| `label`          | `string`   |    -    | -                           | -         | Svg aria-lable or text label                                             |


