# Backdrop

Background for displaying modal dialogs

### Usage

```js
import { Backdrop } from "asc-web-components";
```

```jsx
<Backdrop visible={true} zIndex={1} />
```

### Properties

| Props       |      Type      | Required | Values | Default | Description       |
| ----------- | :------------: | :------: | :----: | :-----: | ----------------- |
| `className` |    `string`    |    -     |   -    |    -    | Accepts class     |
| `id`        |    `string`    |    -     |   -    |    -    | Accepts id        |
| `style`     | `obj`, `array` |    -     |   -    |    -    | Accepts css style |
| `visible`   |     `bool`     |    -     |   -    | `false` | Display or not    |
| `zIndex`    |    `number`    |    -     |   -    |  `100`  | CSS z-index       |
