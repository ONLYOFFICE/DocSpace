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

    <Text.ContentHeader title='Some title' isInline>
        Some text
    </Text.ContentHeader>
	
	<Text.Headline as='h1' title='Some title'>
        Some text
    </Text.Headline>
	
	<Text.MenuHeader title='Some title' isInline>
        Some text
    </Text.MenuHeader>
```

#####  If you need to override styles add forwardedAs instead of as

```js

    const StyledText = styled(Text.Body)`
        &:hover{
            border-bottom: 1px dotted;
        }
    `;

    <StyledText forwardedAs='span' title='Some title'>
        Some text
    </StyledText>

```

#### Properties Text.Body

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `fontSize`         | `number` |    -     | -                     | `13`      | Sets the font size                                  |
| `as`               | `string` |    -     | -                     | `p`       | Sets the tag through which to render the component  |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |
| `display`          | `string` |    -     | -                     | -         | Sets the 'display' property                         |
| `color`            | `string` |    -     | -                     | `#333333` | Specifies the text color                            |
| `isBold`           | `bool`   |    -     | -                     | false     | Sets font weight value ​​to bold                      |
| `isItalic`         | `bool`   |    -     | -                     | false     | Sets the font style                                 |
| `backgroundColor`  | `string` |    -     | -                     | -         | Sets background color                               |
| `fontWeight`       | `number` |    -     | -                     | -         | Sets the font weight                                |

#### Properties Text.ContentHeader

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `color`            | `string` |    -     | -                     | '#333333' | Specifies the contentHeader color                   |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |

#### Properties Text.Headline

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `color`            | `string` |    -     | -                     | '#333333' | Specifies the headline color                            |
| `as `              | `string` |    -     | -                     | `h1`      | Sets the tag through which to render the component      |
| `title`            | `bool`   |    -     | -                     | -         | Title                                                   |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                                  |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property               |
| `size`             | `oneOF`  |    -     | `big`, `medium`, `small` | `big`  | Sets the size of headline                               |

#### Properties Text.MenuHeader

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `color`            | `string` |    -     | -                     | '#333333' | Specifies the menuHeader color                      |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |

