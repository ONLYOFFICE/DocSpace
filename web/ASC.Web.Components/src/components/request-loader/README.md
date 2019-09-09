# RequestLoader

## Usage

```js
import { RequestLoader } from 'asc-web-components';
```

#### Description

RequestLoader component is used for displaying loading actions on a page.

#### Usage

```js
<RequestLoader label="Loading... Please wait..." />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `visible`         | `bool`   |    -     | -               | `false`         | Visibility                                |
| `loaderSize`         | `number` or `string`   |    -     | -               | -         | Svg height and width value                                |
| `loaderColor`         | `string`   |    -     | -               | -         | Svg color                                |
| `label`         | `string`   |    -     | -               | -         | Svg aria-label and text label                                |
| `fontSize`         | `number` or `string`   |    -     | -               | -         | Text label font size                                |
| `fontColor`          | `string`   |    -    | -                           | -         | Text label font color                                             |