# ProgressBar

A container that displays a process or operation as a progress bar

### Usage

```js
import { ProgressBar } from "asc-web-components";
```

```jsx
<ProgressBar value={10} maxValue={100} />
```

### Properties

|       Props       |   Type   | Required | Values | Default | Description            |
| :---------------: | :------: | :------: | :----: | :-----: | ---------------------- |
|      `value`      | `number` |    ✅    |   -    |    -    | Progress value.        |
|      `label`      | `string` |    -     |   -    |    -    | Text in progress-bar.  |
|    `maxValue`     | `number` |    -     |   -    |   100   | Max value of progress. |
| `dropDownContent` |  `any`   |    -     |   -    |    -    | Drop-down content.     |
