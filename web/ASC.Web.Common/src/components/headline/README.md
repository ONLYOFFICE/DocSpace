# Headline

Component that displays Heading text with custom styles

### Usage

```js
import { Headline } from "asc-web-common";
```

```jsx
<Headline type="content" title="Some title" isInline>
  Some text
</Headline>
```

```jsx
<Headline type="menu" title="Some title">
  Some text
</Headline>
```

#### If you need to override styles add forwardedAs instead of as

```js
const StyledText = styled(Headline)`
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

| Props      |   Type   | Required |     Values      |  Default  | Description                                          |
| ---------- | :------: | :------: | :-------------: | :-------: | ---------------------------------------------------- |
| `color`    | `string` |    -     |        -        | `#333333` | Specifies the contentHeader color                    |
| `title`    |  `bool`  |    -     |        -        |     -     | Title                                                |
| `truncate` |  `bool`  |    -     |        -        |  `false`  | Disables word wrapping                               |
| `isInline` |  `bool`  |    -     |        -        |  `false`  | Sets the 'display: inline-block' property            |
| `type`     | `oneOf`  |    ✅    | `menu, content` |     -     | Sets the size of text: menu (27px) or content (21px) |
