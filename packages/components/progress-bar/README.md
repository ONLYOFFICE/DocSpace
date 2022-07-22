# ProgressBar

A container that displays a process or operation as a progress bar

### Usage

```js
import ProgressBar from "@docspace/components/progress-bar";
```

```jsx
<ProgressBar value={10} maxValue={100} />
```

### Properties

|       Props       |   Type   | Required | Values | Default | Description           |
| :---------------: | :------: | :------: | :----: | :-----: | --------------------- |
|     `percent`     | `number` |    ✅    |   -    |    -    | Progress value.       |
|      `label`      | `string` |    -     |   -    |    -    | Text in progress-bar. |
| `dropDownContent` |  `any`   |    -     |   -    |    -    | Drop-down content.    |
