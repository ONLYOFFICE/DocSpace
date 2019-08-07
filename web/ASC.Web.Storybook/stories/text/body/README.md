# Text

## Usage

```js
import { Text } from 'asc-web-components';
```

### <Text.Body>

Component that displays plain text

#### Usage

```js
    <Text.Body as='p' title='Some title'>
        Some text
    </Text.Body>

```

#####  If you need to override styles

```js

    const StyledText = styled(Text.Body)`
        &:hover{
            border-bottom: 1px dotted;
        }
    `;

    <StyledText forwardedAs='p' title='Some title'>
        Some text
    </StyledText>

```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |

| `isDisabled`       | `bool`   |    -     | -                     | false     | Marks text as disabled                              |
| `fontSize`         | `number` |    -     | -                     | `13`      | Sets the font size                                  |
| `as`               | `string` |    -     | -                     | `p`       | Sets the tag through which to render the component  |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |
| `color`            | `string` |    -     | -                     | `#333333` | Specifies the text color                            |
| `disableColor`     | `string` |    -     |                       | `#ECEEF1` | Sets the text disabled color                        |
| `isBold`           | `bool`   |    -     | -                     | false     | Sets the font weight                                |
| `isItalic`         | `bool`   |    -     | -                     | false     | Sets the font style                                 |
| `backgroundColor`  | `bool`   |    -     | -                     | false     | Sets background color                               |

