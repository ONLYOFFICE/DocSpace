# Loaders: Loader

## Usage

```js
import { Loader } from 'asc-web-components';
```

#### Description

Loader component is used for displaying loading actions on a page.

#### Usage

```js
<Loader color="black" width={64} height={64} type="oval" label="Loading..." />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `color`          | `string`   |    -     | -                           | -         | Svg color                                  |
| `type`             | `oneOf`  |    -     | `base`, `oval`, `dual-ring` | `base`     | -                                                                     |
| `height`         | `string` or `string`   |    -     | -               | -         | Svg height                                |
| `width`         | `string` or `string`   |    -     | -                | -         | Svg width                              |
| `label`          | `string`   |    -    | -                           | -         | Svg aria-lable or text label                                             |


