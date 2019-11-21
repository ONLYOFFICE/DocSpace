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
| `visible`     |      `bool`       |    -     |   -    |           `false`           | Visibility                    |
| `loaderSize`  | `number`,`string` |    -     |   -    |            `16`             | Svg height and width value    |
| `loaderColor` |     `string`      |    -     |   -    |           `#999`            | Svg color                     |
| `label`       |     `string`      |    -     |   -    | `Loading... Please wait...` | Svg aria-label and text label |
| `fontSize`    | `number`,`string` |    -     |   -    |            `12`             | Text label font size          |
| `fontColor`   |     `string`      |    -     |   -    |           `#999`            | Text label font color         |
| `zIndex`      |     `string`      |    -     |   -    |            `256`            | CSS z-index                   |