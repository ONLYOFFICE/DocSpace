# Text

## Usage

```js
import { Text } from 'asc-web-components';
```

#### Description

Text used in titles and content

#### Usage

```js
    <Text elementType='h1' title='Some title'>
        Some text
    </Text>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isDisabled`       | `bool`   |    -     | -                     | false     | Marks text as disabled                              |
| `elementType`      | `select` |    -     |`h1`,`h2`,`h3`,`p`,`moduleName`,`mainTitle` | `p`        |Sets the text type with its own font size and font weight|
| `styleType`        | `bool`   |    -     | -                     | `default`     | Style type                                      |
| `title`            | `bool`   |    -     | -                     | -     | Title                                                   |
| `truncate`         | `bool`   |    -     | -                     | false     | Disables word wrapping                              |


