# Headline

Component that displays Heading text with custom styles

### Usage

```js
import Headline from "@docspace/common/components/Headline";
```

```jsx
<Headline type="content" title="Some title" isInline>
  Some text
</Headline>
```

```jsx
<Headline type="header" title="Some title">
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

| Props      |   Type   | Required |         Values          |  Default  | Description                                                         |
| ---------- | :------: | :------: | :---------------------: | :-------: | ------------------------------------------------------------------- |
| `color`    | `string` |    -     |            -            | `#333333` | Specifies the contentHeader color                                   |
| `isInline` |  `bool`  |    -     |            -            |  `false`  | Sets the 'display: inline-block' property                           |
| `title`    |  `bool`  |    -     |            -            |     -     | Title                                                               |
| `truncate` |  `bool`  |    -     |            -            |  `false`  | Disables word wrapping                                              |
| `type`     | `oneOf`  |    ✅    | `content, header, menu` |     -     | Sets the size of text: content (21px), header (28px) or menu (27px) |
