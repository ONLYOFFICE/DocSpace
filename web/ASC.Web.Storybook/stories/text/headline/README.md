# Text

## Usage

```js
import { Text } from 'asc-web-components';
```

### <Text.Headline>

A component that renders headline

#### Usage

```js
    <Text.Headline as='h1' title='Some title'>
        Some text
    </Text.Headline>
```

#####   If you need to override styles add forwardedAs instead of as

```js

    const StyledHeadline = styled(Text.Headline)`
        &:hover{
            border-bottom: 1px dotted;
        }
    `;

    <StyledHeadline forwardedAs='h4' title='Some title'>
        Some text
    </StyledHeadline>

```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `color`            | `string` |    -     | -                     | '#333333' | Specifies the headline color                            |
| `as `              | `string` |    -     | -                     | `h1`      | Sets the tag through which to render the component      |
| `title`            | `bool`   |    -     | -                     | -         | Title                                                   |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                                  |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property               |
| `size`             | `oneOF`  |    -     | `big`, `medium`, `small` | `big`  | Sets the size of headline                               |

