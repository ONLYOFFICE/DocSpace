# Heading

Heading text structured in levels.

## Usage

```js
import Heading from "@docspace/components/heading";
```

```jsx
<Heading level={1} title="Some title">
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

| Props      |   Type   | Required |                     Values                     |  Default  | Description                                                                                                                           |
| ---------- | :------: | :------: | :--------------------------------------------: | :-------: | ------------------------------------------------------------------------------------------------------------------------------------- |
| `color`    | `string` |    -     |                       -                        | `#333333` | Specifies the headline color                                                                                                          |
| `isInline` |  `bool`  |    -     |                       -                        |  `false`  | Sets the 'display: inline-block' property                                                                                             |
| `level`    | `oneOf`  |    -     |                1, 2, 3, 4, 5, 6                |    `1`    | The heading level. It corresponds to the number after the 'H' for the DOM tag. Set the level for semantic accuracy and accessibility. |
| `size`     | `oneOF`  |    -     | `xsmall`, `small`, `medium`, `large`, `xlarge` |  `large`  | Sets the size of headline                                                                                                             |
| `title`    |  `bool`  |    -     |                       -                        |     -     | Title                                                                                                                                 |
| `truncate` |  `bool`  |    -     |                       -                        |  `false`  | Disables word wrapping                                                                                                                |
