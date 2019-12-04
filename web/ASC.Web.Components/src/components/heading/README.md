# Heading

Heading text structured in levels.

## Usage

```js
import { Heading } from "asc-web-components";
```

```jsx
<Heading as="h1" title="Some title">
  Some text
</Heading>
```

### If you need to override styles add forwardedAs instead of as

```js
const StyledHeading = styled(Heading)`
  &:hover {
    border-bottom: 1px dotted;
  }
`;
```

```jsx
<StyledHeading forwardedAs="h2" title="Some title">
  Some text
</StyledHeading>
```

### Properties

| Props      |   Type   | Required |          Values          |  Default  | Description                                        |
| ---------- | :------: | :------: | :----------------------: | :-------: | -------------------------------------------------- |
| `color`    | `string` |    -     |            -             | `#333333` | Specifies the headline color                       |
| `as`       | `string` |    -     |            -             |   `h1`    | Sets the tag through which to render the component |
| `title`    |  `bool`  |    -     |            -             |     -     | Title                                              |
| `truncate` |  `bool`  |    -     |            -             |  `false`  | Disables word wrapping                             |
| `isInline` |  `bool`  |    -     |            -             |  `false`  | Sets the 'display: inline-block' property          |
| `size`     | `oneOF`  |    -     | `big`, `medium`, `small` |   `big`   | Sets the size of headline                          |
