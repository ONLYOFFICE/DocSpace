# Textarea

Textarea is used for displaying custom textarea

### Usage

```js
import { Textarea } from "asc-web-components";
```

```jsx
<Textarea
  placeholder="Add comment"
  onChange={event => alert(event.target.value)}
  value="value"
/>
```

### Properties

| Props         |   Type   | Required | Values | Default | Description                                              |
| ------------- | :------: | :------: | :----: | :-----: | -------------------------------------------------------- |
| `value`       | `string` |    -     |   -    |    -    | Value for Textarea                                       |
| `placeholder` | `string` |    -     |   -    |    -    | Placeholder for Textarea                                 |
| `onChange`    |  `func`  |    -     |   -    |    -    | Allow you to handle changing events of component         |
| `id`          | `string` |    -     |   -    |    -    | Used as HTML `id` property                               |
| `name`        | `string` |    -     |   -    |    -    | Used as HTML `name` property                             |
| `isDisabled`  |  `bool`  |    -     |   -    | `false` | Indicates that the field cannot be used                  |
| `isReadOnly`  |  `bool`  |    -     |   -    | `false` | Indicates that the field is displaying read-only content |
| `className`   | `string` |    -     |   -    |    -    | Class name                                               |
