# RowContainer

Container for Row component

### Usage

```js
import RowContainer from "@docspace/components/row-container";
```

```jsx
<RowContainer manualHeight="500px">{children}</RowContainer>
```

### Properties

| Props            |      Type      | Required | Values |    Default     | Description                                                     |
| ---------------- | :------------: | :------: | :----: | :------------: | --------------------------------------------------------------- |
| `className`      |    `string`    |    -     |   -    |       -        | Accepts class                                                   |
| `id`             |    `string`    |    -     |   -    | `rowContainer` | Accepts id                                                      |
| `itemHeight`     |    `number`    |    -     |   -    |      `50`      | Height of one Row element. Required for scroll to work properly |
| `manualHeight`   |    `string`    |    -     |   -    |       -        | Allows you to set fixed block height for Row                    |
| `style`          | `obj`, `array` |    -     |   -    |       -        | Accepts css style                                               |
| `useReactWindow` |     `bool`     |    -     |   -    |     `true`     | Use react-window for efficiently rendering large lists          |
