# Text

Component that displays plain text

## Usage

```js
import Text from "@docspace/components/text";
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

| Props             |            Type             | Required | Values |  Default  | Description                                        |
| ----------------- | :-------------------------: | :------: | :----: | :-------: | -------------------------------------------------- |
| `as`              |          `string`           |    -     |   -    |    `p`    | Sets the tag through which to render the component |
| `backgroundColor` |          `string`           |    -     |   -    |     -     | Sets background color                              |
| `color`           |          `string`           |    -     |   -    | `#333333` | Specifies the text color                           |
| `display`         |          `string`           |    -     |   -    |     -     | Sets the 'display' property                        |
| `fontSize`        |          `string`           |    -     |   -    |  `13px`   | Sets the font size                                 |
| `fontWeight`      | `oneOfType(number, string)` |    -     |   -    |     -     | Sets the font weight                               |
| `isBold`          |           `bool`            |    -     |   -    |  `false`  | Sets font weight value ​​to bold                   |
| `isInline`        |           `bool`            |    -     |   -    |  `false`  | Sets the 'display: inline-block' property          |
| `isItalic`        |           `bool`            |    -     |   -    |  `false`  | Sets the font style                                |
| `title`           |           `bool`            |    -     |   -    |     -     | Title                                              |
| `truncate`        |           `bool`            |    -     |   -    |  `false`  | Disables word wrapping                             |
