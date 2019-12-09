# Text

Component that displays plain text

## Usage

```js
import { Text } from "asc-web-components";
```

```jsx
<Text as="p" title="Some title">
  Some text
</Text>
```

### If you need to override styles add forwardedAs instead of as

```js
const StyledText = styled(Text)`
  &:hover {
    border-bottom: 1px dotted;
  }
`;
```

```jsx
<StyledText forwardedAs="span" title="Some title">
  Some text
</StyledText>
```

### Properties

| Props             |   Type   | Required | Values |  Default  | Description                                        |
| ----------------- | :------: | :------: | :----: | :-------: | -------------------------------------------------- |
| `fontSize`        | `oneOfType(number, string)` |    -     |   -    |   `13`    | Sets the font size                                 |
| `as`              | `string` |    -     |   -    |    `p`    | Sets the tag through which to render the component |
| `title`           |  `bool`  |    -     |   -    |     -     | Title                                              |
| `truncate`        |  `bool`  |    -     |   -    |  `false`  | Disables word wrapping                             |
| `isInline`        |  `bool`  |    -     |   -    |  `false`  | Sets the 'display: inline-block' property          |
| `display`         | `string` |    -     |   -    |     -     | Sets the 'display' property                        |
| `color`           | `string` |    -     |   -    | `#333333` | Specifies the text color                           |
| `isBold`          |  `bool`  |    -     |   -    |  `false`  | Sets font weight value ​​to bold                   |
| `isItalic`        |  `bool`  |    -     |   -    |  `false`  | Sets the font style                                |
| `backgroundColor` | `string` |    -     |   -    |     -     | Sets background color                              |
| `fontWeight`      | `number` |    -     |   -    |     -     | Sets the font weight                               |
