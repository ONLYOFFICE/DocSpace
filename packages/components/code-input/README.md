# Code Input

Input field for auth code.

### Usage

```js
import CodeInput from "@docspace/components/code-input";
```

```jsx
<CodeInput className="code-input" onSubmit={onSubmit} />
```

### Properties

| Props       |      Type      | Required | Values | Default | Description                                                      |
| ----------- | :------------: | :------: | :----: | :-----: | ---------------------------------------------------------------- |
| `className` |    `string`    |    -     |   -    |    -    | Accepts class                                                    |
| `id`        |    `string`    |    -     |   -    |    -    | Accepts id                                                       |
| `onSubmit`  |     `func`     |    ✅    |   -    |    -    | What the will trigger when press Enter                           |
| `style`     | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                |
| `onChange`  |     `func`     |    -     |   -    |    -    | Sets a callback function that is triggered on the onChange event |
