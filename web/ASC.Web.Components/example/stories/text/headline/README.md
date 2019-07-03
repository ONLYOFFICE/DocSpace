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

