# Text

## Usage

```js
import { Text } from 'asc-web-components';
```

### <Text.Headline>

Wraps the given text in the given HTML header size.

#### Usage

```js
    <Text.Headline tag='h1' title='Some title'>
        Some text
    </Text.Headline>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isDisabled`       | `bool`   |    -     | -                     | false     | Marks text as disabled                                  |
| `tag`              | `oneOf`  |    -     | `h1`,`h2`,`h3`        | `h1`      | Sets the text type with its own font size               |
| `title`            | `bool`   |    -     | -                     | -         | Title                                                   |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                                  |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property               |

### <Text.Body>

Wraps the given text in a <p> or <span> element, for normal content.

#### Usage

```js
    <Text.Body tag='p' title='Some title'>
        Some text
    </Text.Body>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isDisabled`       | `bool`   |    -     | -                     | false     | Marks text as disabled                              |
| `tag`              | `oneOf`  |    -     | `p`,`span`            | `p`       | Sets the text type with its own font size           |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |
| `color`            | `oneOf`  |    -     | `black`, `gray`, `lightGray`      | `black`     | Sets the text color                   |
| `isBold`           | `bool`   |    -     | -                     | false     | Sets the font weight                                |
| `isItalic`         | `bool`   |    -     | -                     | false     | Sets the font style                                 |
| `backgroundColor`  | `bool`   |    -     | -                     | false     | Sets background color                               |


### <Text.MainHeadline>

Wraps the given text in the specified size of the main headline.

#### Usage

```js
    <Text.MainHeadline headlineName='moduleName' title='Some title'>
        Some text
    </Text.MainHeadline>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isDisabled`       | `bool`   |    -     | -                     | false     | Marks text as disabled                              |
| `headlineName`     | `oneOf`  |    -     |`moduleName`, `mainTitle`| `moduleName` | Sets the text type with its own font size      |
| `title`            | `bool`   |    -     | -                     | -         | Title                                               |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |
| `isInline`         | `bool`   |    -     | -                     | false     | Sets the 'display: inline-block' property           |


