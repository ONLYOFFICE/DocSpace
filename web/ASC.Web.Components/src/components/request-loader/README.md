# RequestLoader

RequestLoader component is used for displaying loading actions on a page

### Usage

```js
import { RequestLoader } from "asc-web-components";
```

```jsx
<RequestLoader label="Loading... Please wait..." />
```

### Properties

| Props         |       Type        | Required | Values |           Default           | Description                   |
| ------------- | :---------------: | :------: | :----: | :-------------------------: | ----------------------------- |
| `className`   |     `string`      |    -     |   -    |              -              | Accepts class                 |
| `fontColor`   |     `string`      |    -     |   -    |           `#999`            | Text label font color         |
| `fontSize`    | `number`,`string` |    -     |   -    |            `12`             | Text label font size          |
| `id`          |     `string`      |    -     |   -    |              -              | Accepts id                    |
| `label`       |     `string`      |    -     |   -    | `Loading... Please wait...` | Svg aria-label and text label |
| `loaderColor` |     `string`      |    -     |   -    |           `#999`            | Svg color                     |
| `loaderSize`  | `number`,`string` |    -     |   -    |            `16`             | Svg height and width value    |
| `style`       |  `obj`, `array`   |    -     |   -    |              -              | Accepts css style             |
| `visible`     |      `bool`       |    -     |   -    |           `false`           | Visibility                    |
| `zIndex`      |     `string`      |    -     |   -    |            `256`            | CSS z-index                   |
