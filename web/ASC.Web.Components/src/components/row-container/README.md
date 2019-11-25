# RowContainer

Container for Row component

### Usage

```js
import { RowContainer } from "asc-web-components";
```

```jsx
<RowContainer manualHeight="500px">{children}</RowContainer>
```

### Properties

| Props            |   Type   | Required | Values | Default | Description                                                     |
| ---------------- | :------: | :------: | :----: | :-----: | --------------------------------------------------------------- |
| `manualHeight`   | `string` |    -     |   -    |    -    | Allows you to set fixed block height for Row                    |
| `itemHeight`     | `number` |    -     |   -    |  `50`   | Height of one Row element. Required for scroll to work properly |
| `useReactWindow` |  `bool`  |    -     |   -    | `true`  | Use react-window for efficiently rendering large lists          |
